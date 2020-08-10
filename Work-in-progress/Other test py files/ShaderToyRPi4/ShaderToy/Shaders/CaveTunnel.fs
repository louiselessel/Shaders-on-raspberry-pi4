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

// Created by inigo quilez - iq/2015
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.


// New WebGL 2 version (252 chars)
//
#define t texture(iChannel0,p*.1,3.
void main()
{
	vec2 p = gl_FragCoord.xy;
    vec4 q = p.xyxy/iResolution.y - .5, c=q-q;
    
    p.y = atan( q.x, q.y );
    
    for( float s=0.; s<.1; s+=.01 )
    {
        float x = length( q.xy ), z = 1.;
        
        for( ; z>0. && t).x<z ; z-=.01 )
            p.x = iTime*3. + s + 1./(x+x*z);

        fragColor = c += t*x)*z*x*.2;
    }
}

