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

## Matrix libraries ##
from rgbmatrix import RGBMatrix, RGBMatrixOptions
from PIL import Image
from PIL import ImageDraw

"""
This example runs video at the video's native resolution on the adafruit matrix.
In the bottom are two ways of running.
You either make the video the same resolution as the matrix (eg 32x32)
or you sample (eg 32 x 32) from the video, at the resolution of your matrix.
"""

## Video import ##
W, H, P = 480, 270, 3 # video width, height, bytes per pixel (3 = RGB)
command = [ 'ffmpeg', '-i', 'exercise01.mpg', '-f', 'image2pipe',
                      '-pix_fmt', 'rgb24', '-vcodec', 'rawvideo', '-']

"""
W, H, P = 320,240,3 # video width, height, bytes per pixel (3 = RGB)
command = [ 'ffmpeg', '-i', 'hale_bopp_1_320x240.mpg', '-f', 'image2pipe',
                      '-pix_fmt', 'rgb24', '-vcodec', 'rawvideo', '-']
"""

## Thread for running video ##
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

#========================================

## Settings ##
BACKGROUND_COLOR = (0.0, 0.0, 0.0, 0.0)

fps = 30             # framerate
display = pi3d.Display.create(window_title='Video',
                              w=W, h=H, frames_per_second=fps,
                              background=BACKGROUND_COLOR,
                              display_config=pi3d.DISPLAY_CONFIG_HIDE_CURSOR | pi3d.DISPLAY_CONFIG_MAXIMIZED,
                              use_glx=True
                              )

cam = pi3d.Camera(is_3d=False)


#========================================

# load shader
flatsh = pi3d.Shader("uv_flat")

# Create texture for video
tex = pi3d.Texture(image) # can pass numpy array or PIL.Image rather than path as string

# Create 2D flat sprite for the video texture
sprite = pi3d.Sprite(camera=cam, w=display.width, h=display.height)
sprite.set_shader(flatsh)
sprite.set_draw_details(flatsh, [tex])

## Matrix ##
# Configuration for the matrix
options = RGBMatrixOptions()
options.rows = 32
options.cols = 32
options.chain_length = 1
options.parallel = 1
options.hardware_mapping = 'adafruit-hat'  # If you have an Adafruit HAT: 'adafruit-hat', else 'regular'

matrix = RGBMatrix(options = options)



while display.loop_running():

  if flag:
    tex.update_ndarray(image, 0) # specify the first GL_TEXTURE0 i.e. first in buf[0].texture
    flag = False
    
  # Draw video to screen instead (this draws a weird triangle when CAMERA is in use)
  sprite.draw()
  
  # draw the buffer into a PIL image
  imageMatrix = Image.fromarray(pi3d.screenshot())
  
  # draw the video like this if it matches the pixel resolution of the matrix (32 x 32)
  # if the video is larger than the resolution of the matrix, it will sample lower left corner of you video
  #matrix.SetImage(imageMatrix,0,0)
  
  # Another option is to sample from a subset of the video pixels
  # This will sample 32 x 32 from location sampleX, sampleY in the video.
  # NOTE: must have the minus)
  sampleX = (W/2)
  sampleY = (H/2)
  matrix.SetImage(imageMatrix,-sampleX,-sampleY)

