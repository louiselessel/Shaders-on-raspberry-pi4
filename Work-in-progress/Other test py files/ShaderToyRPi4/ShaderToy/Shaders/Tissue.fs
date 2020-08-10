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

//TEXTURE0:rocks.jpg
//TEXTURE1:noise.png

// Created by inigo quilez - iq/2014
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

#define HSAMPLES 128
#define MSAMPLES   8


void main()
{
    vec4 ran = texture( iChannel1, iTime*vec2(0.11,0.23)+gl_FragCoord.xy/iResolution.xy );

	vec2  p = (-iResolution.xy+2.0*(gl_FragCoord.xy+ran.xy-0.5))/iResolution.y;
    float t =  iTime + 10.0*iMouse.x/iResolution.x;

    float dof = dot( p, p );

    vec3 tot = vec3(0.0);
    for( int j=0; j<MSAMPLES; j++ )
    {
        float mnc = (float(j)+ran.z)/float(MSAMPLES);
        float tim = t + 0.5*(1.0/24.0)*(float(j)+ran.w)/float(MSAMPLES);
        vec2  off = vec2( 0.2*tim, 0.2*sin(tim*0.2) );
 	
	    vec2 q = p + dof*0.03*mnc*vec2(cos(15.7*mnc),sin(15.7*mnc));
        vec2 r = vec2( length(q), 0.5+0.5*atan(q.y,q.x)/3.1416 );

        vec3 uv;
        for( int i=0; i<HSAMPLES; i++ )
        {
            uv.z = (float(i)+ran.x)/float(HSAMPLES-1);
            uv.xy = off + vec2( 0.2/(r.x*(1.0-0.6*uv.z)), r.y );
            if( textureLod( iChannel0, uv.xy, 0.0 ).x < uv.z )
                break;
        }
    
        vec2  uv2 = uv.xy + vec2(0.02,0.0);
        float dif = clamp( 8.0*(textureLod(iChannel0, uv.xy, 0.0).x - textureLod(iChannel0, uv2.xy, 0.0).x), 0.0, 1.0 );
        vec3  col = vec3(1.0);
        col *= 1.0-textureLod( iChannel0, 1.0*uv.xy, 0.0 ).xyz;
        col = mix( col*1.2, 1.5*textureLod( iChannel0, vec2(uv.x*0.4,0.1*sin(2.0*uv.y*3.1316)), 0.0 ).yzx, 1.0-0.7*col );
        col = mix( col, vec3(0.2,0.1,0.1), 0.5-0.5*smoothstep( 0.0, 0.3, 0.3-0.8*uv.z + texture( iChannel0, 2.0*uv.xy + uv.z ).x ) );      
        col *= 1.0-1.3*uv.z;
        col *= 1.3-0.2*dif;        
        col *= exp(-0.35/(0.0001+r.x));
        
        tot += col;
    }
    tot /= float(MSAMPLES);
 
    tot.x += 0.05;
    tot = 1.05*pow( tot, vec3(0.6,1.0,1.0) );
    
    fragColor = vec4( tot*smoothstep(0.0,2.0,iTime), 1.0 );
}
