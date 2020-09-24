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

import time
import demo
import pi3d
import datetime
from rgbmatrix import RGBMatrix, RGBMatrixOptions
from PIL import Image
from PIL import ImageDraw


#-------------------------------------------------
# Configuration for the shader

(W, H) = (64, 32) # Windowed - two screens chained
# For scale, make sure the numbers are divisible to the resolution with no remainders (use even numbers between 0 and 1). 1.0 is full non-scaled resolution.
SCALE = 1.0 # downscale the shadertoy shader resolution

timeScalar = 1.0 # for scaling the speed of time
fps = 30 # framerate

BACKGROUND_COLOR = (0.0, 0.0, 0.0, 0.0)


display = pi3d.Display.create(window_title='shader',
                              w=W, h=H, frames_per_second=fps,
                              background=BACKGROUND_COLOR,
                              display_config=pi3d.DISPLAY_CONFIG_HIDE_CURSOR | pi3d.DISPLAY_CONFIG_MAXIMIZED,
                              use_glx=True
                              )

print(display.opengl.gl_id) # the type of glsl your pi is running

if W is None or H is None:
 (W, H) = (display.width, display.height)
 print('setting display size to ' + str(W) + ' ' + str(H))

## shadertoy shader stuff ##
sprite = pi3d.Triangle(corners=((-1.0, -1.0),(-1.0, 3.0),(3.0, -1.0)))
shader = pi3d.Shader('shadertoy')
sprite.set_shader(shader)

## offscreen texture stuff ##
cam = pi3d.Camera(is_3d=False)
postsh = pi3d.Shader('post_vanilla')
post = pi3d.PostProcess(camera=cam, shader=postsh, scale=SCALE)

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

## pass shadertoy uniforms into our base shader from shadertoy ##
sprite.unif[0:2] = [W, H]       # iResolution
sprite.unif[2] = iTIME          # iTime - shader playback time
sprite.unif[3] = iTIMEDELTA     # iTimeDelta - render time (in seconds) ----- not implemented yet
sprite.unif[4] = SCALE          # iScale - scale for downscaling the resolution of shader
sprite.unif[5] = iFRAME         # iFrame - shader playback frame
sprite.unif[6:8] = [MX, MY]     # iMouse - xpos, ypos (set while button held down)
sprite.unif[9:11] = [MXC, MYC]    # iMouse - xposClicked, yposClicked (set on click)
sprite.unif[12:15] = [YR, MTH, DAY] # iDate
sprite.unif[15] = iDateSecondsSinceMidnight  # seconds since midnight
# iChannel0...3, iChannelTime and iChannelResolution not implemented yet

## pass own uniforms into shader (see notes.py to understand the addressing) ##
#sprite.unif[48:51] = [var1, var2, var3] # ownVar1 This is how you can pass in own variables to uniforms
#sprite.unif[57:60] = [var1, var2, var3] # ownVar2 You can add from 48 - 59, so this is the last address! 

## pass uniforms into postprocessing postsh ##
post.draw({0:W, 1:H, 2:iTIME, 3:iTIMEDELTA, 4:SCALE, 5:iFRAME,
           6:MX, 7:MY, 9:MXC, 10:MYC,
           12:YR, 13:MTH, 14:DAY, 15:iDateSecondsSinceMidnight})    

# time at start
tm0 = time.time()
last_time = 0


#-------------------------------------------------

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
    # drawing
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
        sprite.unif[9:11] = [MXC, MYC]    # update iMouse - xposClicked, yposClicked
        post.draw({9:MXC, 10:MYC})
        #print('(' + str(MXC) + ', ' + str(MYC) + ')')
        MouseClicked = True
    # while mouse is clicked (button held down)
    if MouseClicked == True:
        sprite.unif[6:8] = [MX, MY]       # update iMouse - xpos, ypos
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
    
    ## pass only the changed shadertoy uniforms into our base shader from shadertoy ##
    sprite.unif[2] = iTIME          # iTime - shader playback time
    sprite.unif[3] = iTIMEDELTA     # iTimeDelta - render time (in seconds) ----- not implemented yet
    sprite.unif[5] = iFRAME         # iFrame - shader playback frame
    sprite.unif[12:15] = [YR, MTH, DAY] # iDate
    sprite.unif[15] = iDateSecondsSinceMidnight  # seconds since midnight
        
    ## pass only the changed uniforms into postprocessing postsh ##
    post.draw({2:iTIME, 3:iTIMEDELTA, 5:iFRAME, 12:YR, 13:MTH, 14:DAY, 15:iDateSecondsSinceMidnight})
    
    ## updating variables ##
    iFRAME += 1
    #print(int(FRAME/fps))    # calculate seconds based on framerate, not time.time
    
    
    
    # draw the shader buffer into a PIL image
    image = Image.fromarray(pi3d.screenshot())
    #image = Image.fromarray(pi3d.masked_screenshot(xos, yos, ws, hs))
    #image = Image.fromarray(pi3d.masked_screenshot(xos, yos, W, H))
    matrix.SetImage(image,0,0)
    
    