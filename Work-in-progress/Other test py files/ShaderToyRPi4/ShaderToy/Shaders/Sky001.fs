//https://www.shadertoy.com/view/XsVGz3

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform sampler2D iChannel3;

out vec4 fragColor;

#define MIN_HEIGHT 2.0
#define MAX_HEIGHT 4.5
#define WIND vec2(0.2, 0.1)

vec3 sundir = normalize(vec3(1.0,0.75,1.0));

float noise( in vec3 x )
{
    vec3 f = fract(x);
    vec3 p = floor(x);
    f = f * f * (3.0 - 2.0 * f);
    
    p.xz += WIND * iTime;
    vec2 uv = (p.xz + vec2(37.0, 17.0) * p.y) + f.xz;
    vec2 rg = texture(iChannel0, (uv + 0.5)/256.0, 0.0).yx;
    return mix(rg.x, rg.y, f.y);
}

float fractal_noise(vec3 p)
{
    float f = 0.0;
    // add animation
    //p = p - vec3(1.0, 1.0, 0.0) * iTime * 0.1;
    p = p * 3.0;
    f += 0.50000 * noise(p); p = 2.0 * p;
	f += 0.25000 * noise(p); p = 2.0 * p;
	f += 0.12500 * noise(p); p = 2.0 * p;
	f += 0.06250 * noise(p); p = 2.0 * p;
    f += 0.03125 * noise(p);
    
    return f;
}

float density(vec3 pos)
{    
    float den = 3.0 * fractal_noise(pos * 0.3) - 2.0 + (pos.y - MIN_HEIGHT);
    float edge = 1.0 - smoothstep(MIN_HEIGHT, MAX_HEIGHT, pos.y);
    edge *= edge;
    den *= edge;
    den = clamp(den, 0.0, 1.0);
    
    return den;
}

vec3 raymarching(vec3 ro, vec3 rd, float t, vec3 backCol)
{   
    vec4 sum = vec4(0.0);
    vec3 pos = ro + rd * t;
    for (int i = 0; i < 40; i++) {
        if (sum.a > 0.99 || 
            pos.y < (MIN_HEIGHT-1.0) || 
            pos.y > (MAX_HEIGHT+1.0)) break;
        
        float den = density(pos);
        
        if (den > 0.01) {
            float dif = clamp((den - density(pos+0.3*sundir))/0.6, 0.0, 1.0);

            vec3 lin = vec3(0.65,0.7,0.75)*1.5 + vec3(1.0, 0.6, 0.3)*dif;        
            vec4 col = vec4( mix( vec3(1.0,0.95,0.8)*1.1, vec3(0.35,0.4,0.45), den), den);
            col.rgb *= lin;

            // front to back blending    
            col.a *= 0.5;
            col.rgb *= col.a;

            sum = sum + col*(1.0 - sum.a); 
        }
        
        t += max(0.05, 0.02 * t);
        pos = ro + rd * t;
    }
    
    sum = clamp(sum, 0.0, 1.0);
    
    float h = rd.y;
    sum.rgb = mix(sum.rgb, backCol, exp(-20.*h*h) );
    
    return mix(backCol, sum.xyz, sum.a);
}

float planeIntersect( vec3 ro, vec3 rd, float plane)
{
    float h = plane - ro.y;
    return h/rd.y;
}

mat3 setCamera(vec3 ro, vec3 ta, float cr)
{
	vec3 cw = normalize(ta-ro);
	vec3 cp = vec3(sin(cr), cos(cr),0.0);
	vec3 cu = normalize( cross(cw,cp) );
	vec3 cv = normalize( cross(cu,cw) );
    return mat3( cu, cv, cw );
}

void main()
{
	vec2 p = (2.0 * gl_FragCoord.xy - iResolution.xy) / iResolution.yy;
    vec2 mo = vec2(0.0);
    if (iMouse.z > 0.0) 
    {
        mo += (2.0 * iMouse.xy - iResolution.xy) / iResolution.yy;
    }
    
    vec3 ro = vec3(0.0, 0.0, -2.0);
    
    // Rotate the camera
    vec3 target = vec3(ro.x+10., 1.0+mo.y*3.0, ro.z);
    
    vec2 cossin = vec2(cos(mo.x), sin(mo.x));
    mat3 rot = mat3(cossin.x, 0.0, -cossin.y,
                   	0.0, 1.0, 0.0,
                   	cossin.y, 0.0, cossin.x);
    target = rot * (target - ro) + ro;
    
    // Compute the ray
    vec3 rd = setCamera(ro, target, 0.0) * normalize(vec3(p.xy, 1.5));
    
    float dist = planeIntersect(ro, rd, MIN_HEIGHT);
    
    float sun = clamp(dot(sundir, rd), 0.0, 1.0);
	vec3 col = mix(vec3(0.78,0.78,0.7), vec3(0.3,0.4,0.5), p.y * 0.5 + 0.5);
	col += 0.5*vec3(1.0,0.5,0.1)*pow(sun, 8.0);
    
    if (dist > 0.0) {
        col = raymarching(ro, rd, dist, col);
    }
    
    fragColor = vec4(col, 1.0);
}
