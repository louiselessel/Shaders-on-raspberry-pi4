#include std_head_fs.inc
#define iTime unif[1][0]
#define iResolution unif[0]
#define iScale unif[1][1]
#define iMouse unif[3]
 
// Created by Benoit Marini - 2020
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

// Try it fullscreen ;)
// Try NUM_LAYERS 80. if you GPU can

#define NUM_LAYERS 16.0
#define ITER 4

vec4 tex(vec3 p)
{
    float t = iTime+78.;
    vec4 o = vec4(p.xyz,3.*sin(t*.1));
    vec4 dec = vec4 (1.,.9,.1,.15) + vec4(.06*cos(t*.1),0,0,.14*cos(t*.23));
    for (int i=0; i < ITER;  i++) {
	o.xzyw = abs(o/dot(o,o)- dec);
    }
    return o;
}

void main()
{
    float invScale = 1.0 / iScale; // obviously scale must not be zero!
    vec2 offset = vec2(invScale - 1.0) * 0.5;

    vec2 uv = ((gl_FragCoord-iResolution.xy*.5)/iResolution.y) * invScale - offset;
    vec3 col = vec3(0);   
    float t = iTime * .3;
    
    for(float i=0.; i<=1.; i+=1./NUM_LAYERS)
    {
        float d = fract(i+t); // depth
        float s = mix(5.,.5,d); // scale
        float f = d * smoothstep(1.,.9,d); //fade
        col+= tex(vec3(uv*s,i*4.)).xyz*f;
    }
    
    col/=NUM_LAYERS;
    col*=vec3(2,1.,2.);
   	col=pow(col,vec3(.5 ));  

    gl_FragColor = vec4(col,1.0);
}
 
