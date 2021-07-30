
#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
out vec4 fragColor;

vec4 T( vec2 u )
{
    u += iTime/vec2(49,5);
/*    return 1.-abs(sin(texture(iChannel0,u).x*3.-u.y/9.))
        	+ texture(iChannel1,u*1.3)*.1;*/
    return pow(1.-texture(iChannel0,u).x,3.)
        	+ texture(iChannel1,u*1.3)*.1;
}

void main()
{
    vec2 s = gl_FragCoord.xy/iResolution.y-vec2(.5,-.8);
    s /= 18.-s.y/.1;
	
	vec4 o;
    for ( float i=1.; i > 0.; i -= .002 )
        o = T( o.x<i ? s /= .9987 : s );
    
//    o = o*9.5-T(s+.001).x*9.+.2+s.y*.1;
    fragColor = exp2(-s.y*.3)*(o*9.5-T(s+.001).x*9.-.7)+.9; // nicer fog
}
