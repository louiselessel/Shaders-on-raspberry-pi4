//https://www.shadertoy.com/view/MsVyRh

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

out vec4 fragColor;

vec2 path(float t) {
    float a = sin(t*.2+1.5),b=sin(t*.2);
    return vec2(a*2., a*b);
}

mat2 r2d(float a) {
    float c=cos(a),s=sin(a);
    return mat2(c, s, -s, c);
}

void mo(inout vec2 p, vec2 d) {
    p.x = abs(p.x) - d.x;
    p.y = abs(p.y) - d.y;
    if(p.y>p.x)p=p.yx;
}

float g=0.;
float de(vec3 p) {
   
    vec3 q = p;
    q.x += q.z*.1;
    q.z += iTime*.1;
    q = mod(q-1., 2.)-1.;
    float s = length(q) - .001 + sin(iTime*30.)*.005;
    
    p.xy -= path(p.z);
    
    p.xy *= r2d(p.z*.9);
    mo(p.xy, vec2(.6, .12));
    mo(p.xy, vec2(.9, .2));
    
    p.xy *= r2d(p.z*.5);
    
    mo(p.zy, vec2(.1, .2));
    p.x = abs(p.x) - .4;
    float d = length(p.xy) - .02 - (.5+.5*sin(p.z))*.05;
    
    d = min(d, s);
    
    
    g+=.01/(.01+d*d);
    return d;
}

void main()
{
    vec2 uv = gl_FragCoord.xy/iResolution.xy -.5;
    uv.x*=iResolution.x/iResolution.y;
    
    float dt = iTime * 6.;
    vec3 ro = vec3(0,0, -3. + dt);
    vec3 ta = vec3(0, 0, dt);
    
    ro.xy += path(ro.z);
    ta.xy += path(ta.z);
    
    vec3 fwd = normalize(ta -ro);
    vec3 left = cross(vec3(0,1,0),fwd);
    vec3 up = cross(fwd, left);
    
    vec3 rd = normalize(fwd + left*uv.x+up*uv.y);

    vec3 p;
    float ri,t=0.;
    for(float i=0.;i<1.;i+=.01) {
    	ri = i;
        p=ro+rd*t;
        float d = de(p);
        if(d<.001) break;
        t+=d*.2;
    }
	vec3 bg =  vec3(.2, .6, .6)*.2; 
    vec3 col = mix(vec3(.4, .5, .5), bg,ri);
    col += g*.02;
    
    col = mix(col, bg, 1.-exp(-.01*t*t));

    fragColor = vec4(col,1.0);
}
