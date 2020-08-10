//https://www.shadertoy.com/view/4lXGzj
#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;
uniform sampler2D iChannel0;
out vec4 fragColor;

#define LightDir normalize(vec3(1.,.5,0.))
#define Height .1
#define WaterLevel .02
#define res .001

float terrain(vec2 p) {
	p*=.15;
	float h=0.;
	h+=texture(iChannel0,p).g;
	h+=texture(iChannel0,p*3.+.2568).r*.5;
	h+=texture(iChannel0,p*8.+.1257).r*.25;
	h+=texture(iChannel0,p*12.+.6542).r*.15;
	h*=Height;
	return h;
}
vec3 normal(vec2 p) {
	vec2 eps=vec2(0,res*.5);
	float d1=terrain(p+eps.xy)-terrain(p-eps.xy);
	float d2=terrain(p+eps.yx)-terrain(p-eps.yx);
	vec3 n1=(vec3(0.,eps.y*2.,d1));
	vec3 n2=(vec3(eps.y*2.,0.,d2));
	return normalize(cross(n1,n2));
}
void main()
{
	vec2 m=iMouse.xy/iResolution.xy;
	if (m.y<.001) m.y=.5;
	vec2 uv = gl_FragCoord.xy / iResolution.xy;
	vec2 p2=vec2(0.);
	float terr=0.;
	float t=(iTime+5.)*.1;
	float wl=0.;
	for (int l=0; l<200; l++) {
		float scan=uv.y-float(l)*res;
		float persp=(1.-(1.-scan)*.4);
		vec2 p=vec2((uv.x-.5)*persp*2.,scan*2.)+vec2(t+m.x*6.,-7.6);
		float h=terrain(p)/persp;
		if (scan+h>uv.y) {p2=p;terr=h*persp;wl=(WaterLevel-(.5-m.y)*.07)/persp;}
	}
	vec3 n=normal(p2);
	float gr=pow(max(0.,dot(n,normalize(vec3(0.,0.,-1.)))),4.);
	float nterr=pow(clamp(terr/Height,0.,.97),2.);
	vec3 col=vec3(nterr,nterr*nterr,nterr*nterr*nterr)+vec3(.0,-.1,-.3);
	col=mix(col,vec3(gr*gr,gr*1.2,gr*gr*gr)*1.3,pow(max(0.,gr-nterr*.5),2.));
	col*=max(0.,dot(n,LightDir))+.1;
	col+=pow(max(0.,dot(n,LightDir)),4.)*.8;
	if (terr<wl) {
		col*=vec3(1.,.8,1.);
		float nwl=(wl-terr)/wl;
		col=mix(col,vec3(0.6,0.9,1.)*(1.-smoothstep(0.,.5,nwl)*.25)*.4,clamp(nwl*3.5,0.,1.));
		col*=1.+max(0.,1.-abs(terr-wl)*100.);
	}
	col*=1.-pow(uv.y,3.);
	col=mix(col,vec3(length(col)),.55);
	col=pow(col+.15,vec3(1.25));
	terr=max(wl,terr);
	col=mix(col,vec3(.93,.9,.87),clamp(pow(uv.y,1.5)-terr*2.5,0.,1.));
	fragColor = vec4(col,1);
}
