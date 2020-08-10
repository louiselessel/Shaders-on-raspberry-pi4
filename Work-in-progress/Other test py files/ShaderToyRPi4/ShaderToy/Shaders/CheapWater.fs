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

#define PI 			3.14159265359

#define FOV 		60.0
#define EPSILON		0.001
#define MAX_STEPS	1000
#define MAX_DIST	1000.0

#define PLANE		vec4 (0.0, 1.0, 0.0, 1.0)
#define BOTTOM		vec4 (0.0, 1.0, 0.0, 4.0)

mat3 rotate_x (float fi) {
	float cfi = cos (fi);
	float sfi = sin (fi);
	return mat3 (
		1.0, 0.0, 0.0,
		0.0, cfi, -sfi,
		0.0, sfi, cfi);
}

mat3 rotate_y (float fi) {
	float cfi = cos (fi);
	float sfi = sin (fi);
	return mat3 (
		cfi, 0.0, sfi,
		0.0, 1.0, 0.0,
		-sfi, 0.0, cfi);
}

mat3 rotate_z (float fi) {
	float cfi = cos (fi);
	float sfi = sin (fi);
	return mat3 (
		cfi, -sfi, 0.0,
		sfi, cfi, 0.0,
		0.0, 0.0, 1.0);
}

vec4 noise3v (vec2 p) {
	return texture (iChannel3, p);
}

vec4 fbm3v (vec2 p) {
	vec4 f = vec4 (0.0);
	f += (vec4 (0.5000) * noise3v (p)); p *= vec2 (2.01);
	f += (vec4 (0.2500) * noise3v (p)); p *= vec2 (2.02);
	f += (vec4 (0.1250) * noise3v (p)); p *= vec2 (2.03);
	f += (vec4 (0.0625) * noise3v (p)); p *= vec2 (2.04);
	f /= vec4 (0.9375);
	return f;
}

float dplane (vec3 pt, vec4 pl) {
	return dot (pl.xyz, pt) + pl.w;
}	

float map (vec3 pt) {
	return dplane (pt, PLANE);
}

float map2 (vec3 pt) {
	return dplane (pt, BOTTOM);
}

float march (vec3 ro, vec3 rd) {
	float t = 0.0;
	float d = 0.0;
	vec3 pt = vec3 (0.0);
	for (int i = 0; i < MAX_STEPS; ++i) {
		pt = ro + rd * t;
		d = map (pt); 
		if (d < EPSILON || t + d >= MAX_DIST) {			
			break;
		}
		t += d;
	}
	
	return d <= EPSILON ? t : MAX_DIST;
}

float march2 (vec3 ro, vec3 rd) {
	float t = 0.0;
	float d = 0.0;
	vec3 pt = vec3 (0.0);
	for (int i = 0; i < MAX_STEPS; ++i) {
		pt = ro + rd * t;
		d = map2 (pt); 
		if (d < EPSILON || t + d >= MAX_DIST) {			
			break;
		}
		t += d;
	}
	
	return d <= EPSILON ? t : MAX_DIST;
}

float fresnel_step (vec3 I, vec3 N, vec3 f) {
	return clamp (f.x + f.y * pow (1.0 + dot (I, N), f.z), 0.0, 1.0);
}


void main() {
	vec2 uv = (2.0*gl_FragCoord.xy - iResolution.xy)/min (iResolution.x, iResolution.y) * tan (radians (FOV)/2.0);
	vec2 mo = PI * iMouse.xy / iResolution.xy;
	
	mat3 rt = rotate_y (mo.x * PI);
	
	vec3 up = vec3 (0.0, 1.0, 0.0) ;			// up 
	vec3 fw = vec3 (0.0, 0.0, 1.0) *rt;			// forward
	vec3 lf = cross (up, fw); 					// left
	
	vec3 ro = -fw * 5.0 + vec3 (0.0, 5.0, 0.0); // ray origin
	vec3 rd = normalize (uv.x * lf + uv.y * up + fw) ; 		// ray direction
	
	float t = march (ro, rd);
	vec4 cm = texture (iChannel0, rd.xy);
	vec3 pt = rd*t + ro;
	vec3 pn = PLANE.xyz;
	vec3 dv = fbm3v (pt.xz/512.0+iTime/512.0).xyz-0.5;
	pn = normalize (pn + dv*0.2);
	vec3 rfl = reflect (rd, pn);
	float fs = fresnel_step (rd, pn, vec3 (0.0, 3.0, 6.0));
	vec4 c0 = texture (iChannel0, rfl.xy);
	vec4 c1 = texture (iChannel0, normalize (rd+dv*0.1).xy);

	fragColor = mix (mix (
		c0,
		c1,			
		1.0 - fs), cm, smoothstep (80.0, 160.0, t));
	
}		
