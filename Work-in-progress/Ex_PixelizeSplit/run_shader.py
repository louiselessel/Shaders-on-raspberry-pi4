import time
import demo
import pi3d

#(W, H) = (None, None) # Fullscreen - None should fill the screen (there are edge issues)
(W, H) = (600, 600) # Windowed
# For scale, make sure the numbers are divisible to the resolution with no remainders (use even numbers between 0 and 1). 1.0 is full non-scaled resolution.
SCALE = .40 # downscale the shadertoy shader

BACKGROUND_COLOR = (1.0, 0.0, 0.0, 0.0)

display = pi3d.Display.create(w=W, h=H, frames_per_second=24.0,
                              background=BACKGROUND_COLOR,
                              display_config=pi3d.DISPLAY_CONFIG_HIDE_CURSOR | pi3d.DISPLAY_CONFIG_MAXIMIZED,
                              use_glx=True)
print(display.opengl.gl_id)
if W is None or H is None:
 (W, H) = (display.width, display.height)
 print('setting display size to ' + str(W) + ' ' + str(H))

## shadertoy shader stuff ##
sprite = pi3d.Triangle(corners=((-1.0, -1.0),(-1.0, 3.0),(3.0, -1.0)))
shader = pi3d.Shader('shadertoy01') # shadertoy shader
#shader = pi3d.Shader('cloud') # shadertoy shader
sprite.set_shader(shader)

## offscreen texture stuff ##
cam = pi3d.Camera(is_3d=False)
postsh = pi3d.Shader('post_pixelizeSplit')
post = pi3d.PostProcess(camera=cam, shader=postsh, scale=SCALE)

## interactive input ##
mouse = (0.9, 0.5) # hardcoded, replace these with interactive mouse xy coordinates
kbd = pi3d.Keyboard()

# pass uniforms into our base shader
sprite.unif[0:2] = [W, H] # iResolution
sprite.unif[4] = SCALE # iScale
sprite.unif[6:8] = [mouse[0], mouse[1]] # iMouse

# pass uniforms into postprocessing postsh
post.draw({0:W, 1:H, 4:SCALE, 6:mouse[0], 7:mouse[1]})

# time at start
tm0 = time.time()

while display.loop_running():
    # drawing
    post.start_capture()
    sprite.draw()
    post.end_capture()
    post.draw()
    # setting interactive / changing uniforms
    sprite.unif[3] = (time.time() - tm0) * .20    # change the multiplier to slow time
    sprite.unif[6:8] = [mouse[0], mouse[1]] 
    post.draw({6:mouse[0], 7:mouse[1]})
    # keyboard control
    k = kbd.read()
    if k == 27:
        kbd.close()
        display.stop()
        break
