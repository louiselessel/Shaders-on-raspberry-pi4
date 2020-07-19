#!/usr/bin/python
from __future__ import absolute_import, division, print_function, unicode_literals

import pi3d
import time
import datetime
import math

BACKGROUND_COLOR = (0.0, 0.0, 0.0, 0.0)
ZPLANE = 1000

# Set shader resolution
S_RES_W = 200
S_RES_H = 200

#-----------------------

## EITHER Set size of window
WIDTH = 500
HEIGHT = 500

DISPLAY = pi3d.Display.create(w=WIDTH, h=HEIGHT, background=BACKGROUND_COLOR, frames_per_second=30, 
    display_config=pi3d.DISPLAY_CONFIG_HIDE_CURSOR | pi3d.DISPLAY_CONFIG_MAXIMIZED, use_glx=True)

"""
## OR do Fullscreen
DISPLAY = pi3d.Display.create(background=BACKGROUND_COLOR, frames_per_second=30,
    display_config=pi3d.DISPLAY_CONFIG_HIDE_CURSOR | pi3d.DISPLAY_CONFIG_MAXIMIZED, use_glx=True)
WIDTH, HEIGHT = DISPLAY.width, DISPLAY.height
"""

## Try out different shaders/filters

#shader = pi3d.Shader("shaders/test") # the most rudamentary shader - red color
#shader = pi3d.Shader("shaders/filter_toon") # one of the pi3D shaders, using tex input
#shader = pi3d.Shader("shaders/test_shadertoy")
#shader = pi3d.Shader("shaders/test2_shadertoy")
#shader = pi3d.Shader("shaders/test_cloud_shadertoy")
#shader = pi3d.Shader("shaders/TD_shadertoy")
shader = pi3d.Shader("shaders/paste_here")



# Set the texture and put shader on a sprite (a fullscreen rectangle)
tex = pi3d.Texture("textures/lenna_l.png")
sprite = pi3d.Sprite(w=DISPLAY.height * tex.ix / tex.iy, h=DISPLAY.height, z=200.0)
sprite.set_draw_details(shader, [tex])

# Make variables that could be passed in as uniforms
t = 0

# Use keyboard input
mykeys = pi3d.Keyboard()


while DISPLAY.loop_running():

  
  # count time as iterations instead
  t += 1* 0.1
  #print(t)
  
  # set uniforms, for GLES reasons, these are set on '48'
  sprite.set_custom_data(48, [t, WIDTH, HEIGHT])
  
  # draw the shader on the sprite (flat rectangle)
  sprite.draw()
  
  # read the keyboard for an easy exit (only works on pi, on VNC Viewer: CMD + ESC)
  k = mykeys.read()
  if k == 27:
    mykeys.close()
    DISPLAY.destroy()
    break



""" RESEARCH NOTES ------------------------------------- """

## Looking into another way to do fullscreen
#CAMERA = pi3d.Camera(is_3d=False)
# do fullscreen - but include exit condition in code before implementing (https://www.programcreek.com/python/example/2557/pygame.QUIT)
# use_pygame=True


## Other resizing tests for when using image input - scaling of image isnt correct yet
""" 
#sprite = pi3d.ImageSprite(tex, shader, w=20.0, h=20.0)

#sprite = pi3d.ImageSprite("textures/lenna_l.png", shader, w=WIDTH, h=HEIGHT)
#sprite = pi3d.ImageSprite("textures/lenna_l.png", shader, w=20.0, h=20.0)

#sprite = pi3d.Sprite(camera=CAMERA, w=WIDTH, h=HEIGHT, x=0.0, y=0.0, z=1.0)
#sprite.set_draw_details(shader, "textures/lenna_l.png")
#sprite.set_2d_size(WIDTH, HEIGHT, 0.0, 0.0) # used to get pixel scale by shader
"""

# time in milliseconds
"""
  I havent found a good way of using actual time, the clock on the pi yet.
  It isnt giving a nice smooth number that'll look good in the shader.
  
  #ms = int(round(time.time()*1000))
  #ms = round(time.time()*1000)
  #print(ms/1000000)
  #print(abs(math.sin(ms)))
  #print(datetime.datetime.now())
"""
