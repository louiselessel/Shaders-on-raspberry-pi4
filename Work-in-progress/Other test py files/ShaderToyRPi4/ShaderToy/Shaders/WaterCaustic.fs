// A simple, if a little square, water caustic effect.
// David Hoskins.
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
//https://www.shadertoy.com/view/MdKXDm

// Inspired by akohdr's "Fluid Fields"
// https://www.shadertoy.com/view/XsVSDm

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;

out vec4 k;

#define F length(.5-fract(k.xyw*=mat3(-2,-1,2, 3,-2,1, 1,2,2)*

void main()
{
    k.xy = gl_FragCoord.xy*(sin(k=vec4(iTime,iTime,iTime,iTime)*.5).w+2.)/2e2;
    k = pow(min(min(F.5)),F.4))),F.3))), 7.)*25.+vec4(0,.35,.5,1);
}
