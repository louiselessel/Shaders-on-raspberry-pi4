import time
import demo
import pi3d

## This code was made with help from paddywwoof

#(W, H) = (None, None) # None should fill the screen (there are edge issues)
(W, H) = (400, 400) # None should fill the screen (there are edge issues)
SCALE = 0.8 #should have 16th the shadertoy workload

BACKGROUND_COLOR = (1.0, 0.0, 0.0, 0.0)

display = pi3d.Display.create(w=W, h=H, frames_per_second=30.0, background=BACKGROUND_COLOR)
print(display.opengl.gl_id)
if W is None or H is None:
 (W, H) = (display.width, display.height)
sprite = pi3d.Triangle(corners=((-1.0, -1.0),(-1.0, 3.0),(3.0, -1.0)))
shader = pi3d.Shader('shadertoy01')
sprite.set_shader(shader)

## offscreen texture stuff ##
cam = pi3d.Camera(is_3d=False)
flatsh = pi3d.Shader('post_vanilla')
post = pi3d.PostProcess(camera=cam, shader=flatsh, scale=SCALE)

kbd = pi3d.Keyboard()
sprite.unif[0:2] = [W, H]
sprite.unif[4] = SCALE
tm0 = time.time()

while display.loop_running():
    post.start_capture()
    sprite.draw()
    post.end_capture()
    post.draw()
    sprite.unif[3] = time.time() - tm0
    k = kbd.read()
    if k == 27:
        kbd.close()
        display.stop()
        break
