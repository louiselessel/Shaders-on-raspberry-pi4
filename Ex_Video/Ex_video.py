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


"""
This example runs video at the videos native resolution
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



while display.loop_running():

  if flag:
    tex.update_ndarray(image, 0) # specify the first GL_TEXTURE0 i.e. first in buf[0].texture
    flag = False
    
  
  # Draw video to screen instead (this draws a weird triangle when CAMERA is in use)
  sprite.draw()

