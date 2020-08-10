//https://www.shadertoy.com/view/MsXBRX

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;

uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;

out vec4 fragColor;

const float width = 0.37;
const float height = 1.;


vec2 B(vec2 uv) {
	vec2 uv1= vec2(uv.x-iTime,uv.y-iTime/2.);
	vec3 rain=texture(iChannel0,uv1).rgb/8.;
    vec2 uv2 = uv.xy-rain.xy/4.;
    uv2.x -= iTime/2.;
    uv2.y -= iTime/5.;
    return texture(iChannel0,uv2).xy;
}

void main()
{
	vec2 uv = gl_FragCoord.xy / iResolution.xy;
    vec2 uv_= uv;
    vec4 fuera = texture(iChannel2,uv);
    vec2 uve = uv.xy-B(uv);
    vec2 uvmd = uve+sin(uv_.y*uv_.y/10.+1.)*1.03*B(uv_);
    vec4 centro = texture(iChannel2, uvmd);
	vec4 fondo = texture(iChannel1,uv-B(uv));

	uv = vec2(0.5, 0.5) - uvmd;
    uv.x *= iResolution.x / iResolution.y; 
    float hypot = sqrt(uv.x * uv.x + uv.y * uv.y);
    float uvang = atan(uv.y, uv.x);
    vec2 rot = vec2(hypot * cos(uvang+1.4), hypot * sin(uvang+0.1));
    if(rot.x <= width && rot.x >= -width && rot.y <= height && rot.y >= -height){
    	fragColor = fondo;
    } else if(rot.x-0.1 <= width && rot.x+0.1 >= -width && rot.y <= height && rot.y >= -height) {
        fragColor = centro;
    } else {
    	fragColor=fuera;
    }
}
