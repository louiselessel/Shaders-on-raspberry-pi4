#!/usr/bin/python
from __future__ import absolute_import, division, print_function, unicode_literals

import pi3d
import time
import datetime
import math

LOGGER = pi3d.Log(__name__, level='INFO', format='%(message)s')
LOGGER.info('''#####################################
Running shader program
#####################################''')


BACKGROUND_COLOR = (0.0, 0.0, 1.0, 0.0)

# Set shader resolution ratio
S_RES_W = 1
S_RES_H = 1

#----------------------- CHOOSE WINDOW SIZE
fullscreen = False

if (fullscreen == True):
    ## Do Fullscreen
    DISPLAY = pi3d.Display.create(background=BACKGROUND_COLOR, frames_per_second=30,
        display_config=pi3d.DISPLAY_CONFIG_HIDE_CURSOR | pi3d.DISPLAY_CONFIG_MAXIMIZED, use_glx=True)
    WIDTH, HEIGHT = DISPLAY.width, DISPLAY.height
else:
    ## Or Set size of window here
    WIDTH = 800
    HEIGHT = 400

    DISPLAY = pi3d.Display.create(w=WIDTH, h=HEIGHT, background=BACKGROUND_COLOR, frames_per_second=30, 
        display_config=pi3d.DISPLAY_CONFIG_HIDE_CURSOR, use_glx=True)

#-----------------------


## Try out different shaders/filters
#shader = pi3d.Shader("shaders/filter_sepia")   # maps image onto sprite - sepia tone
#shader = pi3d.Shader("shaders/uv_flat_my")      # maps image onto sprite
shader = pi3d.Shader("shaders/filter_toon")
#shader = pi3d.Shader("shaders/proto_v2")       # paste shader code here
#shader = pi3d.Shader("shaders/paste_here1")    # this uses old code way
#shader = pi3d.Shader("shaders/paste_here2")    # this uses old code way


## Set the texture and put shader on a sprite (a rectangle)
# if using the pixels from the loaded texture (buffer), you might have to set flip=True on sprite

tex = pi3d.Texture("textures/lenna_l.png", mipmap=False)       #256x256 px image
#tex = pi3d.Texture("textures/lenna_tiny.png", mipmap=False)    #25x25 px image


#----------------------- CHOOSE HOW TO MAP THE SPRITE

## Make the sprite that the shader gets put on, and place it in 3d space (z)
# Based on shader resolution - the img or shader will stretch to fit the sprite
# set z to S_RES_H or S_RES_W to fit display to width or height
sprite = pi3d.Sprite(flip=False, w=S_RES_W, h=S_RES_H, z= S_RES_H)

## You can also ignore the shader resolution and...
# keep height aspect ratio of img
#sprite = pi3d.Sprite(flip=False,w=S_RES_H * (tex.ix / tex.iy), h=S_RES_H, z= S_RES_H)
# keep width aspect ratio of img
#sprite = pi3d.Sprite(flip=False,w=S_RES_W, h=S_RES_W * (tex.ix / tex.iy), z= S_RES_W

## Make based on display size (ignore shader resolutions S_RES_H)
#sprite = pi3d.Sprite(flip=False,w=DISPLAY.height * (tex.ix / tex.iy), h=DISPLAY.height, z=DISPLAY.height-1)
#sprite = pi3d.Sprite(flip=False,w=DISPLAY.width, h=DISPLAY.height, z=DISPLAY.height)

print('sprite is this width: ' + str(sprite.width) + ' and this height: ' + str(sprite.height))

#-----------------------

sprite.set_draw_details(shader, [tex])




# Make variables that could be passed in as uniforms
t = 0
# Use keyboard input
mykeys = pi3d.Keyboard()



while DISPLAY.loop_running():
  # count time as iterations
  t += 1* 0.01
  
  # Set uniforms, for python reasons, these are set on '48-57' see notes in bottom
  # I'm still researching this part.
  #sprite.set_custom_data(48, [t, WIDTH, HEIGHT])
  #sprite.set_custom_data(48, [t, S_RES_W, S_RES_H])
  #sprite.set_custom_data(48, [t, 800, 800])
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
    # Uniform variables all in one array!
    self.unib = (c_float * 15)(0.0, 0.0, 0.0,
                               0.5, 0.5, 0.5,
                               1.0, 1.0, 0.0,
                               0.0, 0.0, 1.0,
                               0.5, 0.5, 0.5)
    """
""" pass to shader array of vec3 uniform variables:

    ===== ============================== ==== ==
    vec3        description              python
    ----- ------------------------------ -------
    index                                from to
    ===== ============================== ==== ==
        0  ntile, shiny, blend             0   2
        1  material                        3   5
        2  umult, vmult, point_size        6   8
        3  u_off, v_off, line_width/bump   9  10
        4  specular RGB value *_reflect   11  14 
    ===== ============================== ==== ==

    NB line width and bump factor clash but shouldn't be an issue
"""


# Passing in uniforms to post processing shaders
# pass uniforms into postprocessing flatsh
#post.draw({0:W, 1:H, 4:SCALE, 6:0.9, 7:0.5})

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
https://github.com/tipam/pi3d/blob/master/pi3d/Shape.py#L41

#sprite = pi3d.ImageSprite(tex, shader, w=20.0, h=20.0)

#sprite = pi3d.ImageSprite("textures/lenna_l.png", shader, w=WIDTH, h=HEIGHT)
#sprite = pi3d.ImageSprite("textures/lenna_l.png", shader, w=20.0, h=20.0)

#sprite = pi3d.Sprite(camera=CAMERA, w=WIDTH, h=HEIGHT, x=0.0, y=0.0, z=1.0)
#sprite.set_draw_details(shader, "textures/lenna_l.png")
#sprite.set_2d_size(WIDTH, HEIGHT, 0.0, 0.0) # used to get pixel scale by shader
#sprite.set_point_size(self, point_size=1.0):
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
  
    tm = time.time()
  if tm > (lasttm + tmdelay):
    lasttm = tm
     
"""
