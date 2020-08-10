

// Pi variables
	//#version 330 				// currently defining the version (eg 330 or 120) breaks the code for some reason
	#ifdef GL_ES
	precision mediump float;
	#endif

	uniform vec3 unif[20]; 
	// is a vec3 so there is three spaces to place a uniform
	// for some reason they are in array place index 16
	// 16,0 is uniform0
	// 16,1 is uniform1
	// 16,2 is uniform2
	
 
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
 


 

 
float noise(vec3 p) //Thx to Las^Mercury
{
	vec3 i = floor(p);
	vec4 a = dot(i, vec3(1., 57., 21.)) + vec4(0., 57., 21., 78.);
	vec3 f = cos((p-i)*acos(-1.))*(-.5)+.5;
	a = mix(sin(cos(a)*a),sin(cos(1.+a)*(1.+a)), f.x);
	a.xy = mix(a.xz, a.yw, f.y);
	return mix(a.x, a.y, f.z);
}

float sphere(vec3 p, vec4 spr)
{
	return length(spr.xyz-p) - spr.w;
}

float flame(vec3 p)
{
	float d = sphere(p*vec3(1.,.5,1.), vec4(.0,-1.,.0,1.));
	return d + (noise(p+vec3(.0,iTime*2.,.0)) + noise(p*3.)*.5)*.25*(p.y) ;
}

float scene(vec3 p)
{
	return min(100.-length(p) , abs(flame(p)) );
}

vec4 raymarch(vec3 org, vec3 dir)
{
	float d = 0.0, glow = 0.0, eps = 0.02;
	vec3  p = org;
	bool glowed = false;
	
	for(int i=0; i<64; i++)
	{
		d = scene(p) + eps;
		p += d * dir;
		if( d>eps )
		{
			if(flame(p) < .0)
				glowed=true;
			if(glowed)
       			glow = float(i)/64.;
		}
	}
	return vec4(p,glow);
}

void main()
{
	vec2 v = -1.0 + 2.0 * gl_FragCoord.xy / iResolution.xy;
	v.x *= iResolution.x/iResolution.y;
	
	vec3 org = vec3(0., -2., 4.);
	vec3 dir = normalize(vec3(v.x*1.6, -v.y, -1.5));
	
	vec4 p = raymarch(org, dir);
	float glow = p.w;
	
	vec4 col = mix(vec4(1.,.5,.1,1.), vec4(0.1,.5,1.,1.), p.y*.02+.4);
	
	gl_FragColor = mix(vec4(0.), col, pow(glow*2.,4.));
	//gl_FragColor = mix(vec4(1.), mix(vec4(1.,.5,.1,1.),vec4(0.1,.5,1.,1.),p.y*.02+.4), pow(glow*2.,4.));

}
 
