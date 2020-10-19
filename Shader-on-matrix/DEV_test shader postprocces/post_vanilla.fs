#include std_head_fs.inc

varying vec2 texcoordout;

void main(void) {
 gl_FragColor = texture2D(tex0, texcoordout);
 gl_FragColor.a = 1.0;
}
