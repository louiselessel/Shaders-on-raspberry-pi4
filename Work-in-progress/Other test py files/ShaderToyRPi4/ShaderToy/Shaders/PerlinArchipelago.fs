//Perlin Archipelago
//https://www.shadertoy.com/view/MsS3Wz

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;

out vec4 fragColor;

float rand(vec3 co)
{
    return -1.0 + fract(sin(dot(co.xy,vec2(12.9898 + co.z,78.233))) * 43758.5453) * 2.0;
}

float linearRand(vec3 uv)
{
	vec3 iuv = floor(uv);
	vec3 fuv = fract(uv);
	
	float v1 = rand(iuv + vec3(0,0,0));
	float v2 = rand(iuv + vec3(1,0,0));
	float v3 = rand(iuv + vec3(0,1,0));
	float v4 = rand(iuv + vec3(1,1,0));
	
	float d1 = rand(iuv + vec3(0,0,1));
	float d2 = rand(iuv + vec3(1,0,1));
	float d3 = rand(iuv + vec3(0,1,1));
	float d4 = rand(iuv + vec3(1,1,1));
	
	return mix(mix(mix(v1,v2,fuv.x),mix(v3,v4,fuv.x),fuv.y),
		       mix(mix(d1,d2,fuv.x),mix(d3,d4,fuv.x),fuv.y),
			   fuv.z);
}

float textureNoise(vec3 uv)
{
	float c = (linearRand(uv * 1.0) * 32.0 +
			   linearRand(uv * 2.0) * 16.0 + 
			   linearRand(uv * 4.0) * 8.0 + 
			   linearRand(uv * 8.0) * 4.0 + 
			   linearRand(uv * 16.0) * 2.0 + 
			   linearRand(uv * 32.0) * 1.0) / 32.0;
	return c * 0.5 + 0.5;
}

void main()
{
	vec2 uv = gl_FragCoord.xy / iResolution.xy;
	uv = uv * 2.0 - 1.0;
	uv.x *= iResolution.x / iResolution.y;
	
	float time = iTime + 2.49;
		
	float wave = time * 2.0;
	float waveAnim = wave;//floor(wave) + 0.5 + 0.5 * sin(-3.1415 * 0.5 + fract(wave) * 3.1415);
	float water = textureNoise(vec3(uv * 16.0 + vec2(0,time * 4.0), waveAnim));
	
	float cloud = textureNoise(vec3(uv * 1.0 + vec2(0,time * 8.0), iTime));
	
	float islands = textureNoise(vec3(uv * 2.0 + vec2(0,time * 0.5), 0.0));
	float islandsMix = 1.0;
	vec4 islandsCol = vec4(0.5,0.3,0.1,1);
	if (islands < 0.1 + cos(time) * 0.01) {
		islandsMix = clamp(islands / 0.1, 0.0, 1.0) * 0.3;
		islandsCol = mix(vec4(0.1,0.4,0.05,1),vec4(0.5,0.3,0.1,1),islands / (0.1 + cos(time) * 0.01));
	} else if (islands < 0.3) {
		islandsMix = islands / 0.3;
	}
	
	fragColor = 	mix(vec4(1,1,1,1),
					   	mix(islandsCol,
					   		mix(vec4(0,0,0.6,1),vec4(0,0.1,0.7,1),water),
						islandsMix),
					cloud);
}
