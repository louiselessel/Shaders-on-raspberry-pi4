#include std_head_fs.inc
#define iTime unif[1][0]
#define iResolution unif[0]
#define iScale unif[1][1]
#define iMouse unif[3]


#define NUM_LAYERS 13. // 16.
#define ITER 14 // 23
//int ITER = 23;
//float NUM_LAYERS = 16.0;

vec4 tex(vec3 p)
{
    float t = iTime + 78.0;
    vec4 o = vec4(p.x,p.y,p.z,3.0*sin(t*0.1));
    vec4 dec = vec4 (1.0,0.9,0.1,0.15) + vec4(0.06*cos(t*0.1),0.0,0.0,0.14*cos(t*0.23));
    for (int i=0; i < ITER; i++)
	{o.xyzw = 0.0;}
	//{o.xzyw = abs(o / dot (o,o) - dec);}
    return o;
}

void main() {
    float invScale = 1.0 / iScale; // obviously scale must not be zero!
    vec2 offset = vec2(invScale - 1.0) * 0.5;
    
    vec2 uv = (gl_FragCoord.xy-iResolution.xy*0.5)/iResolution.y) * invScale - offset;
    vec3 col = vec3(0.0,0.0,0.0);
    float t = iTime* 0.3;
    
    for(float i=0.0; i<=1.0; i+=1.0/NUM_LAYERS)
    {
        float d = fract(i+t); // depth
        float s = mix(5.0,0.5,d); // scale
        float f = d * smoothstep(1.0,0.9,d); //fade
        col+= tex(vec3(uv*s,i*4.0)).xyz*f;
    }
    
    col/=NUM_LAYERS;
    col*=vec3(2.0,1.0,2.0);
       col=pow(col,vec3(0.5,0.5,0.5));

    gl_FragColor = vec4(col,1.0);
}
