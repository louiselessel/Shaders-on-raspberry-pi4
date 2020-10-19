#!/usr/bin/python
from __future__ import absolute_import, division, print_function, unicode_literals
import time
import demo
import pi3d
import datetime

import numpy as np
from PIL import Image, ImageDraw
import subprocess as sp
import threading
import time
import math


"""
This example runs a video as input to a post processing shader
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


# For scale, make sure the numbers are divisible to the resolution with no remainders (use even numbers between 0 and 1). 1.0 is full non-scaled resolution.
SCALE = 1.0 # downscale the shadertoy shader resolution


BACKGROUND_COLOR = (0.0, 0.0, 0.0, 0.0)


timeScalar = 1.0     # for scaling the speed of time
fps = 30             # framerate
display = pi3d.Display.create(window_title='post processing shader',
                              w=W, h=H, frames_per_second=fps,
                              background=BACKGROUND_COLOR,
                              display_config=pi3d.DISPLAY_CONFIG_HIDE_CURSOR | pi3d.DISPLAY_CONFIG_MAXIMIZED,
                              use_glx=True
                              )

cam = pi3d.Camera(is_3d=False)


#========================================

# load shaders
flatsh = pi3d.Shader("uv_flat")
postsh = pi3d.Shader('post_pixelize')
post = pi3d.PostProcess(camera=cam, shader=postsh, scale=SCALE)

# Create texture for video
tex = pi3d.Texture(image) # can pass numpy array or PIL.Image rather than path as string

# Create 2D flat sprite for the video texture
sprite = pi3d.Sprite(camera=cam, w=display.width, h=display.height)
sprite.set_shader(flatsh)
sprite.set_draw_details(flatsh, [tex])

## interactive inputs ##
kbd = pi3d.Keyboard()
mouse = pi3d.Mouse() # pi3d.Mouse(restrict = True) # changes input coordinates
mouse.start()
MX, MY = mouse.position()
MXC, MYC = mouse.position()
MC = mouse.button_status() # 8 = hover, 9 = right Click down, 10 = left C, 12 = middle C
MouseClicked = False


## set up time ##
iTIME = 0
iTIMEDELTA = 0
iFRAME = 0
iDate = datetime.datetime.now()
#print ("The current local date time is ", iDate)
(YR, MTH, DAY) = (iDate.year, iDate.month, iDate.day)
iDateSecondsSinceMidnight = iDate.hour*60*60 + iDate.minute*60 + iDate.second
#print (iDateSecondsSinceMidnight)

## pass uniforms into postprocessing postsh ##
post.draw({0:W, 1:H, 2:iTIME, 3:iTIMEDELTA, 4:SCALE, 5:iFRAME,
           6:MX, 7:MY, 9:MXC, 10:MYC,
           12:YR, 13:MTH, 14:DAY, 15:iDateSecondsSinceMidnight})     


# time at start
tm0 = time.time()
last_time = 0



while display.loop_running():

    if flag:
        tex.update_ndarray(image, 0) # specify the first GL_TEXTURE0 i.e. first in buf[0].texture
        flag = False
    
    # Draw video to screen instead (this draws a weird triangle when CAMERA is in use)
    post.start_capture()
    sprite.draw()
    post.end_capture()
    post.draw()
    
    ## inputs - mouse ##
    MX, MY = mouse.position()
    MVX, MVY = mouse.velocity()
    MC = mouse.button_status()
    #print('(' + str(MX) + ', ' + str(MY) + ')')
    
    # if mouse click on this frame (any button)
    if MC == 9 or MC == 10 or MC == 12 and MouseClicked == False:
        (MXC, MYC) = (MX, MY)
        post.draw({9:MXC, 10:MYC})
        #print('(' + str(MXC) + ', ' + str(MYC) + ')')
        MouseClicked = True
    # while mouse is clicked (button held down)
    if MouseClicked == True:
        post.draw({6:MX, 7:MY})
    # mouse button released    
    if MC == 8 and MouseClicked == True:
        MouseClicked = False
    
    ## inputs - keyboard ##
    k = kbd.read()
    if k == 27:
        kbd.close()
        mouse.stop()
        display.stop()
        break
    
    ## setting non-interactive uniforms ##
    iTIME = (time.time() - tm0) * timeScalar    # change the timeScalar to slow time
    iDate = datetime.datetime.now()
    (YR, MTH, DAY) = (iDate.year, iDate.month, iDate.day)
    iDateSecondsSinceMidnight = iDate.hour*60*60 + iDate.minute*60 + iDate.second
    iTIMEDELTA = display.time - last_time # display.time is set at start of each frame
    last_time = display.time
    
    post.draw({2:iTIME, 3:iTIMEDELTA, 5:iFRAME, 12:YR, 13:MTH, 14:DAY, 15:iDateSecondsSinceMidnight})
    
    ## updating variables ##
    iFRAME += 1
    #print(int(FRAME/fps))    # calculate seconds based on framerate, not time.time
