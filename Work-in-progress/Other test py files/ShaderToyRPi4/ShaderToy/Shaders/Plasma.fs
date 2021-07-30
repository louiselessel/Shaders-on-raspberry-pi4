#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;

out vec4 fragColor;

void main()
{
    // normalized pixel coordinates
    vec2 p = 6.0*gl_FragCoord.xy/iResolution.xy;
	
    // pattern
    float f = sin(p.x + sin(2.0*p.y + iTime)) +
              sin(length(p)+iTime) +
              0.5*sin(p.x*2.5+iTime);
    
    // color
    vec3 col = 0.7 + 0.3*cos(f+vec3(0,2.1,4.2));

    // putput to screen
    fragColor = vec4(col,1.0);
}
