//https://www.shadertoy.com/view/wtsSRX

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform sampler2D iChannel3;

out vec4 fragColor;

//============================================================================
// PROJECT ID:
//
// GROUP NUMBER:
//
// STUDENT NAME: 
// NUS User ID.: 
//
// STUDENT NAME: 
// NUS User ID.: 
//
// STUDENT NAME: 
// NUS User ID.: 
//
// COMMENTS TO GRADER: 
//
//============================================================================


// FRAGMENT SHADER FOR SHADERTOY
// Run this at https://www.shadertoy.com/new
// See documentation at https://www.shadertoy.com/howto

// Your browser must support WebGL 2.0.
// Check your browser at http://webglreport.com/?v=2


//============================================================================
// Constants.
//============================================================================
#define iterations 17
#define formuparam 0.53

#define volsteps 20
#define stepsize 0.1

#define zoom   0.800
#define tile   0.850
#define speed  0.010 

#define brightness 0.0015
#define darkmatter 0.300
#define distfading 0.730
#define saturation 0.850

const int NUM_LIGHTS = 2;
const int NUM_MATERIALS = 3;
const int NUM_PLANES = 2;
const int NUM_SPHERES = 3;
const float PI = 3.1415926;

const vec3 BACKGROUND_COLOR = vec3( 0.1, 0.2, 0.6 );

 // Vertical field-of-view angle of camera. In radians.
const float FOVY = 50.0 * 3.1415926535 / 180.0; 

// Use this for avoiding the "epsilon problem" or the shadow acne problem.
const float DEFAULT_TMIN = 10.0e-4;

// Use this for tmax for non-shadow ray intersection test.
const float DEFAULT_TMAX = 10.0e6;

// Equivalent to number of recursion levels (0 means ray-casting only).
// We are using iterations to replace recursions.
const int NUM_ITERATIONS = 2;


//============================================================================
// Define new struct types.
//============================================================================
struct Ray_t {
    vec3 o;  // Ray Origin.
    vec3 d;  // Ray Direction. A unit vector.
};

struct Plane_t {
    // The plane equation is Ax + By + Cz + D = 0.
    float A, B, C, D;
    int materialID;
};

struct Sphere_t {
    vec3 center;
    float radius;
    int materialID;
};

struct Light_t {
    vec3 position;  // Point light 3D position.
    vec3 I_a;       // For Ambient.
    vec3 I_source;  // For Diffuse and Specular.
};

struct Material_t {
    vec3 k_a;   // Ambient coefficient.
    vec3 k_d;   // Diffuse coefficient.
    vec3 k_r;   // Reflected specular coefficient.
    vec3 k_rg;  // Global reflection coefficient.
    float n;    // The specular reflection exponent. Ranges from 0.0 to 128.0. 
};

//----------------------------------------------------------------------------
// The lighting model used here is similar to that on Slides 8 and 12 of 
// Lecture 11 (Ray Tracing). Here it is computed as
//
//     I_local = SUM_OVER_ALL_LIGHTS { 
//                   I_a * k_a + 
//                   k_shadow * I_source * [ k_d * (N.L) + k_r * (R.V)^n ]
//               }
// and
//     I = I_local  +  k_rg * I_reflected
//----------------------------------------------------------------------------


//============================================================================
// Global scene data.
//============================================================================
Plane_t Plane[NUM_PLANES];
Sphere_t Sphere[NUM_SPHERES];
Light_t Light[NUM_LIGHTS];
Material_t Material[NUM_MATERIALS];



/////////////////////////////////////////////////////////////////////////////
// Initializes the scene.
/////////////////////////////////////////////////////////////////////////////
void InitScene()
{
    // Horizontal plane.
    /*Plane[0].A = 0.0;
    Plane[0].B = 1.0;
    Plane[0].C = 0.0;
    Plane[0].D = 0.0;
    Plane[0].materialID = 0;

    // Vertical plane.
    Plane[1].A = 0.0;
    Plane[1].B = 0.0;
    Plane[1].C = 1.0;
    Plane[1].D = 3.5;
    Plane[1].materialID = 0;*/

    // Center bouncing sphere.
    Sphere[0].center = vec3( 1.0 * cos(iTime), 1.0 * sin(iTime), 0.0 );
    Sphere[0].radius = 0.6;
    Sphere[0].materialID = 1;

    // Circling sphere.
    Sphere[1].center = vec3( 1.0 * cos(iTime + 2.0 * PI / 3.0), 1.0 * sin(iTime + 2.0 * PI / 3.0), 0.0);
    Sphere[1].radius = 0.6;
    Sphere[1].materialID = 2;
    
    Sphere[2].center = vec3( 1.0 * cos(iTime - 2.0 * PI / 3.0), 1.0 * sin(iTime - 2.0 * PI / 3.0), 0.0);
    Sphere[2].radius = 0.6;
    Sphere[2].materialID = 0;

    // Silver material.
    Material[0].k_d = vec3( 0.5, 0.5, 0.5 );
    Material[0].k_a = 0.2 * Material[0].k_d;
    Material[0].k_r = 2.0 * Material[0].k_d;
    Material[0].k_rg = 0.5 * Material[0].k_r;
    Material[0].n = 64.0;

    // Gold material.
    Material[1].k_d = vec3( 0.8, 0.7, 0.1 );
    Material[1].k_a = 0.2 * Material[1].k_d;
    Material[1].k_r = 2.0 * Material[1].k_d;
    Material[1].k_rg = 0.5 * Material[1].k_r;
    Material[1].n = 64.0;

    // Green plastic material.
    Material[2].k_d = vec3( 0.0, 0.8, 0.0 );
    Material[2].k_a = 0.2 * Material[2].k_d;
    Material[2].k_r = vec3( 1.0, 1.0, 1.0 );
    Material[2].k_rg = 0.5 * Material[2].k_r;
    Material[2].n = 128.0;

    // Light 0.
    Light[0].position = vec3( 4.0, 8.0, -3.0 );
    Light[0].I_a = vec3( 0.1, 0.1, 0.1 );
    Light[0].I_source = vec3( 1.0, 1.0, 1.0 );

    // Light 1.
    Light[1].position = vec3( -4.0, 8.0, 0.0 );
    Light[1].I_a = vec3( 0.1, 0.1, 0.1 );
    Light[1].I_source = vec3( 1.0, 1.0, 1.0 );
}



/////////////////////////////////////////////////////////////////////////////
// Computes intersection between a plane and a ray.
// Returns true if there is an intersection where the ray parameter t is
// between tmin and tmax, otherwise returns false.
// If there is such an intersection, outputs the value of t, the position
// of the intersection (hitPos) and the normal vector at the intersection 
// (hitNormal).
/////////////////////////////////////////////////////////////////////////////
bool IntersectPlane( in Plane_t pln, in Ray_t ray, in float tmin, in float tmax,
                     out float t, out vec3 hitPos, out vec3 hitNormal ) 
{
    vec3 N = vec3( pln.A, pln.B, pln.C );
    float NRd = dot( N, ray.d );
    float NRo = dot( N, ray.o );
    float t0 = (-pln.D - NRo) / NRd;
    if ( t0 < tmin || t0 > tmax ) return false;

    // We have a hit -- output results.
    t = t0;
    hitPos = ray.o + t0 * ray.d;
    hitNormal = normalize( N );
    return true;
}



/////////////////////////////////////////////////////////////////////////////
// Computes intersection between a plane and a ray.
// Returns true if there is an intersection where the ray parameter t is
// between tmin and tmax, otherwise returns false.
/////////////////////////////////////////////////////////////////////////////
bool IntersectPlane( in Plane_t pln, in Ray_t ray, in float tmin, in float tmax )
{
    vec3 N = vec3( pln.A, pln.B, pln.C );
    float NRd = dot( N, ray.d );
    float NRo = dot( N, ray.o );
    float t0 = (-pln.D - NRo) / NRd;
    if ( t0 < tmin || t0 > tmax ) return false;
    return true;
}



/////////////////////////////////////////////////////////////////////////////
// Computes intersection between a sphere and a ray.
// Returns true if there is an intersection where the ray parameter t is
// between tmin and tmax, otherwise returns false.
// If there is one or two such intersections, outputs the value of the 
// smaller t, the position of the intersection (hitPos) and the normal 
// vector at the intersection (hitNormal).
/////////////////////////////////////////////////////////////////////////////
bool IntersectSphere( in Sphere_t sph, in Ray_t ray, in float tmin, in float tmax,
                      out float t, out vec3 hitPos, out vec3 hitNormal ) 
{
    /////////////////////////////////
    // TASK: WRITE YOUR CODE HERE. //
    /////////////////////////////////
    vec3 sphere_center = sph.center;
    vec3 ray_orig = ray.o;
    vec3 relative_orig = ray_orig - sphere_center;
    float a = 1.0;
    float b = dot(ray.d , relative_orig) * 2.0;
    float c = dot(relative_orig, relative_orig ) -sph.radius * sph.radius;
    float b2_4ac = b*b - 4.0*a*c;
    if(b2_4ac<0.0)
    {
        return false; 
    }
    else
    {
        float relative_intersect_t1 = (-b + sqrt(b2_4ac))/(2.0*a);
        float relative_intersect_t2 = (-b - sqrt(b2_4ac))/(2.0*a);
        float intersect_t;
        if(relative_intersect_t1>=tmin&&relative_intersect_t1<=tmax)
        {
            if(relative_intersect_t2>= tmin&&relative_intersect_t2<=tmax)
            {
                intersect_t = min(relative_intersect_t1,relative_intersect_t2);
            }
            else
            {
                intersect_t = relative_intersect_t1;
            }
        }
        else{
             if(relative_intersect_t2>= tmin&&relative_intersect_t2<=tmax)
            {
                intersect_t = relative_intersect_t2;
            }
            else
            {
                return false;
            }
        }
        t = intersect_t;
        hitPos = ray_orig + t* ray.d;
        hitNormal = hitPos - sphere_center;
        hitNormal = normalize(hitNormal);
        return true;
    }

}



/////////////////////////////////////////////////////////////////////////////
// Computes intersection between a sphere and a ray.
// Returns true if there is an intersection where the ray parameter t is
// between tmin and tmax, otherwise returns false.
/////////////////////////////////////////////////////////////////////////////
bool IntersectSphere( in Sphere_t sph, in Ray_t ray, in float tmin, in float tmax )
{
    /////////////////////////////////
    // TASK: WRITE YOUR CODE HERE. //
    /////////////////////////////////
    vec3 sphere_center = sph.center;
    vec3 ray_orig = ray.o;
    vec3 relative_orig = ray_orig - sphere_center;
    float a = 1.0;
    float b = dot(ray.d , relative_orig) * 2.0;
    float c = dot(relative_orig, relative_orig ) -sph.radius * sph.radius;
    float b2_4ac = b*b - 4.0*a*c;
    if(b2_4ac<0.0)
    {
        return false; 
    }
    else
    {
        float relative_intersect_t1 = (-b + sqrt(b2_4ac))/(2.0*a);
        float relative_intersect_t2 = (-b - sqrt(b2_4ac))/(2.0*a);
        return ((relative_intersect_t1>=tmin&&relative_intersect_t1<=tmax) ||(relative_intersect_t2>=tmin&&relative_intersect_t2<=tmax));
    }
}


/////////////////////////////////////////////////////////////////////////////
// Computes (I_a * k_a) + k_shadow * I_source * [ k_d * (N.L) + k_r * (R.V)^n ].
// Input vectors L, N and V are pointing AWAY from surface point.
// Assume all vectors L, N and V are unit vectors.
/////////////////////////////////////////////////////////////////////////////
vec3 PhongLighting( in vec3 L, in vec3 N, in vec3 V, in bool inShadow, 
                    in Material_t mat, in Light_t light )
{
    if ( inShadow ) {
        return light.I_a * mat.k_a;
    }
    else {
        vec3 R = reflect( -L, N );
        float N_dot_L = max( 0.0, dot( N, L ) );
        float R_dot_V = max( 0.0, dot( R, V ) );
        float R_dot_V_pow_n = ( R_dot_V == 0.0 )? 0.0 : pow( R_dot_V, mat.n );

        return light.I_a * mat.k_a + 
               light.I_source * (mat.k_d * N_dot_L + mat.k_r * R_dot_V_pow_n);
    }
}


/////////////////////////////////////////////////////////////////////////////
// Casts a ray into the scene and returns color computed at the nearest
// intersection point. The color is the sum of light from all light sources,
// each computed using Phong Lighting Model, with consideration of
// whether the interesection point is being shadowed from the light.
// If there is no interesection, returns the background color, and outputs
// hasHit as false.
// If there is intersection, returns the computed color, and outputs
// hasHit as true, the 3D position of the intersection (hitPos), the
// normal vector at the intersection (hitNormal), and the k_rg value
// of the material of the intersected object.
/////////////////////////////////////////////////////////////////////////////
vec3 CastRay( in Ray_t ray, 
              out bool hasHit, out vec3 hitPos, out vec3 hitNormal, out vec3 k_rg ) 
{
    // Find whether and where the ray hits some object. 
    // Take the nearest hit point.

    bool hasHitSomething = false;
    float nearest_t = DEFAULT_TMAX;   // The ray parameter t at the nearest hit point.
    vec3 nearest_hitPos;              // 3D position of the nearest hit point.
    vec3 nearest_hitNormal;           // Normal vector at the nearest hit point.
    int nearest_hitMatID;             // MaterialID of the object at the nearest hit point.

    float temp_t;
    vec3 temp_hitPos;
    vec3 temp_hitNormal;
    bool temp_hasHit;

    /////////////////////////////////////////////////////////////////////////////
    // TASK:
    // * Try interesecting input ray with all the planes and spheres,
    //   and record the front-most (nearest) interesection.
    // * If there is interesection, need to record hasHitSomething,
    //   nearest_t, nearest_hitPos, nearest_hitNormal, nearest_hitMatID.
    /////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////
    // TASK: WRITE YOUR CODE HERE. 
    /////////////////////////////////
    for( int i =0;i<NUM_PLANES;i++)
    {
        Plane_t pln = Plane[i];
        temp_hasHit = IntersectPlane(pln, ray, DEFAULT_TMIN, DEFAULT_TMAX, temp_t, temp_hitPos, temp_hitNormal);
        if(temp_hasHit)
        {
            hasHitSomething = true;
            if(temp_t<= nearest_t)
            {
            nearest_t = temp_t;
            nearest_hitPos = temp_hitPos;
            nearest_hitNormal = temp_hitNormal;
            nearest_hitMatID = pln.materialID;
            }
        } 
    }
    for( int i =0;i<NUM_SPHERES;i++)
    {
        Sphere_t sphere = Sphere[i];
        temp_hasHit = IntersectSphere(sphere, ray, DEFAULT_TMIN, DEFAULT_TMAX, temp_t, temp_hitPos, temp_hitNormal);
        if(temp_hasHit)
        {
            if(temp_t<= nearest_t)
            {
            hasHitSomething = true;
            nearest_t = temp_t;
            nearest_hitPos = temp_hitPos;
            nearest_hitNormal = temp_hitNormal;
            nearest_hitMatID = sphere.materialID;
            }
        }    
        
        
    }
    // One of the output results.
    hasHit = hasHitSomething;
    if ( !hasHitSomething ) return BACKGROUND_COLOR;

    vec3 I_local = vec3( 0.0 );  // Result color will be accumulated here.

    /////////////////////////////////////////////////////////////////////////////
    // TASK:
    // * Accumulate lighting from each light source on the nearest hit point. 
    //   They are all accumulated into I_local.
    // * For each light source, make a shadow ray, and check if the shadow ray
    //   intersects any of the objects (the planes and spheres) between the 
    //   nearest hit point and the light source.
    // * Then, call PhongLighting() to compute lighting for this light source.
    /////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////
    // TASK: WRITE YOUR CODE HERE. //
    /////////////////////////////////
    for(int i=0;i<NUM_LIGHTS;i++)
    {
        Light_t light = Light[i];
        vec3 shadowRay = light.position - nearest_hitPos ;
        Ray_t sRay ;
        sRay.o = nearest_hitPos;
        sRay.d = normalize(shadowRay);
        bool hitObject = false;
        // for( int i =0;i<NUM_PLANES && (!hitObject);i++)
        // {
        //     Plane_t pln = Plane[i];
        //     temp_hasHit = IntersectPlane(pln, sRay, DEFAULT_TMIN, DEFAULT_TMAX);
        //     if(temp_hasHit)
        //         hitObject = true;
        // }
        for( int i =0;i<NUM_SPHERES && (!hitObject);i++)
        {
            Sphere_t sphere = Sphere[i];
            temp_hasHit = IntersectSphere(sphere, sRay, DEFAULT_TMIN, DEFAULT_TMAX);
            if(temp_hasHit)
                hitObject = true;
        }
        //vec3 PhongLighting( in vec3 L, in vec3 N, in vec3 V, in bool inShadow, 
                   // in Material_t mat, in Light_t light )
        I_local += PhongLighting(normalize(shadowRay), normalize(nearest_hitNormal), -ray.d, hitObject, Material[nearest_hitMatID], light);
        // I_local += PhongLighting(-ray.d, normalize(nearest_hitNormal), normalize(-shadowRay),hitObject, Material[nearest_hitMatID], light);
    }


    // Populate output results.
    hitPos = nearest_hitPos;
    hitNormal = nearest_hitNormal;
    k_rg = Material[nearest_hitMatID].k_rg;

    return I_local;
}


float sphDistance( in vec3 ro, in vec3 rd, in Sphere_t sph )
{
	vec3 oc = ro - sph.center;
    float b = dot( oc, rd );
    float c = dot( oc, oc ) - sph.radius * sph.radius;
    float h = b*b - c;
    float d = sqrt( max(0.0, sph.radius * sph.radius - h)) - sph.radius;
    return d;
}


vec3 render( in vec3 ro, in vec3 rd )
{
    vec3 col = vec3(0.0, 0.0, 0.0);
    
	float d = min(min(sphDistance( ro, rd, Sphere[0] ), sphDistance( ro, rd, Sphere[1] )), sphDistance( ro, rd, Sphere[2]));//#
    vec3 glo = vec3(0.0);
    glo += vec3(0.6,0.7,1.0) * 0.3 * exp(-2.0*abs(d)) * step(0.0,d);
    glo += 0.6 * vec3(0.6,0.7,1.0) * 0.3 * exp(-8.0 * abs(d));
    glo += 0.6 * vec3(0.8,0.9,1.0) * 0.4 * exp(-100.0 * abs(d));
    col += glo * 2.0;              
    return col;
}


/////////////////////////////////////////////////////////////////////////////
// Execution of fragment shader starts here.
// 1. Initializes the scene.
// 2. Compute a primary ray for the current pixel (fragment).
// 3. Trace ray into the scene with NUM_ITERATIONS recursion levels.
/////////////////////////////////////////////////////////////////////////////
void main()
{
    InitScene();

    // Scale pixel 2D position such that its y coordinate is in [-1.0, 1.0].
    vec2 pixel_pos = (2.0 * gl_FragCoord.xy - iResolution.xy) / iResolution.y;

    // Position the camera.
    vec3 cam_pos = vec3( 5.0 * cos(iTime), 0.0, 5.0 * sin(iTime) + 5.0 );
    //vec3 cam_pos = vec3( 5.0, 0.0, 10.0 );
    vec3 cam_lookat = vec3(0.0, 0.0, 0.0 );
    vec3 cam_up_vec = vec3( 0.0, 1.0, 0.0 );

    // Set up camera coordinate frame in world space.
    vec3 cam_z_axis = normalize( cam_pos - cam_lookat );
    vec3 cam_x_axis = normalize( cross(cam_up_vec, cam_z_axis) );
    vec3 cam_y_axis = normalize( cross(cam_z_axis, cam_x_axis));

    // Create primary ray.
    float pixel_pos_z = -1.0 / tan(FOVY / 2.0);
    Ray_t pRay;
    pRay.o = cam_pos;
    pRay.d = normalize( pixel_pos.x * cam_x_axis  +  pixel_pos.y * cam_y_axis  +  pixel_pos_z * cam_z_axis );


    // Start Ray Tracing.
    // Use iterations to emulate the recursion.

    vec3 I_result = vec3( 0.0 );
    vec3 compounded_k_rg = vec3( 1.0 );
    Ray_t nextRay = pRay;

    for ( int level = 0; level <= NUM_ITERATIONS; level++ ) 
    {
        bool hasHit;
        vec3 hitPos, hitNormal, k_rg;

        vec3 I_local = CastRay( nextRay, hasHit, hitPos, hitNormal, k_rg );

        I_result += compounded_k_rg * I_local;

        if ( !hasHit ) break;

        compounded_k_rg *= k_rg;

        nextRay = Ray_t( hitPos, normalize( reflect(nextRay.d, hitNormal) ) );
    }
    
    
    //universe
    //get coords and direction
	vec2 uv=gl_FragCoord.xy/iResolution.xy-.5;
	uv.y*=iResolution.y/iResolution.x;
	vec3 dir=vec3(uv*zoom,1.);
	float time=iTime*speed+.25;

	//mouse rotation
	float a1=.5+iMouse.x/iResolution.x*2.;
	float a2=.8+iMouse.y/iResolution.y*2.;
	mat2 rot1=mat2(cos(a1),sin(a1),-sin(a1),cos(a1));
	mat2 rot2=mat2(cos(a2),sin(a2),-sin(a2),cos(a2));
	dir.xz*=rot1;
	dir.xy*=rot2;
	vec3 from=vec3(1.,.5,0.5);
	from+=vec3(time*2.,time,-2.);
	from.xz*=rot1;
	from.xy*=rot2;
	
	//volumetric rendering
	float s=0.1, fade=1.;
	vec3 v=vec3(0.);
	for (int r=0; r<volsteps; r++) {
		vec3 p=from+s*dir*.5;
		p = abs(vec3(tile)-mod(p,vec3(tile*2.))); // tiling fold
		float pa,a=pa=0.;
		for (int i=0; i<iterations; i++) { 
			p=abs(p)/dot(p,p)-formuparam; // the magic formula
			a+=abs(length(p)-pa); // absolute sum of average change
			pa=length(p);
		}
		float dm=max(0.,darkmatter-a*a*.001); //dark matter
		a*=a*a; // add contrast
		if (r>6) fade*=1.-dm; // dark matter, don't render near
		//v+=vec3(dm,dm*.5,0.);
		v+=fade;
		v+=vec3(s,s*s,s*s*s*s)*a*brightness*fade; // coloring based on distance
		fade*=distfading; // distance fading
		s+=stepsize;
	}
	v=mix(vec3(length(v)),v,saturation); //color adjust
    
    //light circle
    vec3 col = render( pRay.o, pRay.d );

    fragColor = vec4( I_result + v*.01 + col, 1.0 );
    //fragColor = vec4( col, 1.0 );
}
