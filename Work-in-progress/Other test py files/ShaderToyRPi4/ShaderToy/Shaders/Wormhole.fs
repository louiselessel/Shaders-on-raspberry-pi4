//https://www.shadertoy.com/view/4ll3zX
#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;

out vec4 fragColor;

float rand(vec2 n) { 
	return fract(sin(dot(n, vec2(12.9898, 4.1414))) * 43758.5453);
}

float noise(vec2 n) {
	const vec2 d = vec2(0.0, 1.0);
	vec2 b = floor(n), f = smoothstep(vec2(0.0), vec2(1.0), fract(n));
	return mix(mix(rand(b), rand(b + d.yx), f.x), mix(rand(b + d.xy), rand(b + d.yy), f.x), f.y);
}

float fbm(vec2 n) {
	float total = 0.0, amplitude = 0.5;
    
	for (int i = 0; i <4; i++) {
        total += noise(n ) * amplitude;
		n += n;
		amplitude *= 0.5;
	}
	return total;
}


vec2 field(vec2 uv) {
    vec2 dir = uv-vec2(0.5, 0.5);
    vec2 dir2 = vec2(dir.y, -dir.x);
    float l = length(dir);
    vec2 a0 = l > 0.3 ? dir/(0.1+(l-0.3)*20.) : dir2*5.;
    return a0*3. + dir2*l*0.9;
}


float getColor(vec2 uv) {
    return fbm(uv*5.);   
}

vec2 calcNext(vec2 uv, float t) {
    vec2 k1 = field(uv);
    vec2 k2 = field(uv + k1*t/2.);
    vec2 k3 = field(uv + k2*t/2.);
    vec2 k4 = field(uv + k3*t);
       
    return uv + t/6.*(k1+2.*k2+2.*k2+k3);
}

vec4 getColor(vec2 uv, float cf, float per) {
    float t1 = per * cf;
    float t2 = t1 + per;
    
    float k1 = 0.4;
    float k2 = 0.4;
    
    vec2 uv1 = calcNext(uv, t1 * k1 + k2);
    vec2 uv2 = calcNext(uv, t2 * k1 + k2);
    
    float c1 = getColor(uv1);
    float c2 = getColor(uv2);
    
    return vec4(mix(c2, c1, cf));
}


void main()
{
    vec2 uv = (gl_FragCoord.xy + (iResolution.yx - iResolution.xx)/2.)/iResolution.y;

    float per = 2.;
    
    float cf = fract(iTime / per);
    vec4 c = getColor(uv,cf, per);
    float l = length(field(uv));
    c =  1. - (abs(c-0.5)*5.);
    
    
    // some empirical coefficients
    c *= 0.5*vec4(0.3, 0.6, 1.1, 1.)*pow(l,-0.3);
    c += 0.25*vec4(1.0, 0.6, 0.2, 1.)*pow(abs(l-0.35),-0.7);
    c*=0.8;

	fragColor = c;
}
