#ifdef GL_ES
precision mediump float;
#endif

uniform vec3 unif[20]; 
// is a vec3 so there is three spaces to place a uniform
// for some reason they are in array place index 16
// 16,0 is uniform0
// 16,1 is uniform1
// 16,2 is uniform2

void main() {
	gl_FragColor = vec4(1.0,0.0,0.0,1.0);
}
