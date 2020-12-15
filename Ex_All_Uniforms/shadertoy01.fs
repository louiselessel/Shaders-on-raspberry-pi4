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

void main(void) {
    float t = iTime;
    vec2 r = iResolution.xy;
    float invScale = 1.0 / iScale; // obviously scale must not be zero!
    vec2 offset = vec2(invScale - 1.0) * 0.5;

    vec3 c;
    float len, z=t;
    for(int i=0; i<3; i++) {
        vec2 uv;
        vec2 p = (gl_FragCoord.xy / r) * invScale - offset;
        uv = p;
        p -= 0.5;
        p.x *= r.x / r.y;
        z += 0.07;
        len = length(p);
        uv += p / len * (sin(z) + 1.0) * abs(sin(len * 9.0 - z * 2.0));
        c[i] = 0.01 / length(abs(mod(uv, 1.0) - 0.5));
    }
    gl_FragColor = vec4(c / len, t);
}
