#version 300 es

in vec3 a_Position;

void main() {
    gl_Position = vec4(a_Position, 1.0);
}
