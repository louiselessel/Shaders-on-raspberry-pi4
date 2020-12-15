
    #include std_head_fs.inc
    #define iResolution unif[0]
    #define iTime unif[0][2]
    #define iTimeDelta unif[1][0]
    #define iScale unif[1][1]
    #define iFrame unif[1][2]
    #define iMouse vec4(unif[2][0], unif[2][1], unif[3][0], unif[3][1])
    #define iDate vec4(unif[4][0], unif[4][1], unif[4][2], unif[5][0])
    
    //#define ownVar1 unif[16]
    //#define ownVar2 unif[19]
    
 


 

 
// http://www.pouet.net/prod.php?which=57245
// If you intend to reuse this shader, please add credits to 'Danilo Guanabara'

#define  t iTime
#define r iResolution.xy

void main(){
    float invScale = 1.0 / iScale; // obviously scale must not be zero!
    vec2 offset = vec2(invScale - 1.0) * 0.5;
    vec3 c;
    float l,z=t;
    for(int i=0;i<3;i++) {
        vec2 uv,p= (gl_FragCoord.xy/r) * invScale - offset; // normalization of sampling coordinates - you may have to delete offset if black output. And make sure the scaling is only added to the line that looks something like: vec2 name = gl_FragCoord/iResolution
        uv=p;
        p-=.5;
        p.x*=r.x/r.y;
        z+=.07;
        l=length(p);
        uv+=p/l*(sin(z)+1.)*abs(sin(l*9.-z*2.));
        c[i]=.01/length(abs(mod(uv,1.)-.5));
    }
    gl_FragColor=vec4(c/l,t);
}
 
