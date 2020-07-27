import time
import demo
import pi3d

display = pi3d.Display.create(w=800, h=600)
print(display.opengl.gl_id)

sprite = pi3d.Triangle(corners=((-1.0, -1.0),(-1.0, 3.0),(3.0, -1.0)))
shader = pi3d.Shader('shaders/shadertoy01')
sprite.set_shader(shader)
kbd = pi3d.Keyboard()

tm0 = time.time()
while display.loop_running():
    sprite.draw()
    sprite.unif[3] = time.time() - tm0
    sprite.unif[0:2] = [800, 600]
    if kbd.read() == 27:
        kbd.close()
        display.stop()
        break
