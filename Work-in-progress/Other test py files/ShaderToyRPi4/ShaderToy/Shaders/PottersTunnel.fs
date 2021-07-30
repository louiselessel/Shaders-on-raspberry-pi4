//https://www.shadertoy.com/view/4st3WX

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

out vec4 fragColor;

#define PI 3.14159265359

void main()
{
	vec2 uv = gl_FragCoord.xy / iResolution.xy / .5 - 1.;
    uv.x *= iResolution.x / iResolution.y;
    
    // make a tube
    float f = 1. / length(uv);
    
    // add the angle
    f += atan(uv.x, uv.y) / acos(0.);
    
    // let's roll
    f -= iTime;
    
    // make it black and white
    // old version without AA: f = floor(fract(f) * 2.);
    // new version based on Shane's suggestion:
   	f = 1. - clamp(sin(f * PI * 2.) * dot(uv, uv) * iResolution.y / 15. + .5, 0., 1.);
    
    // add the darkness to the end of the tunnel
    f *= sin(length(uv) - .1);
	
    fragColor = vec4(f, f, f, 1.0);
}
