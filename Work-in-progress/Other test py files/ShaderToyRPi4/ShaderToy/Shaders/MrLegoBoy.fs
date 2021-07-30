//https://www.shadertoy.com/view/3lXXzl

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

#define maxsteps 100
#define maxdist 100.
#define surfdist 0.1
#define balls 10
vec3 meta[balls];
float metastr[balls];

float GetDist(vec3 p){
          float metadif=0.;
    for(int i=0; i<balls; i++) {
 		metadif +=metastr[i]/length(p-meta[i]);
        
    }
    float metadist = 1.-metadif;
    vec4 s = vec4(0.,1.,6.,1.);
    float spheredist = length(p-s.xyz)-s.w;
    float planedist=p.y;
    float d= min(metadist,spheredist);

    return d;
}
vec3 GetNormal(vec3 p){
    float d = GetDist(p);
    vec2 e = vec2(.01,0);
    vec3 n=d-vec3(
        GetDist(p-e.xyy),
        GetDist(p-e.yxy),
        GetDist(p-e.yyx));
         
    return normalize(n);
    
}
float getlight(vec3 p) {
    vec3 lightpos = vec3(0,    5 , 6);
    lightpos.xz += vec2(sin(iTime),cos(iTime));
    vec3 l = normalize(lightpos-p);
    vec3 n= GetNormal(p);
    float dif = dot(n, l);
    return ((dif +1.)/2.);
}


float raymarch(vec3 ro, vec3 rd){
    float d0=0.;
        
    for(int i=0; i<maxsteps; i++) {
        vec3 p = ro + rd*d0;
        float ds = GetDist(p);
        d0 += ds;
        if (d0>maxdist||ds<surfdist) break;
    }
	return d0;
}



										
void main()
{
    for(int i=0; i<(balls); i+=1) {
 meta[i]=meta[i]+vec3(sin(float(i)*iTime),sin(float(i)*iTime*float(balls/5)),sin(float(i)*iTime*float(balls/2)));   
    metastr[i]=0.1;
}
 	vec2 uv=(gl_FragCoord.xy-0.5*iResolution.xy)/iResolution.y;
    
    vec3 col = vec3(0);

    vec3 ro = vec3(0,sin(iTime),-5);
    vec3 rd = normalize(vec3(uv.x,uv.y,1));
    float d = raymarch(ro,rd);
    vec3 p = ro+rd *d;
    float dif = getlight(p);
    col= vec3(dif);
    fragColor = vec4(col,1.0);
}
