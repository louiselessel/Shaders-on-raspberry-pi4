
//https://www.shadertoy.com/view/lss3DB
// Beehive fragment shader by movAX13h, September 2013

// NOTE: The texture in channel 0 is used as base for the texture of the wings.
// If you wish to see 'The Bee' close-up, uncomment the following line:
//#define THE_BEE


#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

uniform sampler2D iChannel0;
out vec4 fragColor;

float time;

float rand( float n )
{
  	return fract(cos(n)*41415.92653);
}

float noise(vec2 p)
{
  	vec2 f  = smoothstep(0.0, 1.0, fract(p));
  	p  = floor(p);
  	float n = p.x + p.y*57.0;
  	return mix(mix(rand(n+0.0), rand(n+1.0),f.x), mix( rand(n+57.0), rand(n+58.0),f.x),f.y);
}

float fbm( vec2 p )
{
	mat2 m2 = mat2(1.6,-1.2,1.2,1.6);	
  	float f = 0.5000*noise( p ); p = m2*p;
  	f += 0.2500*noise( p ); p = m2*p;
  	f += 0.1666*noise( p ); p = m2*p;
  	f += 0.0834*noise( p );
  	return f;
}

float rand12(vec2 co)
{
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

vec2 rand21(float p)
{
	return fract(vec2(sin(p * 591.32), cos(p * 391.32)));
}

vec2 rotate(vec2 p, float a)
{
	return vec2(p.x * cos(a) - p.y * sin(a), p.x * sin(a) + p.y * cos(a));
}

const float hexSize = 20.0;

vec3 hexCoord(vec2 p)
{
	// simulating a rotated cube...
	// got suggestions?
	
    vec3 q = vec3(p / hexSize, 0.0);
	q.z = -0.5 * q.x - q.y;
	
    float z = -0.5 * q.x - q.y;
    q.y -= 0.5 * q.x;
	
	vec3 i = floor(q+0.5);
    float s = floor(i.x + i.y + i.z);
	vec3 d = abs(i-q);
	
	if( d.x >= d.y && d.x >= d.z ) i.x -= s;
	else if( d.y >= d.x && d.y >= d.z )	i.y -= s;
	else i.z -= s;
	
    vec2 coord = vec2(i.x, ( i.y - i.z + (1.0-mod(i.x, 2.0)) ) / 2.0 );
	float dist = length(p - vec2(coord.x, coord.y - 0.5*mod(i.x-1.0, 2.0))*hexSize) / hexSize;
	return vec3(coord, dist);
}

float wingsTex(vec2 p)
{
	vec3 col = texture(iChannel0, p.yx*5.0).rgb;
	return 0.3+0.8*smoothstep(0.2, 0.0, (col.r+col.g+col.b)/3.0);
}

void bee(vec2 pos, float size, inout vec3 col, float t, bool mirror)
{
	if (clamp(pos.x, 0.0, size) != pos.x || clamp(pos.y, 0.0, size) != pos.y) return;
	
	vec2 p = (pos / size - 0.5) - vec2(0.0, -0.1); // ran out of space ...
	if (mirror) p.x*=-1.0;	
	
	float b = (1.1 - p.y)/0.6;
	float flapping = -abs(sin(t*86.0)); // I know ...
	float f;
	
	// left wing
	f = smoothstep(0.0, -0.02, length(vec2(1.8*b, 1.0)*(rotate(p-vec2(-0.1+0.2*flapping, 0.43+0.1*flapping), 0.3+flapping)))-0.2);
	col += wingsTex(p+flapping)*f*b;
	
	// body
	f = smoothstep(0.0, -0.1, length(rotate(p-vec2(0.0, -0.05), 0.2+0.1*sin(time*4.0))*vec2(1.0, 1.4))-0.5);
	f = max(0.0, f-smoothstep(0.0, -0.1, length(vec2(1.0, 1.6)*(p-vec2(-0.45, -0.5)))-0.6));
	col = mix(col, p.y+vec3(1.0, 0.8, 0.0)-0.5*sin(24.0*length(vec2(1.0, 1.3)*(p-vec2(-0.5, 0.1)))), f);

	// right wing
	f = smoothstep(0.0, -0.02, length(vec2(1.8*b, 1.0)*(rotate(p-vec2(0.04-0.1*flapping, 0.38+0.1*flapping), 0.3-flapping)))-0.2);
	col += wingsTex(p-flapping)*f*b;
	
	// head
	f = smoothstep(0.0, -0.05, length(vec2(1.2, 1.0)*(p-vec2(-0.32, 0.03)))-0.2);
	col = mix(col, vec3(0.8, 0.6, 0.0)-0.12*cos(p.y*15.0), f);
	
	// left eye ball
	f = smoothstep(0.0, -0.02, length(vec2(1.4, 1.0)*(p-vec2(-0.42, 0.05)))-0.1);
	col = mix(col, vec3(smoothstep(0.2, -0.1, length(vec2(1.3, 1.0)*(p-vec2(-0.44, 0.05)))-0.1)), f);
	
	// left eye iris
	f = smoothstep(0.0, -0.02, length(vec2(1.6, 0.9)*(p-vec2(-0.46, 0.05)))-0.06);
	col = mix(col, vec3(0.2, 0.2, 0.8), f);
	
	// left eye pupil
	f = smoothstep(0.0, -0.02, length(vec2(1.4, 1.0)*(p-vec2(-0.47, 0.055)))-0.04);
	col = mix(col, vec3(0.0), f);

	// right eye ball	
	f = smoothstep(0.0, -0.02, length(vec2(1.4, 1.0)*(p-vec2(-0.34, 0.05)))-0.1);
	col = mix(col, vec3(smoothstep(0.2, -0.1, length(vec2(1.3, 1.0)*(p-vec2(-0.36, 0.05)))-0.1)), f);

	// right eye iris
	f = smoothstep(0.0, -0.02, length(vec2(1.6, 0.9)*(p-vec2(-0.365, 0.05)))-0.06);
	col = mix(col, vec3(0.2, 0.2, 0.8), f);
	
	// right eye pupil
	f = smoothstep(0.0, -0.02, length(vec2(1.4, 1.0)*(p-vec2(-0.372, 0.055)))-0.04);
	col = mix(col, vec3(0.0), f);
	
	// eyelids
	f = smoothstep(0.0, -0.03, length(vec2(1.32, 1.1)*(p-vec2(-0.386, 0.06)))-0.17);
	f = max(0.0, f-smoothstep(0.0, -0.03, length(vec2(1.0, 1.0)*(p-vec2(-0.42, -0.2 - step(0.99, sin(time*0.9))*max(0.0, 0.2*sin(t*18.0)))))-0.3));
	col = mix(col, vec3(1.0, 0.8, 0.0) - 0.3*sin(15.0*min(length(p-vec2(-0.46, 0.17)), length(p-vec2(-0.33, 0.16)))-0.4), f);
		
	// mouth (needs improvement!)
	f = smoothstep(0.0, -0.02, length(vec2(1.1, 0.7)*(p-vec2(-0.37, -0.11)))-0.06);
	col = mix(col, vec3(0.8, 0.6, 0.0)-0.07*sin(p.y*40.0+0.5)*sin(p.x*80.0+1.5), f);
	
	// left antenna (can get better)
	f = smoothstep(0.0, -0.03, length(vec2(1.6, 1.1)*(p-vec2(-0.47, 0.25)))-0.17);
	f = max(0.0, f-smoothstep(0.0, -0.03, length(vec2(1.6, 0.9)*(p-vec2(-0.49, 0.21)))-0.17));
	col = mix(col, vec3(0.8, 0.6, 0.0) + 0.4*sin(p.y*18.0-0.4), f);

	// right antenna (can get better)
	f = smoothstep(0.0, -0.03, length(vec2(1.6, 1.1)*(p-vec2(-0.37, 0.24)))-0.17);
	f = max(0.0, f-smoothstep(0.0, -0.03, length(vec2(1.6, 0.9)*(p-vec2(-0.40, 0.20)))-0.17));
	col = mix(col, vec3(0.8, 0.6, 0.0) + 0.2*sin(p.y*18.0), f);
	
	// legs (they are so small, let's skip for now and see if they are needed at all)
	// [even more small squeezed circles]
}

struct Agent
{
	vec2 hash;
	vec2 pos;
	vec2 workplace;
	float lifetime; // 0 to 1
};

const float cycleDuration = 6.0;
	
Agent spawnAgent(float t, vec2 bounds) // time and bounds (+/-)
{
	float tick = floor(t/cycleDuration);
	float lifetime = t/cycleDuration - tick; // 0 to 1
	
	vec2 hash = 2.0*(rand21(tick)-0.5);
	
	vec2 begin = vec2(sign(hash.x), hash.y) * bounds;
	vec2 target = rand21(begin.y+hash.x) * bounds * 0.5 * -sign(hash);
	vec2 workplace = target;
	vec2 way = target - begin;

	float animtime = lifetime;
	if (animtime > 0.5)
	{
		animtime -= 0.5;
		begin = target;
		target = vec2(sign(way.x)*bounds.x, way.y)*1.5;
        target.y *= -1.0;
		way = target - begin;
	}
	
	vec2 pos = begin + way*(smoothstep(0., 0.3, animtime));
	pos += vec2(cos(t*(4.0+hash.x))*10.0, sin(t*(4.0+hash.y))*10.0); // variation
	
	return Agent(hash, pos, workplace, lifetime);
}

void main()
{
    time = iTime*0.8 - 2.0;
	vec2 uv = (gl_FragCoord.xy - iResolution.xy*0.5);

	#ifdef THE_BEE
		vec3 col = vec3(0.501);
		if (iMouse.z < 1.0)
		{
			bee(uv-vec2(-270.0, -150.0), 300.0, col, time, true);
			bee(uv-vec2(110.0, -200.0), 50.0, col, time, true);
			bee(uv-vec2(60.0, -130.0), 20.0, col, time, false);
			bee(abs(uv-vec2(220.0, 0.0)), 100.0, col, time, true);
		}
		else
		{
			bee(uv+vec2(iResolution.y*0.5), iResolution.y, col, time, true);
		}
	#else
		vec2 shift = vec2(100.0*sin(iTime*0.335), 100.0*sin(iTime*0.313));
		uv += shift;
	
		vec3 hex = hexCoord(uv);
		vec3 col = vec3(0.6, 0.3, 0.0)-0.2*fbm(vec2(uv.x-time*28.0, uv.y)*0.008);
		float cellHash = rand12(hex.xy);
		col = mix(col, vec3(1.0), smoothstep(0.2, 1.0, hex.b));
		col -= 0.1*cellHash;
		
		vec2 bounds = iResolution.xy*0.7; // taken +/- (from center)

		// thumbnail version
		if (iResolution.y < 250.0) 
		{
			vec2 p = vec2(iResolution.x*0.4, iResolution.y*0.4+10.0*sin(time*3.0));
			bee(uv+p-shift, iResolution.y*0.8, col, time, true); 
			p = vec2(-90.0, 20.0+10.0*sin(time*4.0));
			bee(uv+p-shift, 20.0, col, time + 10.0, false); 
		}
		else 
		{
			float num = time / 5.123;
			for (float i = 0.0; i < 80.0; i++)
			{	
				if (i > num) break; //continue; // break did not work with my old GPU
				
				float t = time+i*5.123;
				Agent a = spawnAgent(t, bounds);
				vec3 ahex = hexCoord(a.workplace + vec2(hexSize, 0.0));
				float dist = length(hex.xy - ahex.xy);
				if (dist < 5.0)	col -= 0.04*(5.0-dist)*smoothstep(0.1, 0.0, min(0.1, abs(a.lifetime-0.5)));
				bee(uv-a.pos, 20.0+220.0*(max(0.0, a.lifetime-0.5)), col, t*1.235, a.hash.x < 0.0);
			}
		}
	#endif
	
	fragColor = vec4(col,1.0);
}
