//// [2TC 15] Mystery Mountains.
// David Hoskins.
//https://www.shadertoy.com/view/llsGW7
// Add texture layers of differing frequencies and magnitudes...

#version 300 es
precision highp float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;

uniform sampler2D iChannel0;

out vec4 c;

#define F +texture(iChannel0,.3+p.xz*s/3e3)/(s+=s) 

void main()
{
    vec4 p=vec4(gl_FragCoord.xy/iResolution.xy,1,1)-.5,d=p,t;
    p.z += iTime*20.;d.y-=.4;
    
    for(float i=1.5;i>0.;i-=.002)
    {
        float s=.5;
        t = F F F F F F;
        c =1.+d.x-t*i; c.z-=.1;
        if(t.x>p.y*.007+1.3)break;
        p += d;
    }
}
