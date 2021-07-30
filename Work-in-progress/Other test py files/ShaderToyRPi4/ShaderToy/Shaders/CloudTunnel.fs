//https://www.shadertoy.com/view/4d3yRH
#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform sampler2D iChannel0;
out vec4 fragColor;

#define xDiv 2.
//1. / 2. else it fails
#define yDiv 2.

void main(){
    vec2 uv=gl_FragCoord.xy/iResolution.xy-0.5;
    uv.x*=iResolution.x/iResolution.y;
    vec2 t=vec2(atan(uv.x,uv.y)/3.1415926,1./length(uv));
    t.x/=(xDiv);
    t.y/=(yDiv);
    float o = t.y+0.5;
    t.y+=iTime;
    
    float s=sin(t.y*3.1415926*(yDiv));
	fragColor=mix(
        (1.-texture(iChannel0,vec2(t.x,t.y)))/o,
        (1.-texture(iChannel0,vec2(t.x+0.5/(xDiv),t.y)))/o,
        smoothstep(-0.5/(xDiv),0.,-abs(t.x))+smoothstep(0.5/(xDiv),1./(xDiv),abs(t.x))
    );
}
