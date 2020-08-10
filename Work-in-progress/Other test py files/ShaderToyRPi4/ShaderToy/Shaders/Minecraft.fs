//https://www.shadertoy.com/view/4tsGD7
#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
out vec4 fragColor;

// [2TC 15] Minecraft. Created by Reinder Nijhoff 2015
// @reindernijhoff
//
// https://www.shadertoy.com/view/4tsGD7
// 

void main() {
    vec3 d = vec3(gl_FragCoord.xy/iResolution.xy-.5,1.), p, c, f, g=d, o, y=vec3(1,2,0);
 	o.y = 3.*cos((o.x=.3)*(o.z=iTime));

    for( float i=.0; i<9.; i+=.01 ) {
        f = fract(c = o += d*i*.01), p = floor( c )*.3;
        if( cos(p.z) + sin(p.x) > ++p.y ) {
	    	g = (f.y-.04*cos((c.x+c.z)*40.)>.8?y:f.y*y.yxz) / i;
            break;
        }
    }
    fragColor = vec4(g,1.);
}
