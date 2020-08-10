// Created by sebastien durand - 01/2014
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

// ------------------------------------------------------------------------------
// Deep Space 
// Star Nest by Kali
// https://www.shadertoy.com/view/4dfGDM
// ------------------------------------------------------------------------------
// Metal effect
// Exterminate! by Antonalog
// https://www.shadertoy.com/view/ldX3RX
// Blue Spiral by donfabio
// ------------------------------------------------------------------------------
// Spiral effect
// BLue Spiral by donfabio
// https://www.shadertoy.com/view/lds3WB

//https://www.shadertoy.com/view/MsjGRt

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


//#define ANTIALIASING
// Anti-Aliasing Level
#define AA 3


#define NB_ITER 64
#define PI 3.14159265359
#define TAO 6.28318531
#define MAX_DIST 4000.
#define PRECISION .0001

#define PLANET 200.
#define SHIP_GLOB 103.
#define SHIP_HUBLOT 104.
#define SHIP_TOP 500.
#define SHIP_BOTTOM 501.
#define SHIP_SIDE 502.
#define SHIP_ARM 505.
#define FLAG 300.

#define COS cos
#define SIN sin
#define ATAN atan


const vec2 V01 = vec2(0,1);
const vec2 Ve = V01.yx*.001;
vec3 L = normalize(vec3(10.25,.33,-.7));

const mat2 Rot1 = mat2(0.54030230586, 0.8414709848, -0.8414709848, 0.54030230586);

float C1,S1, C2, S2, time; 

int AnimStep = 0;
bool withPlanet = true;

// 0.1% error - enough for animations
float sin_(in float x) {
	x = mod(PI+x,2.*PI) - PI;
	float s = x*(1.27323954 - .4052847345*abs(x));
	return s*(.776 + .224*abs(s));
}

// 0.1% error - enough for animations
float cos_(in float x) {
	return sin_(x+PI*.5);
}
/*
float atan2_(float y, float x) {
  float t0, t1, t2, t3, t4;
  t3 = abs(x);
  t1 = abs(y);
  t0 = max(t3, t1);
  t1 = min(t3, t1);
  t3 = float(1) / t0;
  t3 = t1 * t3;
  t4 = t3 * t3;
  t0 =         - .013480470;
  t0 = t0 * t4 + .057477314;
  t0 = t0 * t4 - .121239071;
  t0 = t0 * t4 + .195635925;
  t0 = t0 * t4 - .332994597;
  t0 = t0 * t4 + .999995630;
  t3 = t0 * t3;
  t3 = (abs(y) > abs(x)) ? 1.570796327 - t3 : t3;
  t3 = (x < 0.) ?  3.141592654 - t3 : t3;
  return (y < 0.) ? -t3 : t3;;
}
*/

// k : [0..1]
float steps(in float x, in float k) {
	float fr = fract(x);
	return floor(x)+(fr<k?0.:(fr-k)/(1.-k));
}

float pyramid(in float x) {
	float fr = fract(x*.5+1./16.);
	return clamp(4.*min(fr,1.-fr)-1.,-.75,.75);
}

float Noise(in vec3 x) {
    vec3 p = floor(x);
    vec3 f = fract(x);
	f = f*f*(3.0-2.0*f);
	
	vec2 uv = (p.xy+vec2(37.0,17.0)*p.z) + f.xy;
	vec2 rg = textureLod( iChannel0, (uv+ 0.5)/256.0, -100.0 ).yx;
	return mix( rg.x, rg.y, f.z );
}

float Noise( in vec2 x ) {
    vec2 p = floor(x);
    vec2 f = fract(x);
	vec2 uv = p.xy + f.xy*f.xy*(3.0-2.0*f.xy);
	return textureLod( iChannel0, (uv+118.4)/256.0, -100.0 ).x;
}

float smin( float a, float b, float k ) {
    float h = clamp( 0.5+0.5*(b-a)/k, 0.0, 1.0 );
    return mix(b, a, h ) - k*h*(1.0-h);
}

float Kaleido(inout vec2 v, in float nb){
	float id=floor(.5+ATAN(v.x,-v.y)*nb/TAO);
	float a = id*TAO/nb;
	float ca = COS(a), sa = SIN(a);
	v*=mat2(ca,sa,-sa,ca);
	return id;
}

vec2 Kaleido2(inout vec3 p, in float nb1, in float nb2, in float d) {
	float id1 = Kaleido(p.yx, nb1);
	float id2 = Kaleido(p.xz, nb2*2.);
	p.z+=d;	
	return vec2(id1,id2);
}

vec2 minObj(vec2 o1, vec2 o2) {
	return o1.x<o2.x?o1:o2;
}

vec2 sminObj(vec2 o1, vec2 o2, float k) {
	float d = smin(o1.x, o2.x, k);
	return vec2(d, o1.x<o2.x?o1.y:o2.y);
}

vec2 maxObj(vec2 o1, vec2 o2) {
	return o1.x>o2.x?o1:o2;
}

const vec3 
	COLOR_GLOBE1 = vec3(.1,.1,.1),
	COLOR_GLOBE2 = vec3(.1,2.,2.),
	COLOR_HUBLOT = vec3(.2,.2,.2),
	COLOR_SIDE = vec3(.0,9.0,9.0);
	
// ------------------------------------------------------------------------------
// Spiral texture
// Blue Spiral by donfabio
// https://www.shadertoy.com/view/lds3WB
float textureSpiral(vec2 uv) {
	float angle = ATAN(uv.y, uv.x),
	shear = length(uv),
	blur = 0.5;
	return smoothstep(-blur, blur, cos_(8.0 * angle + 200.0 * time - 12.0 * shear));
}


float sdCapsule( vec3 p, vec3 a, vec3 b, float r1, float r2) {
    vec3 pa = p - a, ba = b - a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return length( pa - ba*h ) - mix(r1,r2,h);
}

float textureInvader(vec2 uv) {
	float y = 7.-floor((uv.y)*16.+4.);
	if(y < 0. || y > 7.) return 0.;
	float x = floor((abs(uv.x))*16.);
//	if(x < 0. || x > 14.) return 0.;
	float v=(y>6.5)? 6.:(y>5.5)? 40.:(y>4.5)? 47.:(y>3.5)? 63.:
			(y>2.5)? 27.:(y>1.5)? 15.:(y>0.5)? 4.: 8.;
	return floor(mod(v/pow(2.,x), 2.0)) == 0. ? 0.: 1.;
}

vec4 DEFlag(vec3 p) {
    vec3 ba = vec3(1.5,0,0);
    float h = clamp( dot(p,ba)/dot(ba,ba), 0.0, 1.0 );
    vec2 d = vec2(length( p - ba*h ) - .02, 1.);
	p.y -= .4;
	p.x -= 1.2;
	float box = length(max(abs(p)-vec3(.3,.4,.005),0.));
	d = min(vec2(box, FLAG), d);
	return vec4(d.x, FLAG, p.y, p.x);
}


float DEAlienArm(vec3 p0) {
	vec3 p = p0;
	p.x = -p.x;
	float d = MAX_DIST;
	float dy, dx = abs(C1);
	dx = clamp(dx,0.,.8);
	dy = .5*sqrt(1.-dx*dx);
	p.x-=dx;
	float x = dx;
	p = abs(p);
	d = min(d, sdCapsule(p, vec3(x-dx,0,-dy), vec3(x, 0,dy),.01,.01));
	d = min(d, sdCapsule(p, vec3(x,.04,-dy), vec3(x-dx,.04,dy),.01,.01));
	d = min(d, length(p.xz+vec2(x-dx,-dy))-.05);
	d = min(d, length(p.xz+vec2(-x,-dy))-.05);
	x+=dx;
	d = min(d, sdCapsule(p, vec3(x-dx,0,-dy), vec3(x-dx*.5, 0,0),.01,.01));
	d = min(d, sdCapsule(p, vec3(x-dx*.5,.04,.04), vec3(x-dx,.04,dy),.01,.01));
	d = max(p.y-.06,max(-p.y+.005,d));
	return d;
}


vec4 DEAlien(vec3 p0) {
	vec3 p=p0;
	vec2 d = minObj(vec2(length(p+vec3(-.125,0,0))-.75, SHIP_GLOB),
					vec2(length(p+vec3(1.6,0,0))-2., SHIP_TOP));
	p.yz = -abs(p.yz);
	p.yz+= .7;
	d = minObj(vec2(length(p)-.24, SHIP_GLOB),d);
	d = maxObj(vec2(length(p0-vec3(1.6,0,0))-2.,SHIP_BOTTOM),d);
	p.x+= .1;
	p.yz = -abs(p.yz);
	p.y+= .3;
	p.z+=.3;
	d = maxObj(vec2(length(p0)-1.15, SHIP_SIDE), sminObj(vec2(length(p)-.2,SHIP_BOTTOM), d, .1));

	float r=0.;
	if (AnimStep >= 6) {
		r = 0.; //.05*(1.-clamp(0.,1.,time-7.25));
	} else if (AnimStep >= 3) { // ouverture du panneau
		r = .05*clamp(0.,1.,time-5.25);
	}
	if (r>0.) {
		float dd = length(max(abs(p0+vec3(.35,0,0))-vec3(r,r,9.*r),0.))-r;
		d = maxObj(vec2(-dd,SHIP_SIDE), d); 
	}
	if (AnimStep == 4 || AnimStep == 5) {
		p0.x += .2;
		d = minObj(vec2(DEAlienArm(p0), SHIP_ARM), d);
	}
	
	return vec4(d.x,d.y, p0.yz);
}

float DECrater(vec3 p) {
	float d = MAX_DIST;
	vec2 id = Kaleido2(p, 9.,6.,2.);
	float noise = Noise(id*10.);
	if (noise<.6 && abs(id.y)>0.&&abs(id.y)<6.-1.) {  
		d = sdCapsule(p, vec3(0,0,-.15), vec3(0,0,.1),.1+noise*.2,.1+noise*.5);
		d = max(-(length(p-vec3(0,0,-.25))-(.1+noise*.5)),d);
		d = max(-(length(p-vec3(0,0,-.05))-(.1+noise*.15)),d);
		d*=.8;
	}
	return d;
}

bool intersectSphere(in vec3 ro, in vec3 rd, in vec3 c, in float r) {
    ro -= c;
	float b = dot(rd,ro), d = b*b - dot(ro,ro) + r*r;
	return (d>0. && -sqrt(d)-b > 0.);
}

// vec4 : distance / id (object) / uv (texture) 
vec4 DE(vec3 p0) {
	float scalePlanet = 10.,
		  scaleFlag = 2.,
		  scaleAlien = .5;
	vec4 res = vec4(1000);	
	vec3 p = p0;
    float d,d1,dx;

//    if (withPlanet) {
	p = p0;
	p.x+=2.;
	p*=scalePlanet;
	p.yz *= Rot1;
	p.xz *= mat2(C2,S2,-S2,C2);
    if (withPlanet) {    
	d1 = DECrater(p);
// Much better but cannot be render with the full scene in my computer
//	p.xz *= Rot1;
//	p.xy *= Rot1;
//	float d2 = DECrater(p);
	d = smin(length(p)-2.,d1,.15); //smin(d2, d1,.2),.15);

	d += .1*Noise((p)*2.);
	d += .005*Noise((p)*20.);
	res = vec4(d/=scalePlanet,PLANET, length(p), p.z);
	
    }
    
	if (AnimStep >= 4) {
		dx = abs(C1);
		dx = clamp(dx,.1,.8);
		
		if (AnimStep == 4) {
			p = p0;
			p.x += (2.5*dx/scaleAlien) - 2.1;
		} else {
			p /= scalePlanet;
			//p.x-=1.;
		}
		p = p*scaleFlag;
		vec4 dFlag = DEFlag(p);
		dFlag.x /= scaleFlag;
		res = (dFlag.x<res.x) ? dFlag: res;
	}
	
	if (AnimStep > 1 && AnimStep < 7) {
		p = p0;
		if (AnimStep < 3) {
			p.x -= 3.2-steps(10.*(.038+time-5.25),.75);
			p.z -= 2.* /*floor*/(5.*pyramid(10.*(.038+time-5.25)));
		} else if(AnimStep>5) {
			p.x -= 3.2-steps(10.*(.038+6.75-time),.75);
			p.z -= 2.* /*floor*/(5.*pyramid(10.*(.038+6.75-time)));
		} else {
			p.x-=3.2;
		}
		p*=scaleAlien;
		vec4 dAlien = DEAlien(p);
		dAlien.x/=scaleAlien;
		res = (dAlien.x<res.x) ? dAlien: res;
	}
	return res;
}

vec3 N(in vec3 p) {
    vec2 e = vec2(Ve.x, -Ve.x); 
    return normalize(e.xyy * DE(p + e.xyy).x + e.yyx * DE(p + e.yyx).x + e.yxy * DE(p + e.yxy).x + e.xxx * DE(p + e.xxx).x);;
}

float softshadow(in vec3 ro, in vec3 rd, in float mint, in float maxt, in float k) {
	float res = 1.0, h, t = mint;
    for( int i=0; i<20; i++ ) {
		h = DE( ro + rd*t ).x;
		res = min( res, k*h/t );
                if( res<0.0001 ) break;
		t += 0.02;
    }
    return clamp(res, 0., 1.);
}

// ------------------------------------------------------------------------------
// Deep Space 
// Star Nest by Kali
// https://www.shadertoy.com/view/4dfGDM

#define iterations 17
#define formuparam 0.53
#define volsteps 10
#define stepsize 0.1
#define tile   0.850
#define brightness 0.0015
#define darkmatter 1.500
#define distfading .530
#define saturation 0.650

vec4 space(vec3 rd)
{
	vec3 dir=rd;
	vec3 from=vec3(1.,.5,0.5);

	//volumetric rendering
	float s=0.1,fade=1.;
	vec3 v=vec3(0.);
	for (int r=0; r<volsteps; r++) {
		vec3 p=from+s*dir*.5;
		p = abs(vec3(tile)-mod(p,vec3(tile*2.))); // tiling fold
		float pa,a=pa=0.;
		for (int i=0; i<iterations; i++) { 
			p=abs(p)/dot(p,p)-formuparam; // the magic formula
			a+=abs(length(p)-pa); // absolute sum of average change
			pa=length(p);
		}
		float dm=max(0.,darkmatter-a*a*.001); //dark matter
		a*=a*a; // add contrast
		if (r>6) fade*=1.-dm; // dark matter, don't render near
		v+=fade;
		v+=vec3(s,s*s,s*s*s*s)*a*brightness*fade; // coloring based on distance
		fade*=distfading; // distance fading
		s+=stepsize;
	}
	v=mix(vec3(length(v)),v,saturation); //color adjust
	return vec4(v*.01,1.);	
}

// ------------------------------------------------------------------------------
// Metal effect
// Exterminate! by Antonalog
// https://www.shadertoy.com/view/ldX3RX
float one_pi = 0.31830988618;
float lightIntensity = 1.0;

vec3 rho_d = vec3(0.147708, 0.0806975, 0.033172);
vec3 rho_s = vec3(0.160592, 0.217282, 0.236425);
vec3 alpha = vec3(0.122506, 0.108069, 0.12187);
vec3 ppp = vec3(0.795078, 0.637578, 0.936117);
vec3 F_0 = vec3(9.16095e-12, 1.81225e-12, 0.0024589);
vec3 F_1 = vec3(-0.596835, -0.331147, -0.140729);
vec3 K_ap = vec3(5.98176, 7.35539, 5.29722);
vec3 sh_lambda = vec3(2.64832, 3.04253, 2.3013);
vec3 sh_c = vec3(9.3111e-08, 8.80143e-08, 9.65288e-08);
vec3 sh_k = vec3(24.3593, 24.4037, 25.3623);
vec3 sh_theta0 = vec3(-0.284195, -0.277297, -0.245352);

void initShipColor() {			
	rho_d = vec3(0.0657916, 0.0595705, 0.0581288);
	rho_s = vec3(1.55275, 2.00145, 1.93045);
	alpha = vec3(0.0149977, 0.0201665, 0.0225062);
	ppp = vec3(0.382631, 0.35975, 0.361657);
	F_0 = vec3(4.93242e-13, 1.00098e-14, 0.0103259);
	F_1 = vec3(-0.0401315, -0.0395054, -0.0312454);
	K_ap = vec3(50.1263, 38.8508, 34.9978);
	sh_lambda = vec3(3.41873, 3.77545, 3.78138);
	sh_c = vec3(6.09709e-08, 1.02036e-07, 1.01016e-07);
	sh_k = vec3(46.6236, 40.8229, 39.1812);
	sh_theta0 = vec3(0.183797, 0.139103, 0.117092);
}
			
vec3 Fresnel(vec3 F0, vec3 F1, float V_H)
{
	return F0 - V_H * F1  + (1. - F0)*pow(1. - V_H, 5.);
}

vec3 D(vec3 _alpha, vec3 _p, float cos_h, vec3 _K)
{
	float cos2 = cos_h*cos_h;
	float tan2 = (1.-cos2)/cos2;
	vec3 ax = _alpha + tan2/_alpha;
	
	ax = max(ax,0.); //bug?
	
	return one_pi * _K * exp(-ax)/(pow(ax,_p) * cos2 * cos2);
	// return vec3( 0.0 / (cos2 * cos2));
}

vec3 G1(float theta) {
	theta = clamp(theta,-1.,1.); //bug?
	return 1.0 + sh_lambda * (1. - exp(sh_c * pow(max(acos(theta) - sh_theta0,0.), sh_k)));
}

vec3 shade(float inLight, float n_h, float n_l, float n_v, float v_h)
{
  	return  one_pi*inLight*(n_l*rho_d+rho_s*D(alpha,ppp,n_h,K_ap)*G1(n_l)*G1(n_v)*Fresnel(F_0,F_1,v_h));
}

vec3 brdf(vec3 lv, vec3 ev, vec3 n)
{
	vec3 halfVector = normalize(lv + ev);
	
	float v_h = dot(ev, halfVector);
	float n_h = dot(n, halfVector);
	float n_l = dot(n, lv); 
	float inLight = 1.0;
	if (n_l < 0.) inLight = 0.0;
	float n_v = dot(n, ev); 
	
	vec3 sh = shade(inLight, n_h, n_l, n_v, v_h);
	sh = clamp( sh, 0., 1.); //bug?
	vec3 retColor = lightIntensity * sh;
	
	return retColor;
}

// -------------------------------------------------------------------- 

	
vec3 findColor(float obj, vec2 uv, vec3 n) {
	if (obj == FLAG) {
// FLAG
		float c = textureInvader(uv);
		return vec3(1.,c, c);
	} else if (obj == PLANET) {
// PLANET
		return mix(vec3(.7,.3,0),vec3(1,0,0), clamp(1.1-5.*(uv.x-1.8),0.1,.9));
	} else if (obj == SHIP_SIDE) {
		float spi = textureSpiral(uv);
		return mix(COLOR_SIDE, .4*COLOR_SIDE, spi);
	} else {
		vec3 c, sp = space(n).xyz;
		if (obj == SHIP_GLOB || obj == SHIP_HUBLOT) {
			c = mix(COLOR_GLOBE1, COLOR_GLOBE2, .5+.5*C2);
			return mix(c, sp, .8);
		} else if (obj == SHIP_ARM) {
			return mix(vec3(1), sp, .2);
		} else {			
			float spi = textureSpiral(uv);
			const vec3 lightblue = .25*vec3(0.5, 0.7, 0.9);
			c = mix(lightblue,lightblue*.4, spi);
			return mix(c, sp, .4);
		}
	}
}

vec3 Render(in vec3 p, in vec3 rd, in float t, in float obj, in vec2 uv) {
	//return V01.xxy*(dot(N(p),L));	

	vec3 nor = N(p);
	vec3 col = findColor(obj, uv, reflect(rd,nor));	
	vec3 sunLight = L;
	float	amb = clamp(.5+.5*nor.y, .0, 1.),
            dif = clamp(dot( nor, sunLight ), 0., 1.),
            bac = clamp(dot( nor, normalize(vec3(-sunLight.x,0.,-sunLight.z))), 0., 1.)*clamp( 1.0-p.y,0.0,1.0);

	float sh = softshadow( p, sunLight, .02, 100., 7.); 
	
	if (obj != PLANET && obj != FLAG) {
		if (obj != SHIP_ARM) {
			initShipColor();
		}
		float gamma = 2.2;
		lightIntensity *= 10.*(5.+.5*sh);
		col *= (brdf(sunLight, -rd, nor) + .4*brdf(-sunLight, -rd, nor));
		return sqrt(col);
	} else {dif *= sh; 

			vec3 brdf = 
				.2*(amb*vec3(.10,.11,.13) + bac*.15) +
				1.2*dif*vec3(1.,.9,.7);
	
			float 
				pp = (obj == PLANET) ? 0. : clamp(dot(reflect(rd,nor), sunLight),0.,1.),
				spe = 2.*sh*pow(pp,16.),  // brillance
				fre = pow( clamp(1.+dot(nor,rd),0.,1.), 2.);
	
			col = col*(.1+brdf + spe) + .2*fre*(.5+.5*col)*exp(-.01*t*t);
		
		return sqrt(clamp(col,0.,1.));
	}
}


mat3 lookat(in vec3 ro, in vec3 up){
	vec3 fw=normalize(ro);
	vec3 rt=normalize(cross(fw,up));
	return mat3(rt, cross(rt,fw),fw);
}



vec3 RD(in vec3 ro, in vec3 cp, vec2 fCoord) {
	return lookat(cp-ro, V01.yxx)*normalize(vec3((2.*fCoord-iResolution.xy)/iResolution.y, 12.0));
}


void main() {
	time = 3.*TAO+iTime*.75;
	
	C1 = COS(time);
	S1 = SIN(time);
	S2 = 2.*S1*C1;
	C2 = 1.-2.*S1*S1;
	
// Animation	
	time /= TAO;
	AnimStep = 0;

	vec3 cp = (AnimStep<2) ? vec3(-2,0,0) : vec3(0,0,0);
	float rCam = (AnimStep<2)?5.:45.;

	if (time > 7.25) {
		AnimStep = 7; // apres de la remontee
		rCam = mix(45.,5.,clamp(time-7.25,0.,1.));
		cp = mix(vec3(0,0,0), vec3(-2,0,0),clamp(time-7.25,0.,1.));
	} else if (time>6.75) {
	 	AnimStep = 6; // apres de la remontee
		rCam = 45.;
//		cp = vec3(0,0,0);
		cp = mix(vec3(1,0,0),vec3(0,0,0),clamp(time-6.25,0.,1.));
	} else if (time>6.5) {
		AnimStep = 5; // remontee sans drapeau
		rCam = 45.;
//		cp = vec3(0,0,0);
		cp = mix(vec3(1,0,0),vec3(0,0,0),clamp(time-6.25,0.,1.));
	} else if (time>6.25) {
		AnimStep = 4; // pause du drapeau
		rCam = 45.;
		cp = mix(vec3(1,0,0),vec3(0,0,0),clamp(time-6.25,0.,1.));
	} else if (time>5.25) {
		AnimStep = 3; // pause
		rCam = mix(160.,45.,clamp(time-5.25,0.,1.));
		cp = vec3(1,0,0);
	} else if (time>3.25) {		
		AnimStep = 2; // arrivee du vaiseau
		rCam = mix(5.,160.,clamp(time-3.25,0.,1.));
		cp = mix(vec3(-2,0,0), vec3(1,0,0),clamp(time-3.25,0.,1.));
	}

	vec3 rd, ro = rCam*vec3(-.5+4.*iMouse.y/iResolution.y,
						-SIN(time*2.12+iMouse.x/iResolution.x),
						-COS(time*2.12)
					    );
	
	vec3 ctot = vec3(0);
	
#ifdef ANTIALIASING 
	for (int i=0;i<AA;i++) {
		vec2 fCoord = gl_FragCoord.xy+.4*vec2(COS(6.28*float(i)/float(AA)),SIN(6.28*float(i)/float(AA)));	
#else
		vec2 fCoord = gl_FragCoord.xy;
#endif
		rd = RD(ro, cp, fCoord);
	
        withPlanet = intersectSphere(ro, rd, vec3(-2.,0,0), .21);
        
		// Ray marching
		float t=0.0,d=1.0,od=1.0;
		vec4 res;
		for(int i=0;i<NB_ITER;i++){
			if(d<PRECISION|| t>MAX_DIST)break;
			t += res.x;
			res=DE(ro+rd*t); // *0.95;
		}
	
		// Render colors
		if(t<MAX_DIST){// if we hit a surface color it
			ctot += Render(ro + rd*t, rd,t, res.y, res.zw);
		} else {
			ctot += space(rd).xyz;
		}
#ifdef ANTIALIASING 		
    }
	ctot /= float(AA);	
#endif 		
	fragColor = vec4(ctot,1.0);
}
