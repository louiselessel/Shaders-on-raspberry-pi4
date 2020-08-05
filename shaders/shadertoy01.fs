#include std_head_fs.inc

#define iTime unif[1][0]
#define iResolution unif[0]
#define scale unif[1][1]

// Example of https://www.shadertoy.com/view/XsXXDn
// http://www.pouet.net/prod.php?which=57245
// If you intend to reuse this shader, please add credits to 'Danilo Guanabara'

void main(void) {
    float t = iTime;
    vec2 r = iResolution.xy;
    float invScale = 1.0 / scale; // obviously scale must not be zero!
    float offset = vec2(invScale - 1.0) * 0.5;

    vec3 c;
    float len, z=t;
    for(int i=0; i<3; i++) {
        vec2 uv;
        vec2 p = gl_FragCoord.xy / r * invScale - offset;
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
