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
  
  # Set uniforms, for python reasons, these are set on '48-57' see notes in bottom
  # I'm still researching this part.
  sprite.set_custom_data(48, [t, WIDTH, HEIGHT])
  #sprite.set_custom_data(51, [var, var, var])
  #sprite.set_custom_data(54, [var, var, var])
  #sprite.set_custom_data(57, [var, var, var])
  
  
  
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

"""
Passing in own uniforms

 # uniform variables all in one array (for Shape and one for Buffer)
    self.unif =  (ctypes.c_float * 60)(
      x, y, z, rx, ry, rz,
      sx, sy, sz, cx, cy, cz,
      0.5, 0.5, 0.5, 5000.0, 0.8, 1.0,
      0.0, 0.0, 0.0, light.is_point, 0.0, 0.0,
      light.lightpos[0], light.lightpos[1], light.lightpos[2],
      light.lightcol[0], light.lightcol[1], light.lightcol[2],
      light.lightamb[0], light.lightamb[1], light.lightamb[2],
      0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
      0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
      0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0)
"""
""" pass to shader array of vec3 uniform variables:
    ===== ========================================== ==== ==
    vec3  description                                python
    ----- ------------------------------------------ -------
    index                                            from to
    ===== ========================================== ==== ==
       0  location                                     0   2
       1  rotation                                     3   5
       2  scale                                        6   8
       3  offset                                       9  11
       4  fog shade                                   12  14
       5  fog distance, fog alpha, shape alpha        15  17
       6  camera position                             18  20
       7  point light if 1: light0, light1, unused    21  23
       8  light0 position, direction vector           24  26
       9  light0 strength per shade                   27  29
      10  light0 ambient values                       30  32
      11  light1 position, direction vector           33  35
      12  light1 strength per shade                   36  38
      13  light1 ambient values                       39  41
      14  defocus dist_from, dist_to, amount          42  44 # also 2D x, y
      15  defocus frame width, height (only 2 used)   45  46 # also 2D w, h, tot_ht
      16  custom data space                           48  50 <---- We can pass uniforms from 48-59
      17  custom data space                           51  53 <- We fetch them in the shader as 16-19
      18  custom data space                           54  56
      19  custom data space                           57  59
    ===== ========================================== ==== ==
"""


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
