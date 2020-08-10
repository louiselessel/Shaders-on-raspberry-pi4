// Created by inigo quilez - iq/2015
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

void mainImage( out vec4 c, vec2 p )
{
    c.w = length(p = p/iResolution.y - .5);
    c = texture( iChannel0, vec2(atan(p.y,p.x), .2/c.w)+iTime )*c.w;
}
