// raymarching loop based on "[SH17A] Lava Planet" by P_Malin (https://www.shadertoy.com/view/ldBfDR)
// https://www.shadertoy.com/view/Md2fWz

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform sampler2D iChannel3;

out vec4 Y;

#define T texture( iChannel0, u * .07 )

void main()
{	
	vec2 u = gl_FragCoord.xy;
    vec3 d = vec3( .45 * u / iResolution.y, 1 ) - .5, 
         p = d;
    
    float i = 0.,
          t = .1 * iTime;
    
    d /= d.y;
    u = .5 * d.xz - 3. * t;

    for ( Y = 3e2 * T; i++ < 1e3; u = p.xz - t )
        p = p.y < T.x ? Y += T, d * i / 1e3 : p;

    Y = Y / 8e1 + texture( iChannel1, u );
    u = 5. * ( u - t );
    Y += T + .3 * p.zzxz;
    Y *= Y * Y / 6e2;
}