import time
import demo
import pi3d

## This code was made with help from paddywwoof
"""
This example is developed from the simple example, it implements downscaling of the shader resolution using post-processing, to optimize graphics more for the pi to run larger screens.
NOTE: Only iResolution, iTime and SCALE is passed into the shader uniforms. If you want full shadertoy functionality, use the All_Uniforms example.
"""

#(W, H) = (None, None) # Fullscreen - None should fill the screen (there are unresolved edge issues)
(W, H) = (400, 400) # Windowed
# For scale, make sure the numbers are divisible to the resolution with no remainders (use even numbers between 0 and 1). 1.0 is full non-scaled resolution.
SCALE = 0.2 # downscale the shadertoy shader resolution

timeScalar = 1.0 # for scaling the speed of time
fps = 30 # framerate

BACKGROUND_COLOR = (0.0, 0.0, 0.0, 0.0)


display = pi3d.Display.create(window_title='shader',
                              w=W, h=H, frames_per_second=fps,
                              background=BACKGROUND_COLOR,
                              display_config=pi3d.DISPLAY_CONFIG_HIDE_CURSOR | pi3d.DISPLAY_CONFIG_MAXIMIZED,
                              use_glx=True
                              )

print(display.opengl.gl_id) # the type of glsl your pi is running

if W is None or H is None:
 (W, H) = (display.width, display.height)
 print('setting display size to ' + str(W) + ' ' + str(H))
 
## load shader ##
sprite = pi3d.Triangle(corners=((-1.0, -1.0),(-1.0, 3.0),(3.0, -1.0)))
shader = pi3d.Shader('shadertoy01')
sprite.set_shader(shader)

## offscreen texture stuff ##
cam = pi3d.Camera(is_3d=False)
flatsh = pi3d.Shader('post_vanilla')
post = pi3d.PostProcess(camera=cam, shader=flatsh, scale=SCALE)

kbd = pi3d.Keyboard()

## pass shadertoy uniforms into our base shader from shadertoy ##
sprite.unif[0:2] = [W, H]       # iResolution
sprite.unif[2] = iTIME          # iTime - shader playback time
sprite.unif[4] = SCALE          # iScale - scale for downscaling the resolution of shader

tm0 = time.time()

while display.loop_running():
    post.start_capture()
    sprite.draw()
    post.end_capture()
    post.draw()
    
    iTIME = (time.time() - tm0) * timeScalar    # change the timeScalar to slow time
    sprite.unif[2] = iTIME          # iTime - shader playback time

    k = kbd.read()
    if k == 27:
        kbd.close()
        display.stop()
        break
