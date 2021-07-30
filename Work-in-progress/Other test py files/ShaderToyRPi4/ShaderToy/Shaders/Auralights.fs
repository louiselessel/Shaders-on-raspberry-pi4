
#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;

out vec4 fragColor;


float speed=4.;

float blob(float x,float y,float fx,float fy){
   float xx = x+sin(iTime*fx/speed)*.7;
   float yy = y+cos(iTime*fy/speed)*.7;

   return 20.0/sqrt(xx*xx+yy*yy);
}

void main() {
   vec2 position = ( gl_FragCoord.xy / iResolution.xy )-0.5;

   float x = position.x*2.0;
   float y = position.y*2.0;

   float a = blob(x,y,3.3,3.2) + blob(x,y,3.9,3.0);
   float b = blob(x,y,3.2,2.9) + blob(x,y,2.7,2.7);
   float c = blob(x,y,2.4,3.3) + blob(x,y,2.8,2.3);
   
   vec3 d = vec3(a,b,c)/60.0;
   
   fragColor = vec4(d.x,d.y,d.z,1.0);
}
