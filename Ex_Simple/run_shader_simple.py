import time
import demo
import pi3d

## This code was made with help from paddywwoof

BACKGROUND_COLOR = (0.0, 0.0, 0.0, 0.0)

#----------------------- CHOOSE WINDOW SIZE
fullscreen = False

if (fullscreen == True):
    ## Do Fullscreen --- , use_glx=True
    display = pi3d.Display.create(background=BACKGROUND_COLOR, frames_per_second=30,
        display_config=pi3d.DISPLAY_CONFIG_HIDE_CURSOR | pi3d.DISPLAY_CONFIG_MAXIMIZED)
    WIDTH, HEIGHT = display.width, display.height
    
else: 
    WIDTH, HEIGHT = 800, 600
    display = pi3d.Display.create(w=WIDTH, h=HEIGHT)
    
print(display.opengl.gl_id)

#----------------------- 

sprite = pi3d.Triangle(corners=((-1.0, -1.0),(-1.0, 3.0),(3.0, -1.0)))
shader = pi3d.Shader('shadertoy_simple')
sprite.set_shader(shader)
kbd = pi3d.Keyboard()

# time at code start
tm0 = time.time()

while display.loop_running():
    sprite.draw()

    ## pass shadertoy uniforms into our base shader from shadertoy ##
    sprite.unif[0:2] = [WIDTH, HEIGHT]       # iResolution
    sprite.unif[2] = (time.time() - tm0) * 0.1  # iTime * multiplication to slow time
    
    
    if kbd.read() == 27:
        kbd.close()
        display.stop()
        break
