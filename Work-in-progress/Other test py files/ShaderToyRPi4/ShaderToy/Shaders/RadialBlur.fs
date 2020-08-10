// Created by inigo quilez - iq/2013
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

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

vec3 deform( in vec2 p )
{
    vec2 q = sin( vec2(1.1,1.2)*iTime + p );

    float a = atan( q.y, q.x );
    float r = sqrt( dot(q,q) );

    vec2 uv = p*sqrt(1.0+r*r);
    uv += sin( vec2(0.0,0.6) + vec2(1.0,1.1)*iTime);
         
    return texture( iChannel0, uv*0.3).yxx;
}

void main()
{
    vec2 p = -1.0 + 2.0*gl_FragCoord.xy/iResolution.xy;

    vec3  col = vec3(0.0);
    vec2  d = (vec2(0.0,0.0)-p)/64.0;
    float w = 1.0;
    vec2  s = p;
    for( int i=0; i<64; i++ )
    {
        vec3 res = deform( s );
        col += w*smoothstep( 0.0, 1.0, res );
        w *= .99;
        s += d;
    }
    col = col * 3.5 / 64.0;

	fragColor = vec4( col, 1.0 );
}
