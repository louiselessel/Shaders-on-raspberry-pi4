#!/usr/bin/python

from __future__ import absolute_import, division, print_function, unicode_literals

"""This demo shows the use of special transition shaders on the Canvas 
shape for 2D drawing. Also threading is used to allow the file access to 
be done in the background.
"""
import random, time, glob, threading
#import demo
import pi3d

from six_mod.moves import queue

LOGGER = pi3d.Log(__name__, level='INFO', format='%(message)s')
LOGGER.info('''#########################################################
press ESC to escape, S to go back, any key for next slide
#########################################################''')

# Setup display and initialise pi3d
DISPLAY = pi3d.Display.create(background=(0.0, 0.0, 0.0, 1.0), frames_per_second=20)
shader = pi3d.Shader("shaders/test")

tex = pi3d.Texture("textures/lenna_l.png", blend=True, mipmap=False) #pixelly but faster 3.3MB in 3s
xrat = DISPLAY.width/tex.ix
yrat = DISPLAY.height/tex.iy
if yrat < xrat:
    xrat = yrat
wi, hi = tex.ix * xrat, tex.iy * xrat
#wi, hi = tex.ix, tex.iy
xi = (DISPLAY.width - wi)/2
yi = (DISPLAY.height - hi)/2
dimensions = (wi, hi, xi, yi)


class Carousel:
  def __init__(self):
    self.canvas = pi3d.Canvas()
    self.canvas.set_shader(shader)

  def next(self, step=1):
    sfg = tex #foreground
    sbg = tex #background
    self.canvas.set_draw_details(self.canvas.shader,[sfg.tex, sbg.tex])
    self.canvas.set_2d_size(dimensions[0], dimensions[1], dimensions[2], dimensions[3])
    #self.canvas.unif[48:54] = self.canvas.unif[42:48] #need to pass shader dimensions for both textures
    self.canvas.set_2d_size(dimensions[0], dimensions[1], dimensions[2], dimensions[3])

    
  def draw(self):
    self.canvas.draw()

crsl = Carousel()
crsl.next() # use to set up draw details for canvas

# Fetch key presses
mykeys = pi3d.Keyboard()
CAMERA = pi3d.Camera.instance()
CAMERA.was_moved = False #to save a tiny bit of work each loop
pictr = 0 #to do shader changing
shnum = 0
lasttm = time.time()
tmdelay = 8.0

while DISPLAY.loop_running():
  crsl.draw()
  tm = time.time()
  if tm > (lasttm + tmdelay):
    lasttm = tm
    crsl.next()

  k = mykeys.read()
  #k = -1
  if k >-1:
    if k==27: #ESC
      mykeys.close()
      DISPLAY.stop()
      break

DISPLAY.destroy()


