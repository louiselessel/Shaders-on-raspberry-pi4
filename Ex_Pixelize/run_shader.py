import time
#import demo
import pi3d

#(W, H) = (None, None) # Fullscreen  - None should fill the screen (there are edge issues)
(W, H) = (400, 400) # Windowed
SCALE = 1.0 #should have 16th the shadertoy workload

BACKGROUND_COLOR = (1.0, 0.0, 0.0, 0.0)

display = pi3d.Display.create(w=W, h=H, frames_per_second=24.0,
                              background=BACKGROUND_COLOR,
                              display_config=pi3d.DISPLAY_CONFIG_HIDE_CURSOR | pi3d.DISPLAY_CONFIG_MAXIMIZED,
                              use_glx=True)
print(display.opengl.gl_id)
if W is None or H is None:
 (W, H) = (display.width, display.height)
 print('setting display size to ' + str(W) + ' ' + str(H))
sprite = pi3d.Triangle(corners=((-1.0, -1.0),(-1.0, 3.0),(3.0, -1.0)))
shader = pi3d.Shader('shadertoy04') # cloud
sprite.set_shader(shader)

## offscreen texture stuff ##
cam = pi3d.Camera(is_3d=False)

#flatsh = pi3d.Shader('post_vanilla')
flatsh = pi3d.Shader('post_pixelize2')
post = pi3d.PostProcess(camera=cam, shader=flatsh, scale=SCALE)
#post = pi3d.PostProcess(camera=cam, shader=flatsh, scale=1.0)

# interactive input
mouse = (0.9, 0.5)
kbd = pi3d.Keyboard()

# pass uniforms into our base shader
sprite.unif[0:2] = [W, H] # iResolution
sprite.unif[4] = SCALE # iScale
sprite.unif[6:8] = [0.9, 0.5] # iMouse

# pass uniforms into postprocessing flatsh
post.draw({0:W, 1:H, 4:SCALE, 6:0.9, 7:0.5})

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


