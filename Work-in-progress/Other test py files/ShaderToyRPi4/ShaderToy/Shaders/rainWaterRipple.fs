//Render final image, compute some lighting using normals generated in previews buffers
//https://www.shadertoy.com/view/Mt33DH

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;

uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;

out vec4 fragColor;

float rotSpeed = 0.05;

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec2 uv = gl_FragCoord.xy / iResolution.xy;
    
    vec4 buff = texture(iChannel0, uv)*2.0-1.0;
    float z = sqrt(1.0 - clamp(dot(vec2(buff.x,buff.y), vec2(buff.x,buff.y)),0.0, 1.0));
    vec3 n = normalize(vec3(buff.x, buff.y, z));
    
    vec3 lightDir = vec3(sin(iTime*rotSpeed),cos(iTime*rotSpeed),0.0);
    
    float l = max(0.0, dot(n, lightDir));
    float fresnel = 1.0 - dot(vec3(0.0,0.0,1.0), n);
    vec4 refl = texture(iChannel2, reflect(n, lightDir));
    
    vec4 tex = texture(iChannel1, vec2(uv.x*(iResolution.x/iResolution.y), uv.y) + n.xy);
    
    fragColor = tex*0.5 + vec4((fresnel + l)*5.0)*refl + refl*0.5;
}
