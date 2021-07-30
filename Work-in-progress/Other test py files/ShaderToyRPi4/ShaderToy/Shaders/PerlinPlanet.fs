//https://www.shadertoy.com/view/MdX3RM

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

out vec4 fragColor;

uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform sampler2D iChannel3;

vec2 noiseOffset = vec2(0.0, 0.0);
vec2 noiseScale = vec2(16.0, 16.0);
vec2 noiseScale2 = vec2(200.0, 200.0);
vec2 noiseScale3 = vec2(50.0, 50.0);
vec2 cloudNoise = vec2(10.0, 30.0);

float cloudiness = 0.5;

vec3 ocean = vec3(13.0 / 255.0, 55.0 / 255.0, 79.0 / 255.0);
vec3 ice = vec3(250.0 / 255.0, 250.0 / 255.0, 250.0 / 255.0);
vec3 cold = vec3(53.0 / 255.0, 102.0 / 255.0, 100.0 / 255.0);
vec3 temperate = vec3(79.0 / 255.0, 109.0 / 255.0, 68.0 / 255.0);
vec3 warm = vec3(119.0 / 255.0, 141.0 / 255.0, 82.0 / 255.0);
vec3 hot = vec3(223.0 / 255.0, 193.0 / 255.0, 148.0 / 255.0);

//
// GLSL textureless classic 2D noise "cnoise",
// with an RSL-style periodic variant "pnoise".
// Author:  Stefan Gustavson (stefan.gustavson@liu.se)
// Version: 2011-08-22
//
// Many thanks to Ian McEwan of Ashima Arts for the
// ideas for permutation and gradient selection.
//
// Copyright (c) 2011 Stefan Gustavson. All rights reserved.
// Distributed under the MIT license. See LICENSE file.
// https://github.com/ashima/webgl-noise
//

vec4 mod289(vec4 x)
{
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec4 permute(vec4 x)
{
  return mod289(((x*34.0)+1.0)*x);
}

vec4 taylorInvSqrt(vec4 r)
{
  return 1.79284291400159 - 0.85373472095314 * r;
}

vec2 fade(vec2 t) {
  return t*t*t*(t*(t*6.0-15.0)+10.0);
}

// Classic Perlin noise, periodic variant
float pnoise(vec2 P, vec2 rep)
{
  vec4 Pi = floor(P.xyxy) + vec4(0.0, 0.0, 1.0, 1.0);
  vec4 Pf = fract(P.xyxy) - vec4(0.0, 0.0, 1.0, 1.0);
  Pi = mod(Pi, rep.xyxy); // To create noise with explicit period
  Pi = mod289(Pi);        // To avoid truncation effects in permutation
  vec4 ix = Pi.xzxz;
  vec4 iy = Pi.yyww;
  vec4 fx = Pf.xzxz;
  vec4 fy = Pf.yyww;

  vec4 i = permute(permute(ix) + iy);

  vec4 gx = fract(i * (1.0 / 41.0)) * 2.0 - 1.0 ;
  vec4 gy = abs(gx) - 0.5 ;
  vec4 tx = floor(gx + 0.5);
  gx = gx - tx;

  vec2 g00 = vec2(gx.x,gy.x);
  vec2 g10 = vec2(gx.y,gy.y);
  vec2 g01 = vec2(gx.z,gy.z);
  vec2 g11 = vec2(gx.w,gy.w);

  vec4 norm = taylorInvSqrt(vec4(dot(g00, g00), dot(g01, g01), dot(g10, g10), dot(g11, g11)));
  g00 *= norm.x;  
  g01 *= norm.y;  
  g10 *= norm.z;  
  g11 *= norm.w;  

  float n00 = dot(g00, vec2(fx.x, fy.x));
  float n10 = dot(g10, vec2(fx.y, fy.y));
  float n01 = dot(g01, vec2(fx.z, fy.z));
  float n11 = dot(g11, vec2(fx.w, fy.w));

  vec2 fade_xy = fade(Pf.xy);
  vec2 n_x = mix(vec2(n00, n01), vec2(n10, n11), fade_xy.x);
  float n_xy = mix(n_x.x, n_x.y, fade_xy.y);
  return 2.3 * n_xy;
}

float spow(float x, float y) { float s = sign(x); return s * pow(s * x, y); }

vec4 planet(in vec2 pix, in float rotspeed, in float rot, in float light, in float zLight, in float time) 
{
    vec2 p = -1.0 + 2.0 * pix;
    p.x *= iResolution.x / iResolution.y;
    p = mat2(cos(rot), sin(rot), -sin(rot), cos(rot)) * p;
	
    vec3 ro = vec3( 0.0, 0.0, 2.25 );
    vec3 rd = normalize( vec3( p, -2.0 ) );

    vec3 col = vec3(0.0);

    // intersect sphere
    float b = dot(ro,rd);
    float c = dot(ro,ro) - 1.0;
    float h = b*b - c;
	float t = -b - sqrt(h);
	vec3 pos = ro + t*rd;
	vec3 nor = pos;

	// texture mapping
	vec2 uv;
	uv.x = atan(nor.x,nor.z)/6.2831 + rotspeed*time;
	uv.y = acos(nor.y)/3.1416;
	uv.y = 0.5 + spow(uv.y - 0.5, 1.2);
	uv += noiseOffset;
	
	float n2 = pnoise(uv * noiseScale2, noiseScale2) * 0.05;
	float n = pnoise(uv * noiseScale, noiseScale) + n2;
	
	float temp = cos(nor.y * 4.0) + pnoise(uv * noiseScale3, noiseScale3) * 0.8 + n * 0.5;
	
	float oceanity = min(1.0, 1.0 - smoothstep(0.19, 0.2, n) + 1.0 - smoothstep(0.05, 0.08, mod(temp - uv.x * 35.0 + 0.3, 1.0) + n * n * 0.35));
			
	float iceity = max(0.0, 1.0 - oceanity - smoothstep(-0.8, -0.6, temp));
	float coldity = max(0.0, 1.0 - iceity - oceanity - smoothstep(-0.4, 0.0, temp));
	float temperateity = max(0.0, 1.0 - iceity - coldity - oceanity - smoothstep(0.3, 0.8, temp));
	float warmity = max(0.0, 1.0 - iceity - coldity - temperateity - oceanity - smoothstep(1.05, 1.3, temp));
	float hottity = max(0.0, 1.0 - oceanity - iceity - coldity - temperateity - warmity);
	
	col = ocean * oceanity + ice * iceity + cold * coldity + temperate * temperateity + warm * warmity + hot * hottity;
	
	col *= (0.7 + abs(temp + n * 0.2) * 0.3);
	col *= 0.92 + step(0.1, mod(n2, 0.4)) * 0.08;
	col *= 1.0 + step(0.39, mod(n + uv.x, 0.4)) * 0.1;
	
	float cloudN = max(0.0, pnoise((uv + vec2(rotspeed * time, 0)) * cloudNoise, cloudNoise) + cloudiness + n2);
	col *= 0.7;
	col += vec3(cloudN, cloudN, cloudN) * 0.5;

    float lighting = max(sin(light) * nor.y * 2.0 + cos(light) * nor.x * 2.0 + nor.z * zLight,0.0);
	col *= 0.2 + lighting * 0.7;

    return vec4(mix(vec3(0.0), col, step(0.0, h)), 1.0);
}

void main()
{
    float time = iTime * 0.1;
    float left = iResolution.x * 0.3;
    float top = iResolution.y * 0.3;
    vec2 resolution = iResolution.xy * 1.6;
    
	vec3 coord = vec3(gl_FragCoord.xy,0.);
	coord.x += left;
	coord.y += top;

	fragColor = planet(coord.xy / resolution.xy, 0.05, float(int(time * 0.08)), time * 0.1, sin(time * 0.05) * 2.0, time), 1.0;
}
