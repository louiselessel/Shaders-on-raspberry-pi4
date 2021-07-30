//https://www.shadertoy.com/view/lsfyW4

// Created by Stephane Cuillerdier - Aiekick/2017 (twitter:@aiekick)
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
// Tuned via XShade (http://www.funparadigm.com/xshade/)

#version 300 es
precision highp float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

out vec4 fragColor;

vec2 df(vec3 p)
{
	float z = p.z * .13;
	p.xy *= mat2(cos(z),-sin(z),sin(z),cos(z));
	float mesh = length(cos(p.xz)) - 1.;
	float tri = max(abs(p.x)+p.y,-p.y) - 5.;
	return vec2(length(vec2(mesh,tri)) - 0.15, 0);
}

vec3 nor( vec3 p, float prec )
{
    vec2 e = vec2( prec, 0. );
    vec3 n = vec3(
		df(p+e.xyy).x - df(p-e.xyy).x,
		df(p+e.yxy).x - df(p-e.yxy).x,
		df(p+e.yyx).x - df(p-e.yyx).x );
    return normalize(n);
}

// from Dave Hoskins // https://www.shadertoy.com/view/Xsf3zX
vec3 GetSky(in vec3 rd, in vec3 sunDir, in vec3 sunCol)
{
	float sunAmount = max( dot( rd, sunDir), 0.0 );
	float v = pow(1.0-max(rd.y,0.0),6.);
	vec3  sky = mix(vec3(.1, .2, .3), vec3(.32, .32, .32), v);
	sky = sky + sunCol * sunAmount * sunAmount * .25;
	sky = sky + sunCol * min(pow(sunAmount, 800.0)*1.5, .3);
	return clamp(sky, 0.0, 1.0);
}

float SubDensity(vec3 p, float ms) 
{
	return df(p - nor(p,0.0001) * ms).x/ms;
}

vec2 shade(vec3 ro, vec3 rd, float d, vec3 lp, vec3 ldo, float li)
{
	vec3 p = ro + rd * d;
	vec3 n = nor(p, 0.1);
	vec3 ldp = normalize(lp-n*1.5-p);
	vec3 refl = reflect(rd,n);
	float amb = 0.6;
	float diff = clamp( dot( n, ldp ), 0.0, 1.0);
	float fre = pow( clamp( 1. + dot(n,rd),0.0,1.0), 4.);
	float spe = pow(clamp( dot( refl, ldo ), 0.0, 1.0 ), 16.);
	float sss = 1. - SubDensity(p, 0.1);
	return vec2(
        (diff + fre + spe) * amb * li,
        (diff + fre + sss) * amb * li + spe
    );
}

void main()
{
	vec2 uv = (2.*gl_FragCoord.xy-iResolution.xy)/iResolution.y;
	float t = iTime * 5.;

    vec3 ld = vec3(0.,1., .5);
    
	vec3 ro = vec3(0,0,t);
	vec3 cu = vec3(0,1,0);
	vec3 tg = ro + vec3(0,0,.1);
	
	float fov = .5;
	vec3 z = normalize(tg - ro);
	vec3 x = normalize(cross(cu, z));
	vec3 y = normalize(cross(z, x));
	vec3 rd = normalize(z + fov * (uv.x * x + uv.y * y));
	
	float s = 1., d = 1.;
	float dm = 200.;
	
	for (float i=0.; i<250.; i++)
	{
		if (log(d*d/s/1e5)>0. || d>dm) break;
		d += (s = df(ro + rd * d).x) * .3;
	}
	
    fragColor.rgb = GetSky(rd, ld, vec3(1.5));
    
	if (d<dm)
	{
		vec2 sh = shade(ro, rd, d, ro, ld, 1.);
		fragColor = vec4(mix( 
            vec3(.49,1,.32) * sh.y * .6 + vec3(.45,0,.72) * sh.x * 1.2, 
            fragColor.rgb, 
            1.0 - exp( -0.001*d*d )),1.); 
	}
}

