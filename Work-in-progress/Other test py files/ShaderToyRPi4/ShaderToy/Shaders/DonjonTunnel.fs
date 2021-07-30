//https://www.shadertoy.com/view/Xslyzf
// Created by Stephane Cuillerdier - Aiekick/2017 (twitter:@aiekick)
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;

uniform sampler2D iChannel0;
uniform sampler2D iChannel1;

out vec4 fragColor;

#define PI 3.141592654

//* FabriceNeyret2 and GregRostami 250c
void main()
{
	vec2 f=iResolution.xy;
	vec2 g = (gl_FragCoord.xy+gl_FragCoord.xy-f) / f.y;
    
    float t = iTime * .5, d, 
        X = abs(g.x),
        Y = g.y += sin(t)*.5;
    
    d = max(max(X, abs(Y)), min(X+Y, length(g)));

    g = vec2(atan(g.x,Y)/1.57,.8/d + t);
    
    fragColor = d * (d <= X || d < X+Y
            ? texture(iChannel1, g)
            : texture(iChannel0, g));
}/**/

/* 256c by GregRostami
void mainImage(out vec4 f,vec2 g)
{
    g = (g+g-(f.xy=iResolution.xy))/f.y;
    float t = iDate.w*.5, d, X= abs(g.x), y;
    g.y += sin(t)*.5;
    d = max( max(X, abs(y=g.y)), min(X+y, length(g)) );
    g = vec2( atan(g.x,y)/1.6, .8/d + t );
    
    f = d* (d <= X || d < X+y
            ? texture(iChannel1, g)
            : texture(iChannel0, g));
}/**/

/* 264c by GregRostami
void mainImage(out vec4 f,vec2 g)
{
	g = (g+g-(f.xy=iResolution.xy))/f.y;
    
    float t = iDate.w * .5,d, X= abs(g.x);
    g.y += sin(t)*.5;
    
    d = max(max(X, abs(g.y)), min(X+g.y, length(g)));
   
    vec2 k = vec2(atan(g.x,g.y)/1.57,.8/d + t);
    
    f = d* (d <= X || d < X+g.y
            ? texture(iChannel1, k)
            : texture(iChannel0, k.yx));
}/**/

/* original 306c
void mainImage( out vec4 f, vec2 g )
{
    f.xyz = iResolution;
	g = (g+g-f.xy)/f.y;
    
    float t = iTime * .5;
    g.y += sin(t)*.5;
    
    float a = atan(g.x,g.y)/1.57;
    float d = max(max(abs(g.x),abs(g.y)), min(abs(g.x)+g.y, length(g)));
   
    vec2 k = vec2(a,.8/d + t);
    
    // ground
    f = texture(iChannel0, k.yx) * d;
    
    // wall
    if (d<=abs(g.x)||d<=abs(g.x)+g.y)
        f = texture(iChannel1, k) * d;
}/**/
