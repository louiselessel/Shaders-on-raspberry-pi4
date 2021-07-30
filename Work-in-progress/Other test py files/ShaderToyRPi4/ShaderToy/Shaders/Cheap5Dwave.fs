//Cheap 5D wave
//https://www.shadertoy.com/view/lscyD7

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

out vec4 fragColor;

/*
5D wave noise
afl_ext 2018
public domain
*/

float hash(float p){
    return fract(4768.1232345456 * sin(p));
}

#define EULER 2.7182818284590452353602874
float wave(vec4 uv, vec4 emitter, float speed, float phase, float timeshift){
    float dst = distance(uv, emitter);
    return pow(EULER, sin(dst * phase - timeshift * speed)) / EULER;
}
vec4 wavedrag(vec4 uv, vec4 emitter){
    return normalize(uv - emitter);
}
float seedWaves = 0.0;
vec4 randWaves(){
    float x = hash(seedWaves);
    seedWaves += 1.0;
    float y = hash(seedWaves);
    seedWaves += 1.0;
    float z = hash(seedWaves);
    seedWaves += 1.0;
    float w = hash(seedWaves);
    seedWaves += 1.0;
    return vec4(x,y,z,w) * 2.0 - 1.0;
}

float getwaves5d(vec4 position, float dragmult, float timeshift){
    float iter = 0.0;
    float phase = 6.0;
    float speed = 2.0;
    float weight = 1.0;
    float w = 0.0;
    float ws = 0.0;
    for(int i=0;i<20;i++){
        vec4 p = randWaves() * 30.0;
        float res = wave(position, p, speed, phase, 0.0 + timeshift);
        float res2 = wave(position, p, speed, phase, 0.006 + timeshift);
        position -= wavedrag(position, p) * (res - res2) * weight * dragmult;
        w += res * weight;
        iter += 12.0;
        ws += weight;
        weight = mix(weight, 0.0, 0.2);
        phase *= 1.2;
        speed *= 1.02;
    }
    return w / ws;
}

// helper function

vec3 polarToXyz(vec2 xy){
    xy *= vec2(2.0 *3.1415,  3.1415);
    float z = cos(xy.y);
    float x = cos(xy.x)*sin(xy.y);
    float y= sin(xy.x)*sin(xy.y);
    return normalize(vec3(x,y,z));
}

void main()
{
    vec2 uv = gl_FragCoord.xy/iResolution.xy;
	vec2 mouse = iMouse.xy / iResolution.xy;
    vec3 col = vec3(1.0) * getwaves5d(vec4(polarToXyz(uv), (mouse.x * 2.0 - 1.0) * 10.0), 10.0 + mouse.y * 10.0, iTime);

    // Output to screen
    fragColor = vec4(col,1.0);
}
