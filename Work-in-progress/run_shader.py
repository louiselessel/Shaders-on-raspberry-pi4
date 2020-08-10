import time
import demo
import pi3d

#(W, H) = (None, None) # None should fill the screen (there are edge issues)
(W, H) = (400, 400) # Windowed
SCALE = 0.25 #should have 16th the shadertoy workload

BACKGROUND_COLOR = (0.0, 0.0, 0.0, 0.0)

display = pi3d.Display.create(w=W, h=H, frames_per_second=24.0, 
                              background=BACKGROUND_COLOR,
                              display_config=pi3d.DISPLAY_CONFIG_HIDE_CURSOR | pi3d.DISPLAY_CONFIG_MAXIMIZED,
                              use_glx=True)
print(display.opengl.gl_id)
if W is None or H is None:
 (W, H) = (display.width, display.height)
 print('setting display size to ' + str(W) + ' ' + str(H))
sprite = pi3d.Triangle(corners=((-1.0, -1.0),(-1.0, 3.0),(3.0, -1.0)))

## choose shader
shader = pi3d.Shader('shaders/shadertoy01') # kaledoscope
#shader = pi3d.Shader('shaders/shadertoy02') # fire 1
#shader = pi3d.Shader('shaders/shadertoy03_fire') # fire 2
#shader = pi3d.Shader('shaders/shadertoy04') # cloud
#shader = pi3d.Shader('shaders/shadertoy05') # psychedelic --- Work in progress
sprite.set_shader(shader)

## offscreen texture stuff ##
cam = pi3d.Camera(is_3d=False)
flatsh = pi3d.Shader('post_vanilla')
post = pi3d.PostProcess(camera=cam, shader=flatsh, scale=SCALE)

kbd = pi3d.Keyboard()
sprite.unif[0:2] = [W, H] # iResolution
sprite.unif[4] = SCALE # iScale
sprite.unif[6:8] = [0.9, 0.5] # iMouse

tm0 = time.time()
while display.loop_running():
    post.start_capture()
    sprite.draw()
    post.end_capture()
    post.draw()
    sprite.unif[3] = (time.time() - tm0) * 1.0
    k = kbd.read()
    if k == 27:
        kbd.close()
        display.stop()
        break
