//https://www.shadertoy.com/view/XlfSWS

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

out vec4 fragColor;

#define numOfPlanets 3
#define scale 5.0

vec3 planet[numOfPlanets];

void main()
{
    vec2 center = iResolution.xy * 0.5;
    planet[0] = vec3(  center.x,   center.y,   400);
    planet[1] = vec3(center.x + sin(iTime) * 70.0, center.y + cos(iTime) * 70.0, 50);
    planet[2] = vec3(center.x - sin(iTime * 1.3) * 120.0, center.y + cos(iTime * 1.3) * 120.0, 100);
	vec2 uv = gl_FragCoord.xy;
    vec2 res = vec2(0,0);
    for (int i=0;i<numOfPlanets;i++) {
        vec2 dist = uv - vec2(planet[i]);
        res += normalize(dist) * (scale * planet[i].z) / (length(dist) * length(dist));
    }
    res.x = abs(res.x);
    res.y = abs(res.y);
	fragColor = vec4(length(res),length(res),length(res),1.0);
}
