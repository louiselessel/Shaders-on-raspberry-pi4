#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform sampler2D iChannel3;

out vec4 fragColor;

//https://www.shadertoy.com/view/4t33z8

void main()
{
	vec2 uv = gl_FragCoord.xy / iResolution.xy;
    vec2 warpUV = 2. * uv;

	float d = length( warpUV );
	vec2 st = warpUV*0.1 + 0.2*vec2(cos(0.071*iTime*2.+d),
								sin(0.073*iTime*2.-d));

    vec3 warpedCol = texture( iChannel3, st ).xyz * 2.0;
    float w = max( warpedCol.r, 0.85);
	
    vec2 offset = 0.01 * cos( warpedCol.rg * 3.14159 );
    vec3 col = texture( iChannel0, uv + offset ).rgb * vec3(0.8, 0.8, 1.5) ;
	col *= w*1.2;
	
	fragColor = vec4( mix(col, texture( iChannel0, uv + offset ).rgb, 0.5),  1.0);
}
