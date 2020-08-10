//https://www.shadertoy.com/view/ltyfDm
#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

out vec4 fragColor;

// LOOKS BEST IN FULLSCREEN

// raymarching defines
#define FAR 100.0
#define DISTANCE_BIAS 0.75
#define EPSILON 0.0001



// part of the edge detection which I learned here by nimitz: https://www.shadertoy.com/view/4s2XRd
// I had a similar approach before, but this method looks better.
#define EDGE_SIZE 0.022
#define SMOOTH 0.02
#define SIZE 0.9

// sdf subtraction from Iq's website
float opSub( float d1, float d2 ) { return max(-d1,d2); }

// normalized mouse coords
vec2 m(){
    vec2 m = iMouse.xy / iResolution.xy-.5; 
    m.x *= iResolution.x/iResolution.y;
	return m;
}

// Noise and rand found everywhere on shadertoy
float rand(vec2 n){ 
	return fract(sin(dot(n, vec2(17.12037, 5.71713))) * 12345.6789);
}

float noise(vec2 n){
	vec2 d = vec2(0.0, 1.0);
	vec2 b = floor(n), f = smoothstep(vec2(0.0), vec2(1.0), fract(n));
	return mix(mix(rand(b + d.xx), rand(b + d.yx), f.x), mix(rand(b + d.xy), rand(b + d.yy), f.x), f.y);
}

float map(vec3 rp)
{
    // sphere pos
    vec3 pos = rp - vec3(1.0, -1.0, 0.0); 
    
    pos.z -= 0.035;
    
    // movements
    pos.x += iTime * 0.5;
    pos.y -= m().y*8.0;
    pos.x -= m().x*8.0;
    
    // repeat coordinates
    pos = mod(pos, vec3(2))-0.5 * vec3(2);
    
    // Endless filled space that starts in front of the camera 
    float res = 2.00001 - rp.z;
    
    // subtract from the space using spheres
    res = opSub(length(pos) - 1.33, res);
   
    return res;
}

// Got this from one of Shane's shaders. So fun to play with
float softShadow(vec3 ro, vec3 lp, float k)
{
    const int maxIterationsShad = 32; 
    
    vec3 rd = (lp-ro); 

    float shade = 1.;
    float dist = .005;    
    float end = max(length(rd), 0.001);
    float stepDist = end/float(maxIterationsShad);
    
    rd /= end;

    for (int i=0; i<maxIterationsShad; i++){

        float h = map(ro + rd*dist);
       
        shade = min(shade, smoothstep(0.0, 1.0, k*h/dist)); 
       
        dist += clamp(h, .02, .2);
        
       
        if (h<0.0 || dist > end) break; 
       
    }

   
    return min(max(shade, 0.) + 0.03, 1.0); 
}


vec3 getNormal(in vec3 p) {
	const vec2 e = vec2(0.002, 0);
	return normalize(vec3(map(p + e.xyy) - map(p - e.xyy), map(p + e.yxy) - map(p - e.yxy),	map(p + e.yyx) - map(p - e.yyx)));
}

// Directly from Iq's site: http://iquilezles.org/www/articles/distfunctions/distfunctions.htm
float calcAO( in vec3 pos, in vec3 nor )
{
	float occ = 0.0;
    float sca = 1.0;
    for( int i=0; i<5; i++ )
    {
        float hr = 0.01 + 0.12*float(i)/4.0;
        vec3 aopos =  nor * hr + pos;
        float dd = map( aopos );
        occ += -(dd-hr)*sca;
        sca *= 0.95;
    }
    return clamp( 1.0 - 3.0*occ, 0.0, 1. );    
}




    
 
// A mixture of techniques here that Iv'e seen used by Iq and Shane
vec3 color(vec3 ro, vec3 rd, vec3 norm, vec3 lp, float t, float md)
{
    
    
    vec3 p = ro + rd * t; // hit location
    
    // Lighting
    vec3 ld = lp-ro;
    float lDist = max(length(ld), 0.001); // Light to surface distance.
    float atten = 2.0 / (1.0 + lDist*0.2 + lDist*lDist*0.1); // light attenuation 
    ld /= lDist;
    
    
    float diff = max(dot(norm, ld), 0.0); // diffuse
    float spec = pow(max( dot( reflect(-ld, norm), -rd ), 0.0 ), 12.0); // specular
    
    float occ = calcAO( ro + rd, norm )*1.0; // get AO 
    
    float amb = clamp( 0.5 + 0.5 * norm.y, 0.0, 1.0 ); // ambient
    float fre = pow( clamp(1.0 + dot(norm,rd),0.0,1.0), 2.0 ); // fresnel
    // 
    
    
    
    vec3 lf = vec3(0.0); // light color
    
    // adding all the light terms
    lf += 1.0 * amb * vec3(0.5,0.5,0.5) * occ;
    lf += 2.0 * fre * vec3(0.7,0.5,0.0) * occ;
    lf += diff + 0.5;
    
    // moving the hit location with the geometry. Im sure I'll figure out a better way
    p.x += iTime;  
    p.y -= m().y*16.0;
    p.x -= m().x*16.0;
        
    float nn = noise(p.xy*5.0); // some noise
    
    // pink blue pattern
    vec3 col = vec3(sin(nn*10.0), cos(nn * 10.0), 1.0) * 0.9;
    
    // simply colors the scene yellow if the z normal if below a threshold. quick and easy
    // but probably not sustainable 
    if(norm.x != 0.0 && p.z < 4.1)
        col = vec3(1.0, 1.0, 0.0);
           
    // apply lighting to main color 
    col *= lf;
    
    // some additional lighting / coloring 
    col  = (col*(diff + 0.7) + vec3(1.0, 0.6, 0.5)*spec*0.2);
    
    // apply attenuation
    col *= atten;
    
    // adds a little more detail to texture
    col *= clamp(noise(p.xy*52.0), 0.3, 1.0) * 1.4;
    col *= 0.8;
    return col;
    
}



vec2 trace(vec3 ro, vec3 rd)
{
    float t = 0.0; // total distance
    float d = 0.0; // distance to nearest object
    
    float md = 999.0; 
    
    float h = EPSILON; 
  
    vec2 dd = vec2(0.,10000.);
    
    bool stp = false;
    
    for (int i = 0; i < 64; i++) 
    {
        d = map(ro + rd*t); // get distance to nearest object
        
        if(abs(d)<EPSILON || t > FAR) {break;} // hit object or went too far
        
        
        // Part of the edge detection. Basically checks if we are within the set edge bounds 
        // of the object the ray is passing
        if (stp == false) 
        {
            md = min(md,d); // get current minimum distance
            
            if (h < EDGE_SIZE && h < d && i > 0)
            {
                stp = true;
                dd.y = dd.x;
            }
        }
        
         h = d;
        
        t += d * DISTANCE_BIAS;
        
    }
    
    if (stp) md = smoothstep(EDGE_SIZE-SMOOTH, EDGE_SIZE+SIZE, md);
    else md = 1.0;
    
    return vec2(t, md );
}


void main()
{
    vec2 uv = 2.0 * vec2(gl_FragCoord.xy - 0.5*iResolution.xy)/iResolution.y; 
 
    vec3 ro = vec3(0.0, 0.0, 0.0); 
 
    // fish eyed ray direction vector because this thing isnt a sphere haha fooled you
     vec3 rd = normalize(vec3(uv, 1.0 - dot(uv, uv) * 0.35));
    
   
    // light position. Fun to play with. bottom one looks pretty cool because the light goes
    // in the ball
    vec3 lp = ro + vec3(0.8, 0.7, 0.0);
    //vec3 lp = ro + vec3(1.0, 0.7, sin(iTime) * 3.0);
    
    // Distance to scene object and minimum distance 
    //to an object if nothing was hit
    vec2 t = trace(ro, rd); 
    						
    // set the ray origin location to the hit location
    ro += rd * t.x; 
    
    // normals of hit object
    vec3 norm = getNormal(ro); 
    
    // shadows
    float sh = softShadow(ro, lp, 120.); 
    
    // color the scene...except for fog and
    // edge outline which happens below
    vec3 col = color(ro, rd, norm, lp, t.x, t.y); 
    												
    
    vec3 fogCol = vec3(0.0, 0.0, 0.0);
    
    col *= sh; // apply shadows to scene
        
   
    col *= mix(t.y,1.0, smoothstep(44.0,45.0,t.x)); // apply the edge outline
    
    col = mix( col, vec3(0), 1.0 - exp( -0.0036*t.x*t.x*t.x )); // some fog
    
   
    fragColor = vec4(sqrt(clamp(col, 0.0, 1.0)), 1.0);
 
}
