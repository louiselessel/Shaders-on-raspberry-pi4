
// Pi variables
	//#version 330 				// currently defining the version (eg 330 or 120) breaks the code for some reason
	//#ifdef GL_ES
	//	precision mediump float;
	//#endif
	
	#include std_head_fs.inc

	varying vec2 texcoordout;

// ShaderToy variables (under implementation as uniforms)
	vec3      iResolution = vec3(unif[16][1],unif[16][2], 0.0); // viewport resolution (in pixels)
	float     iTime = unif[16][0];             					// shader playback time (in seconds)
	float     iTimeDelta = 1.0;	              					// render time (in seconds)
	int       iFrame = int(1);                					// shader playback frame
	//float     iChannelTime[4] = 1.0;       					// channel playback time (in seconds)
	//vec3      iChannelResolution[4]; 							// channel resolution (in pixels)
	vec4      iMouse = vec4(0.5, 0.5, 0.0, 0.0);               	// mouse pixel coords. xy: current (if MLB down), zw: click
	//sampler2D iChannel0;          	 						// input channel
	//sampler2D iChannel1;
	//sampler2D iChannel2;
	//sampler2D iChannel3;
	//vec4      iDate;                 							// (year, month, day, time in seconds)
 
 
// http://www.pouet.net/prod.php?which=57245
// If you intend to reuse this shader, please add credits to 'Danilo Guanabara'

//#define  t iTime
//#define r iResolution.xy
float t = iTime;
vec2 r = iResolution.xy;
vec2 tc = texcoordout;


void main(){
	#include std_main_uv.inc
	vec3 c;
	float l,z=t;
	
	for(int i=0;i<3;i++) {
		//vec2 uv,p=gl_FragCoord.xy/r; // original code - should normalize coords
		//vec2 uv,p=texc.xy;		 // rainbow img (texc contains img buffer colors)
		vec2 uv,p=tc; 	 		 // close, skewed
		//vec2 uv,p=gl_FragCoord.xy; // no jitter spots
		//vec2 uv,p=tc/r; 			// long rainbow lines - close corner is right
		//vec2 uv,p=r/tc; 			// no jitter spots
		//vec2 uv,p=gl_FragCoord.xy/r; 	//
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
 
