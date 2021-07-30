//https://www.shadertoy.com/view/3lXSRl

#version 300 es
precision highp float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

out vec4 fragColor;

void main()
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = gl_FragCoord.xy/iResolution.xy;

    uv -= 0.5;
    uv *= 12.5;
    
    float t = iTime+25.;
    uv -= t*4.45;
    // Time varying pixel color
    vec3 col;
    for (int i=0; i<9; i++) {
        t *= 1.13879213724+sin(col.r+col.g+col.b)*0.0052863;
	    col.r += sin(uv.x*0.4+t);
	    col.g += cos(uv.y*0.4+t*1.001379);
        col.rgb = col.gbr;
    }

    col.rgb = clamp(col.rgb/9.0+0.5,0.0,1.0);
    // Output to screen
    fragColor = vec4(pow(col,vec3(2.0/1.0)),1.0);
}
