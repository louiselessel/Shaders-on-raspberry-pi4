//https://www.shadertoy.com/view/ld23Rw

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform sampler2D iChannel0;
out vec4 fragColor;

void main()
{
	float time = iTime;
    vec2 p = -1.0 + 2.0 * gl_FragCoord.xy / iResolution.xy;
    vec2 uv;
	p.x += 0.5*sin(time*0.71);
	p.y += 0.5*cos(time*1.64);
    float r = sqrt( dot(p,p) );
    float a = time + atan(p.y,p.x) + 0.1*sin(r*(5.0+2.0*sin(time/4.0))) + 5.0*cos(time/7.0);
    float s = smoothstep( 0.0, 0.7, 0.5+0.4*cos(7.0*a)*sin(time/3.0) );
    uv.x = time + 0.9/( r + .2*s );
    uv.y = -time + sin(time/2.575) + 3.0*a/3.1416;
    float w = (0.5 + 0.5*s)*r*r;
    vec3 col = texture(iChannel0,uv).xyz;
	col.x *= 1.0+0.5*sin(0.5*time);
	col.y *= 1.0+0.5*cos(0.7*time);
	col.z *= 1.0+0.5*sin(1.1*time+1.5);
    fragColor = vec4(col*w,1.0);
}
