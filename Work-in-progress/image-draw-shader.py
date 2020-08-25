#!/usr/bin/env python

# (This is an example similar to an example from the Adafruit fork
#  to show the similarities. Most important difference currently is, that
#  this library wants RGB mode.)
#
# A more complex RGBMatrix example works with the Python Imaging Library,
# demonstrating a few graphics primitives and image loading.
# Note that PIL graphics do not have an immediate effect on the display --
# image is drawn into a separate buffer, which is then copied to the matrix
# using the SetImage() function (see examples below).
# Requires rgbmatrix.so present in the same directory.

# PIL Image module (create or load images) is explained here:
# http://effbot.org/imagingbook/image.htm
# PIL ImageDraw module (draw shapes to images) explained here:
# http://effbot.org/imagingbook/imagedraw.htm

from PIL import Image
from PIL import ImageDraw
import time
import demo
import pi3d
from rgbmatrix import RGBMatrix, RGBMatrixOptions

# Configuration for the matrix
options = RGBMatrixOptions()
options.rows = 32
options.chain_length = 1
options.parallel = 1
options.hardware_mapping = 'adafruit-hat'  # If you have an Adafruit HAT: 'adafruit-hat', else 'regular'

matrix = RGBMatrix(options = options)

#-------------------------------------------------
# Configuration for the shader

#(W, H) = (None, None) # None should fill the screen (there are edge issues)
(W, H) = (32, 32) # Windowed

# For scale, make sure the numbers are divisible to the resolution with no remainders (use even numbers between 0 and 1). 1.0 is full non-scaled resolution.
SCALE = 1.0

BACKGROUND_COLOR = (0.0, 0.0, 0.0, 0.0)

display = pi3d.Display.create(window_title='shader', w=W, h=H, frames_per_second=30.0, background=BACKGROUND_COLOR)
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

sprite.unif[0:2] = [W, H]
sprite.unif[4] = SCALE
tm0 = time.time()

#-------------------------------------------------
# Shader display
'''
while display.loop_running():
    post.start_capture()
    sprite.draw()
    post.end_capture()
    post.draw()
    sprite.unif[3] = time.time() - tm0
    '''

#-------------------------------------------------
# This would draw a rectangle with a cross..

# RGB example w/graphics prims.
# Note, only "RGB" mode is supported currently.
image = Image.new("RGB", (32, 32))  # Can be larger than matrix if wanted!!
draw = ImageDraw.Draw(image)  # Declare Draw instance before prims
# Draw some shapes into image (no immediate effect on matrix)...
draw.rectangle((0, 0, 31, 31), fill=(0, 0, 0), outline=(0, 0, 255))
draw.line((0, 0, 31, 31), fill=(255, 0, 0))
draw.line((0, 31, 31, 0), fill=(0, 255, 0))


#-------------------------------------------------
# PUT SHADER ON MATRIX

while display.loop_running():
    post.start_capture()
    sprite.draw()
    post.end_capture()
    post.draw()
    sprite.unif[3] = time.time() - tm0
    
    # draw the shader buffer into a PIL image here  <---- #
    
    # set image to matrix
    matrix.SetImage(image,0,0)


#-------------------------------------------------
# REGULAR DRAWING ON MATRIX
'''
## STATIC DRAWING ##
matrix.SetImage(image,0,0)
time.sleep(5)
matrix.Clear()
'''

'''
## ANIMATION ##
# Scroll image across matrix.
for n in range(-32, 33):  # Start off top-left, move off bottom-right
    matrix.Clear()
    matrix.SetImage(image, n, n)
    time.sleep(0.2)
matrix.Clear()
'''