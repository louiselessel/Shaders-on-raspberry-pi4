#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;

out vec4 fragColor;

//"[SH17B] Cold Shore" by Xor
//License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

#define FOG 100.
#define PRE .01

#define SUN normalize(vec3(0,4,1))

float height(vec2 P)
{
    float H = 8.*texture(iChannel0,P/96.).r+
              2.*texture(iChannel0,P/25.).r+
              .1*texture(iChannel0,P/2.).r;
    return min(H-.1*P.y,5.02+.02*pow(cos(3.*H-3.*P.y+iTime),2.));
}

vec4 raymarch(vec4 P,vec3 R)
{
    P = vec4(P.xyz+R*2.,2);
    float E = 1.;
 	for(int i = 0;i<300;i++)
    {
        P += vec4(R,1)*E;
        float H = height(P.xy);
        E = clamp(E+(H-P.z)-.5,E,1.);
        if (H-E*.6<P.z)
        {
        	P -= vec4(R,1)*E;
            E *= .7;
            if (E<PRE*P.w/FOG) break;
        }
    }
    return P;
}

float shadow(vec4 P,vec3 R)
{
    float S = 0.;
    P = vec4(P.xyz,0);
    float E = .5;
 	for(int i = 0;i<10;i++)
    {
        
        P += vec4(R,1)*E;
        float H = height(P.xy);
        if (H<P.z)
        {
            S = min((H-P.z)*9.,S);
        }
    }
    return S;
}

float bump(vec2 P)
{
 	return texture(iChannel1,P*4.).r*texture(iChannel1,P).r*texture(iChannel1,P/4.).r;
}

vec3 normal(vec2 P)
{
    vec2 N = vec2(1,0);
    N = height(P-N.xy*PRE)*-N.xy+height(P+N.xy*PRE)*N.xy+height(P-N.yx*PRE)*-N.yx+height(P+N.yx*PRE)*N.yx;
 	return normalize(vec3(N,-PRE));
}

vec3 sky(vec3 R)
{
    vec3 S = vec3(1,.5,.2)/(dot(R,SUN)*99.+99.5);
    vec2 P = R.xy/sqrt(max(.1-R.z,.1))*9.;
    float C = (cos(P.y+cos(P.x*.5+cos(P.y)))*cos(P.y*.7)*.5+.5)*min(abs(.1-6.*R.z),1.);
 	return mix(vec3(.4,.7,.9),vec3(1),C*C)+S;
}

vec3 color(vec4 P,vec3 R)
{
    vec3 N = normal(P.xy);
    vec3 I = reflect(R,N);
    float F = min(P.w/FOG,1.);
    float L = exp(shadow(P,-SUN)+dot(N,-SUN)-1.);
    vec3 S = mix(vec3(.3,.4,.5),vec3(1),L)-bump(P.xy);
    vec3 C = (P.z<5.)?vec3(.4+.6*smoothstep(-PRE*(5.+50.*F),-PRE*4.,P.z-height(P.xy-PRE*vec2(8,2)))):
         sky(I)*min(I.z+1.,1.)+max(-I.z,0.)*vec3(.1,.2,.4);
 	return mix(C*S,sky(R),F*F);
}

void main()
{    
    vec2 Coord = (gl_FragCoord.xy-iResolution.xy*.5)/iResolution.y;
    float A = 3.*(.5-(iMouse.y/iResolution.y))*sign(iMouse.y)+.6-.3*cos(iTime*.3);
    float B = 3.*(.5-(iMouse.x/iResolution.x))*sign(iMouse.x);
    vec3 R = normalize(vec3(1,Coord))*mat3(cos(A),0,sin(A),0,1,0,sin(A),0,-cos(A))*mat3(cos(B),sin(B),0,sin(B),-cos(B),0,0,0,1);
    vec4 P = raymarch(vec4(iTime,cos(iTime*.2),-3.+2.*cos(iTime*.3),0),R);
    vec4 C = vec4(color(P,R),1);
    fragColor = C;
}
