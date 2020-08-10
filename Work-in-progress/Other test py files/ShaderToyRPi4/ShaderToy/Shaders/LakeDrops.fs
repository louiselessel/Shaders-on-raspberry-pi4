//Lake Drops
//https://www.shadertoy.com/view/MdjBDh

//Horrible mod to non-cubemap texture!

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform sampler2D iChannel3;

out vec4 o;

void main()
{
	vec2 c = gl_FragCoord.xy;
	vec2 u=c/iResolution.x*2.-1.,a=u-u;
    u/=(u.y+=.8)-1.;
    for(float i=0.;i++<65.;c=u-cos(vec2(919.,154.)*i)*4.+2.,a+=c/dot(c,c)*sin(20.*clamp(length(c)-mod(i*.12+iTime,3.),-.157,.157)))
		o=texture(iChannel1,vec3(a-u*6.,5.).xy)*.7+texture(iChannel0,u)*.4;
}
