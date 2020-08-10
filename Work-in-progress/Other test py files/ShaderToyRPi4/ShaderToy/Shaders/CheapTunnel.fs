//https://www.shadertoy.com/view/MstyRS

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform sampler2D iChannel0;
out vec4 fragColor;

// 108 chars - Fabrice did ... you know ... what only Fabrice can do! :)
/*
#define mainImage(o,u)           \
	o = texture( iChannel0, vec2(iTime + .3/length(o.xy=u/iResolution.y-.7), \
                                 atan(o.y, o.x)/3.14) )
*/
        
        
// 112 chars - My favroite version.
/*
#define mainImage(o,U)           \
    vec2 u = U/iResolution.y-.7; \
	o = texture( iChannel0, vec2(atan(u.y, u.x)/3.14, iTime + .3/length(u)) )
*/


// 107 chars - Shorter, but not my favorite.
/*
#define mainImage(o,U)           \
    vec2 u = U/iResolution.y-.7; \
	o = texture( iChannel0, vec2(atan(u.y, u.x), iTime + .3/length(u)) )
*/


// 391 chars - Original shader by Falken:

const float TUNNEL_SIZE  = 0.25;	// smaller values for smaller/thinner tunnel
const float TUNNEL_SPEED = 3.;		// speed of tunnel effect, negative values ok

const float PI = 3.141592;

vec2 tunnel(vec2 uv, float size, float time)
{
    vec2 p  = -1.0 + (2.0 * uv);
    float a = atan(p.y, p.x);
    float r = sqrt(dot(p, p));
    return vec2(a / PI, time + (size / r));
}

void main()
{
	vec2 uv = gl_FragCoord.xy / iResolution.xy;
    uv = tunnel(uv, TUNNEL_SIZE, iTime * TUNNEL_SPEED);
	fragColor = texture(iChannel0, uv);
}


