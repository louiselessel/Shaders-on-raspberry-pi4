#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

out vec4 fragColor;

// a variant from jt's https://www.shadertoy.com/view/ldtGD8

// -5 ( 146 ) by coyote

void main() {
	vec2 o = iResolution.xy/2.;
    vec2 I = gl_FragCoord.xy-o;
    fragColor = 1. - vec4(.5,1,9,0) *
        ( sin(atan(I.y,I.x)/.1) * sin(20.*(o.y/=length(I))+iTime*30.) - 1. + o.y );
}
