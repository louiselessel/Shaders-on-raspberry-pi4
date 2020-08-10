//#version 330
#ifdef GL_ES
precision mediump float;
#endif

uniform vec3 unif[20]; 
// is a vec3 so there is three spaces to place a uniform
// for some reason they are in array place index 16
// 16,0 is uniform0
// 16,1 is uniform1
// 16,2 is uniform2

//----------------------
// None of these methods work
//uniform float     iTime;                 // shader playback time (in seconds)
//uniform float iTime = unif[16][0];

//#define t iTime;
//float t = iTime;

//#define t unif[16][0];

//uniform float t;

//uniform float iTime => unif[16][0];


//------------------------

float t = unif[16][0]; 		// ------------------ This works

//------------------------

//layout (location = 0) out vec4 fragColor;

// ShaderToy variables
/*
	uniform vec3      iResolution;           // viewport resolution (in pixels)
	//uniform float     iTime;                 // shader playback time (in seconds)
	uniform float     iTimeDelta;            // render time (in seconds)
	uniform int       iFrame;                // shader playback frame
	uniform float     iChannelTime[4];       // channel playback time (in seconds)
	uniform vec3      iChannelResolution[4]; // channel resolution (in pixels)
	uniform vec4      iMouse;                // mouse pixel coords. xy: current (if MLB down), zw: click
	uniform sampler2D iChannel0;          	 // input channel
	uniform sampler2D iChannel1;
	uniform sampler2D iChannel2;
	uniform sampler2D iChannel3;
	uniform vec4      iDate;                 // (year, month, day, time in seconds)
 
 */
// http://www.pouet.net/prod.php?which=57245
// If you intend to reuse this shader, please add credits to 'Danilo Guanabara'

//#define t iTime
//#define r iResolution.xy



void main() {
	//float t = unif[16][0];
	vec2 r;
	r.x = unif[16][1];
	r.y = unif[16][2];

	
	vec3 c;
	float l,z =t;
	
	
	for(int i=0;i<3;i++) {
		vec2 uv,p=gl_FragCoord.xy/r;
		uv=p;
		p-= 0.5;
		p.x*=r.x/r.y;
		z+=.07;
		l=length(p);
		uv+=p/l*(sin(z)+1.)*abs(sin(l*9.-z*2.));
		c[i]=.01/length(abs(mod(uv,1.)-.5));
	}
		
	//gl_FragColor = vec4(t,0.0,0.0,1.0);
	//gl_FragColor = vec4(l, r.y/r.y, r.x/r.x, 1.0);
	//gl_FragColor = vec4(z, 0.0, 0.0, 1.0);
	gl_FragColor=vec4(c/l,t);
}
