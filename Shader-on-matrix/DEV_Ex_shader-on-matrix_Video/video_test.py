#!/usr/bin/python
from __future__ import absolute_import, division, print_function, unicode_literals
import demo
import pi3d
import numpy as np
from PIL import Image, ImageDraw
import subprocess as sp
import threading
import time
import math

## Matrix ##
"""
from rgbmatrix import RGBMatrix, RGBMatrixOptions
from PIL import Image
from PIL import ImageDraw
"""



W, H, P = 480, 270, 3 # video width, height, bytes per pixel (3 = RGB)

command = [ 'ffmpeg', '-i', 'exercise01.mpg', '-f', 'image2pipe',
                      '-pix_fmt', 'rgb24', '-vcodec', 'rawvideo', '-']
flag = False # use to signal new texture
image = np.zeros((H, W, P), dtype='uint8')

def pipe_thread():
  global flag, image
  pipe = None
  while True:
    st_tm = time.time()
    if pipe is None:
      pipe = sp.Popen(command, stdout=sp.PIPE, stderr=sp.PIPE, bufsize=-1)
    image =  np.frombuffer(pipe.stdout.read(H * W * P), dtype='uint8')
    pipe.stdout.flush() # presumably nothing else has arrived since read()
    pipe.stderr.flush() # ffmpeg sends commentary to stderr
    if len(image) < H * W * P: # end of video, reload
      pipe.terminate()
      pipe = None
    else:
      image.shape = (H, W, P)
      flag = True
    step = time.time() - st_tm
    time.sleep(max(0.04 - step, 0.0)) # adding fps info to ffmpeg doesn't seem to have any effect


t = threading.Thread(target=pipe_thread)
t.daemon = True
t.start()


while flag is False:
  time.sleep(1.0)

# Setup display and initialise pi3d
DISPLAY = pi3d.Display.create(x=50, y=50, frames_per_second=25) # ????? x,y ?
DISPLAY.set_background(0.4,0.8,0.8,1)      # r,g,b,alpha





#========================================

# load shader
shader = pi3d.Shader("uv_bump")
flatsh = pi3d.Shader("uv_flat")

# Create monument (a sphere with video texture on)
tex = pi3d.Texture(image) # can pass numpy array or PIL.Image rather than path as string
monument = pi3d.Sphere(sx=W, sy=H, sz=W)
monument.set_draw_details(shader, [tex, tex], 4.0, umult=2.0)

# create 2D flat sprite instead, taking up the whole screen
sprite = pi3d.Triangle(corners=((-1.0, -1.0),(-1.0, 3.0),(3.0, -1.0)))
#shader = pi3d.Shader('shader')
#sprite.set_shader(flatsh)
#sprite.set_draw_details(flats, [tex, tex], 4.0, umult=2.0)
sprite.set_draw_details(flatsh, [tex])



## Matrix ##
# Configuration for the matrix
"""
options = RGBMatrixOptions()
options.rows = 32
options.cols = 32
options.chain_length = 1
options.parallel = 1
options.hardware_mapping = 'adafruit-hat'  # If you have an Adafruit HAT: 'adafruit-hat', else 'regular'

matrix = RGBMatrix(options = options)
"""

#cam = pi3d.Camera(is_3d=False) # this camera worked in the shader example
CAMERA = pi3d.Camera.instance() # camera from videowalk example

# Display scene and rotate cuboid
while DISPLAY.loop_running():
  CAMERA.reset()
  CAMERA.position((0, 0, -20))

  if flag:
    tex.update_ndarray(image, 0) # specify the first GL_TEXTURE0 i.e. first in buf[0].texture
    flag = False
    
  # this works and draws a sphere with some video mapping (when cam is in use)
  monument.draw()
  monument.rotateIncY(0.25)
  
  # Draw video to screen instead (this draws a weird triangle when CAMERA is in use)
  sprite.draw()
  
  # draw video to matrix
   # draw the buffer into a PIL image
   #image = Image.fromarray(pi3d.screenshot())
   #matrix.SetImage(image,0,0)

