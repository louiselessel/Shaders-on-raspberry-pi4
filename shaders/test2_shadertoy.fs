//#version 330
#ifdef GL_ES
precision mediump float;
#endif

// Add this in so I can pass in uniforms
uniform vec3 unif[20]; 
// is a vec3 so there is three spaces to place a uniform
// for some reason they are in array place index 16
// 16,0 is uniform0
// 16,1 is uniform1
// 16,2 is uniform2

// ShaderToy variables boilerplate
// can only get it to work by commenting out...
/*
	uniform vec3      iResolution;           // viewport resolution (in pixels)
	uniform float     iTime;                 // shader playback time (in seconds)
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
 
// Shader from shadertoy
// http://www.pouet.net/prod.php?which=57245
// If you intend to reuse this shader, please add credits to 'Danilo Guanabara'
// http://www.shadertoy.com/view//XsXXDnv


// Why don't #define work? If I comment them in, the code breaks
//#define t iTime
//#define r iResolution.xy

void main() {
	
	// THIS WORKS
	
	vec2 iResolution = vec2(unif[16][1],unif[16][2]);
	float iTime = unif[16][0];
	
	// The #defines don't work so I can remake them in the main()
	float t = iTime;
	vec2 r = iResolution;
	
	// From the original shader code
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
	gl_FragColor=vec4(c/l,t);
	
	
	/*
	// THIS WORKS
	
	// The #defines don't work so instead I have been doing this.
	// I found that I have to assign the unif[...] in the main loop?
	float t = i;
	vec2 r = vec2(unif[16][1],unif[16][2]);
	
	// From the original shader code
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
	gl_FragColor=vec4(c/l,t);
	*/
	
	
	/*
	// THIS WORKS
	
	// Actually using the uniforms from the
	// shadertoy boilerplate doesnt work, and I can't assign them above
	// and then use them here...
	// Commenting this in + the boilerplate will break the code!
	
	//iResolution = vec3(unif[16][1],unif[16][2], 0.0); // dont work
	//iResolution.xy = vec2(unif[16][1],unif[16][2]);	// dont work
	
	// This approach works - remaking them in the main()
	 vec2 iResolution = vec2(unif[16][1],unif[16][2]);
	 float iTime = unif[16][0];
	
	// Went through the code to change to iTime and iResolution
	// ... from the original shader code
	vec3 c;
	float l,z = iTime;
	
	
	for(int i=0;i<3;i++) {
		vec2 uv,p=gl_FragCoord.xy/iResolution;
		uv=p;
		p-= 0.5;
		p.x*=iResolution.x/iResolution.y;
		z+=.07;
		l=length(p);
		uv+=p/l*(sin(z)+1.)*abs(sin(l*9.-z*2.));
		c[i]=.01/length(abs(mod(uv,1.)-.5));
	}
	gl_FragColor=vec4(c/l,iTime);
	*/
}
