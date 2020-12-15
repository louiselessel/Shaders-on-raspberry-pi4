#!/usr/bin/env python

# PIL Image module (create or load images) is explained here:
# http://effbot.org/imagingbook/image.htm
# PIL ImageDraw module (draw shapes to images) explained here:
# http://effbot.org/imagingbook/imagedraw.htm

import time
import demo
import pi3d
from rgbmatrix import RGBMatrix, RGBMatrixOptions
from PIL import Image
from PIL import ImageDraw

"""
This example runs a shader on one matrix.
Make sure you set the shader resolution (W, H) to be the resolution of your matrix,
NOTE: It only passes in the uniforms for iResolution and iTime and SCALE, to make the example easier to read.
If you want full shadertoy functionality, use the example called All_Uniforms.
"""

#-------------------------------------------------
# Configuration for the shader

(W, H) = (32, 32) # Windowed
# For scale, make sure the numbers are divisible to the resolution with no remainders (use even numbers between 0 and 1). 1.0 is full non-scaled resolution.
SCALE = 1.0 # downscale the shadertoy shader resolution

BACKGROUND_COLOR = (0.0, 0.0, 0.0, 0.0)

timeScalar = 1.0     # for scaling the speed of time
fps = 30             # framerate

display = pi3d.Display.create(window_title='shader',
                              w=W, h=H, frames_per_second=fps,
                              background=BACKGROUND_COLOR,
                              display_config=pi3d.DISPLAY_CONFIG_HIDE_CURSOR | pi3d.DISPLAY_CONFIG_MAXIMIZED,
                              use_glx=True
                              )

print(display.opengl.gl_id)
if W is None or H is None:
 (W, H) = (display.width, display.height)
 
# make shader
sprite = pi3d.Triangle(corners=((-1.0, -1.0),(-1.0, 3.0),(3.0, -1.0)))
shader = pi3d.Shader('shadertoy01')
sprite.set_shader(shader)

## offscreen texture stuff ##
cam = pi3d.Camera(is_3d=False)
flatsh = pi3d.Shader('post_vanilla')
post = pi3d.PostProcess(camera=cam, shader=flatsh, scale=SCALE)

## set up time ##
iTIME = 0
iTIMEDELTA = 0

## pass shadertoy uniforms into our base shader from shadertoy ##
sprite.unif[0:2] = [W, H]       # iResolution
sprite.unif[2] = iTIME          # iTime - shader playback time
sprite.unif[4] = SCALE          # iScale - scale for downscaling the resolution of shader

tm0 = time.time()


# Configuration for the matrix
options = RGBMatrixOptions()
options.rows = H
options.cols = W
options.chain_length = 1
options.parallel = 1
options.hardware_mapping = 'adafruit-hat'  # If you have an Adafruit HAT: 'adafruit-hat', else 'regular'

matrix = RGBMatrix(options = options)


## matrix scaling ##
(ws, hs) = (int(W*SCALE), int(H*SCALE))
(xos, yos) = (int((W-ws)* 0.5), int((H-hs)*0.5))


#-------------------------------------------------
# PUT SHADER ON MATRIX

while display.loop_running():
    post.start_capture()
    sprite.draw()
    post.end_capture()
    post.draw()

    iTIME = (time.time() - tm0) * timeScalar    # change the timeScalar to slow time
    sprite.unif[2] = iTIME          # iTime - shader playback time
    
    # draw the shader buffer into a PIL image
    image = Image.fromarray(pi3d.screenshot())
    #image = Image.fromarray(pi3d.masked_screenshot(xos, yos, ws, hs))
    #image = Image.fromarray(pi3d.masked_screenshot(xos, yos, W, H))
    matrix.SetImage(image,0,0)
    
    
