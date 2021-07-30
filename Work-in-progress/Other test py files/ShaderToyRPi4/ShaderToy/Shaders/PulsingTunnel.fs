
//https://www.shadertoy.com/view/lsfyzl
/*
* License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
* Created by bal-khan
*/


#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

out vec4 fragColor;

float 	t; // time
float	a; // angle used both for camera path and distance estimator
float	id_t; // id used for coloring

#define I_MAX		100
#define E			0.001

#define	CAM_PATH // pretty slopy (pun intended)
//#define	LIGHTS
//#define	BAD_TRIP // 30 seconds animation (approximatively)
//#define	LOOKING_AROUND
//#define	PERF_COLS // only one pair of fancy colors, 6 less sin calls

#define		FWD_SPEED	-7.	// the speed at wich the tunnel travel towards you

vec2	march(vec3 pos, vec3 dir);
vec3	camera(vec2 uv);
vec2	rot(vec2 p, vec2 ang);
void	rotate(inout vec2 v, float angle);
vec3	calcNormal( in vec3 pos, float e, vec3 dir);

// blackbody by aiekick : https://www.shadertoy.com/view/lttXDn

// -------------blackbody----------------- //

// return color from temperature 
//http://www.physics.sfasu.edu/astro/color/blackbody.html
//http://www.vendian.org/mncharity/dir3/blackbody/
//http://www.vendian.org/mncharity/dir3/blackbody/UnstableURLs/bbr_color.html

vec3 blackbody(float Temp)
{
	vec3 col = vec3(255.);
    col.x = 56100000. * pow(Temp,(-3. / 2.)) + 148.;
   	col.y = 100.04 * log(Temp) - 623.6;
   	if (Temp > 6500.) col.y = 35200000. * pow(Temp,(-3. / 2.)) + 184.;
   	col.z = 194.18 * log(Temp) - 1448.6;
   	col = clamp(col, 0., 255.)/255.;
    if (Temp < 1000.) col *= Temp/1000.;
   	return col;
}

// -------------blackbody----------------- //

void main()
{
    t  = iTime*.125;
    vec3	col = vec3(0., 0., 0.);
	vec2 R = iResolution.xy,
          uv  = vec2(gl_FragCoord.xy-R/2.) / R.y;
	vec3	dir = camera(uv);
    vec3	pos = vec3(.0, .0, .0);

    pos.z = t*FWD_SPEED;

    #ifdef	LOOKING_AROUND
    dir.zy *= mat2(cos(t*.5),sin(t*.5),-sin(t*.5),cos(t*.5) );
    dir.xy *= mat2(cos(1.57+t*.5),sin(1.57+t*.5),-sin(1.57+t*.5),cos(1.57+t*.5) );
    #endif
    
    vec2	inter = (march(pos, dir));

    // coloring (empiricism == power)
    #ifndef PERF_COLS
    col.xyz = step(id_t, 0.)*blackbody( ( inter.y-.0251*inter.x ) * 500. );
    col.xyz += step(1.,id_t)*vec3(abs(sin(t+1.04)), abs(sin(t+2.09)), abs(sin(t+3.14)))*inter.x*.01; // .01 == 1./float(I_MAX)
    #else
    col.xyz = step(id_t, 0.)*blackbody( ( inter.y-.0251*inter.x ) * 500. )*inter.x*.01*vec3(0.866555, 0.001592, 0.865759);
    col.xyz += step(1.,id_t)*vec3(0.865759, 0.866555, 0.001592)*inter.x*.01;
    #endif

    
    /*
	* lighting originally taken from gltracy : https://www.shadertoy.com/view/XsB3Rm
	*/
	#ifdef	LIGHTS
    if (inter.y <= 30.)
	{
        vec3	v = pos+inter.y*dir;
        vec3	n = calcNormal(v, E*.1, dir);
        vec3	ev = normalize(v - pos);
		vec3	ref_ev = reflect(ev, n);
        vec3	light_pos   = pos+vec3(0., 0., -100.0);
        vec3	vl = normalize(light_pos - v);
		float	diffuse  = max(.0, dot(vl, n));
		float	specular = pow(max(.0, dot(vl, ref_ev)), 40.);
        col.xyz += ( (specular + diffuse) * vec3(.25, .25, .25));
    }
    #endif
    fragColor =  vec4(col,1.0);
}

float	de_0(vec3 p)
{
	float	mind = 1e5;
	vec3	pr = p;

    // rotate x and y based on z and time
	rotate(pr.xy, a);

    // rotate y and z to skew the grid a bit in our face
	p.yz *= mat2(0., 1., -1., 0.);

    // take the fractional part of the ray (p), 
    // and offset it to get a range from [0.,1.] to [-.5, .5]
    // this is a space partitioning trick I saw on "Data Transfer" by srtuss : https://www.shadertoy.com/view/MdXGDr
	pr.xyz = fract(pr.xyz);
	pr -= .5;
    
    // magic numbers : .666 == 2/3, 2.09 == 2*(3.14/3), 4.18 == 4*(3.14/3)
    // dephasing is needed in order to get the lattice
    pr.y *= sin(t*.666     +p.z+p.y-p.x);
    pr.x *= sin(t*.666+2.09+p.z+p.y-p.x);
    pr.z *= sin(t*.666+4.18+p.z+p.y-p.x);

    mind = length(pr.yyxx)-.65025; // this is the grid
	id_t = mind;
    mind = min(mind, (length(pr.xyz)-.65025 ) ); // this is the blobs/stripes/thingys
    id_t = (id_t != mind)? 1. : 0. ; // used for coloring
	return (mind);
}

float	de_1(vec3 p) // cylinder
{
	float	mind = 1e5;
	vec3	pr = p;	
	vec2	q;
    
	q = vec2(length(pr.yx) - 4., pr.z );

    q.y = rot(q.xy, vec2(-1.+sin(t*10.)*6., 0.)).x;

	mind = length(q) - 4.5;
    #ifdef	BAD_TRIP
    #undef	CAM_PATH
    mind -= (sin(t*.75) );
	#endif

	return mind;
}

// add 2 distances to constraint the de_0 to a cylinder
float	de_2(vec3 p)
{
    return (de_0(p)-de_1(p)*.125);
}

float	scene(vec3 p)
{  
    float	mind = 1e5;
    #ifdef	CAM_PATH
    a = ( .8*(p.y*.015 + p.x*.015 + p.z *.15)  + t*3.);
    vec2	rot = vec2( cos(a+t), sin(a+t) );
    #else
    a = ( .8*(p.y*.015 + p.x*.015 + p.z *.15)  + t*3.);
    vec2	rot = vec2( cos(a), sin(a) );
    #endif

   	p.x += rot.x*4.;
	p.y += rot.y*4.;

	mind = de_2(p);
	
    return (mind);
}


vec2	march(vec3 pos, vec3 dir)
{
    vec2	dist = vec2(0.0, 0.0);
    vec3	p = vec3(0.0, 0.0, 0.0);
    vec2	s = vec2(0.0, 0.0);

    for (int i = -1; i < I_MAX; ++i)
    {
    	p = pos + dir * dist.y;
        dist.x = scene(p);
        dist.y += dist.x;
        if (dist.x < E || dist.y > 30.)
            break;
        s.x++;
    }
    s.y = dist.y;
    return (s);
}

// Utilities

void rotate(inout vec2 v, float angle)
{
	v = vec2(cos(angle)*v.x+sin(angle)*v.y,-sin(angle)*v.x+cos(angle)*v.y);
}

vec2	rot(vec2 p, vec2 ang)
{
	float	c = cos(ang.x);
    float	s = sin(ang.y);
    mat2	m = mat2(c, -s, s, c);
    
    return (p * m);
}

vec3	camera(vec2 uv)
{
    float		fov = 1.;
	vec3		forw  = vec3(0.0, 0.0, -1.0);
	vec3    	right = vec3(1.0, 0.0, 0.0);
	vec3    	up    = vec3(0.0, 1.0, 0.0);

    return (normalize((uv.x) * right + (uv.y) * up + fov * forw));
}

vec3 calcNormal( in vec3 pos, float e, vec3 dir)
{
    vec3 eps = vec3(e,0.0,0.0);

    return normalize(vec3(
           march(pos+eps.xyy, dir).y - march(pos-eps.xyy, dir).y,
           march(pos+eps.yxy, dir).y - march(pos-eps.yxy, dir).y,
           march(pos+eps.yyx, dir).y - march(pos-eps.yyx, dir).y ));
}
