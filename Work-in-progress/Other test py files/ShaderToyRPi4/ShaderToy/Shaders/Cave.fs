// Created by inigo quilez - iq/2014
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

#define HSAMPLES 100    // try 512
#define MSAMPLES   8    // try  12

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2  p = (-iResolution.xy+2.0*fragCoord.xy)/iResolution.y;
    float t =  12.25 + iTime;
	
    float an = 0.2*sin( -0.5*t );
    float co = cos( an ), si = sin( an );
    p = mat2(co,-si,si,co) * p;
    
    float ra = texture( iChannel1, fragCoord.xy/iChannelResolution[1].xy ).x;
    
    vec3 tot = vec3(0.0);
    for( int j=0; j<MSAMPLES; j++ )
    {
        float time = t + 0.5*(1.0/24.0)*(float(j)+ra)/float(MSAMPLES);
        vec2  offset = time*vec2(0.03,0.015);
	
        vec3 uv;
        for( int i=0; i<HSAMPLES; i++ )
        {
            uv.z = (float(i)+ra)/float(HSAMPLES-1);
            uv.xy = offset + vec2(p.x,1.0)/abs(p.y) * (0.001+0.0125*uv.z) * 0.5 + sign(p.y)*0.1;
            if( texture( iChannel0, uv.xy ).x < uv.z )
                break;
        }
    
        vec2  uv2 = offset + vec2(p.x-0.04,1.0)/abs(p.y) * (0.001+0.0125*uv.z) * 0.5 + sign(p.y)*0.1;
        float dif = clamp( texture(iChannel0, uv.xy).x - texture(iChannel0, uv2.xy).x, 0.0, 1.0 );
        vec3  col = vec3(2.0);
        col *= 0.2+0.9*texture( iChannel0, 24.0*uv.xy, 0.0 ).xyz;
        col *= 0.5+0.5*texture( iChannel0, 128.0*uv.xy, 0.0 ).xyz;
        col *= 1.0-1.0*uv.z;
        col *= vec3(0.4,0.56,0.7)*0.7 + vec3(16.0,9.0,3.0)*dif;
        col *= clamp(3.0*abs(p.y)  - 0.6*uv.z + 0.1,0.0,2.0);
        tot += col;
    }
    tot /= float(MSAMPLES);
 
    fragColor = vec4( tot*smoothstep(0.0,2.0,iTime), 1.0 );
}
