import time
import demo
import pi3d
import pygame

#(W, H) = (None, None) # Fullscreen - None should fill the screen (there are edge issues)
(W, H) = (400, 400) # Windowed
# For scale, make sure the numbers are divisible to the resolution with no remainders (use even numbers between 0 and 1). 1.0 is full non-scaled resolution.
SCALE = 0.2 # downscale the shadertoy shader

timeScalar = 1.0 # for scaling the speed of time
fps = 30

BACKGROUND_COLOR = (0.0, 0.0, 0.0, 0.0)

display = pi3d.Display.create(w=W, h=H, frames_per_second=fps,
                              background=BACKGROUND_COLOR,
                              #display_config=pi3d.DISPLAY_CONFIG_HIDE_CURSOR | pi3d.DISPLAY_CONFIG_MAXIMIZED,
                              use_glx=True)
print(display.opengl.gl_id)
if W is None or H is None:
 (W, H) = (display.width, display.height)
 print('setting display size to ' + str(W) + ' ' + str(H))

## shadertoy shader stuff ##
sprite = pi3d.Triangle(corners=((-1.0, -1.0),(-1.0, 3.0),(3.0, -1.0)))
#shader = pi3d.Shader('shadertoy01')
shader = pi3d.Shader('cloud')
sprite.set_shader(shader)

## offscreen texture stuff ##
cam = pi3d.Camera(is_3d=False)
postsh = pi3d.Shader('post_vanilla')
post = pi3d.PostProcess(camera=cam, shader=postsh, scale=SCALE)

## interactive input ##
kbd = pi3d.Keyboard()

mouse = pi3d.Mouse() # pi3d.Mouse(restrict = True)
mouse.start()
MX, MY = mouse.position()
MC = mouse.button_status() # 8 = hover, 9 = right Click down, 10 = left C, 12 = middle C
MouseClicked = False

iTIME = 0
iTIMEDELTA = 0
iFRAME = 0
(YR, MTH, DAY) = (0.0, 0.0, 0.0)

## pass shadertoy uniforms into our base shader from shadertoy ##
sprite.unif[0:2] = [W, H]       # iResolution
sprite.unif[2] = iTIME          # iTime - shader playback time
sprite.unif[3] = iTIMEDELTA     # iTimeDelta - render time (in seconds) ----- not implemented yet
sprite.unif[4] = SCALE          # iScale - scale for downscaling the resolution of shader
sprite.unif[5] = iFRAME         # iFrame - shader playback frame
sprite.unif[6:8] = [MX, MY]     # iMouse - xpos, ypos (set while button held down)
sprite.unif[9:11] = [MX, MY]    # iMouse - xposClicked, yposClicked (set on click)
sprite.unif[12:15] = [YR, MTH, DAY] # iDate ----- not implemented yet
# iChannel0...3, iChannelTime and iChannelResolution not implemented yet

## pass own uniforms into shader (see notes.py to understand the addressing) ##
#sprite.unif[48:51] = [var1, var2, var3] # ownVar1 This is how you can pass in own variables to uniforms
#sprite.unif[57:60] = [var1, var2, var3] # ownVar2 You can add from 48 - 59, so this is the last address! 

## pass uniforms into postprocessing postsh ##
post.draw({0:W, 1:H, 4:SCALE, 6:MX, 7:MY})    # you can add more as you need, like above

# time at start
tm0 = time.time()

# initializes Pygame
#pygame.init()

# sets the window title
#pygame.display.set_caption(u'Mouse events')

# sets the window size
#pygame.display.set_mode((W, H))


def mouseFromPygame():
    # initializes Pygame
    #pygame.init()

    # sets the window title
    #pygame.display.set_caption('Mouse events')

    # sets the window size
    #pygame.display.set_mode((W, H))

    # infinite loop
    #while True:
    # gets a single event from the event queue
    event = pygame.event.wait()

    # if the 'close' button of the window is pressed
    if event.type == pygame.QUIT:
        # stops the application
        # finalizes Pygame
        pygame.quit()

    # if any mouse button is pressed
    if event.type == pygame.MOUSEBUTTONDOWN:
        print('test')
            # prints on the console the pressed button and its position at that moment
            #print u'button {} pressed in the position {}'.format(event.button, event.pos)

    # if any mouse button is released
    if event.type == pygame.MOUSEBUTTONUP:
        print('test')
            # prints on the console the button released and its position at that moment
            #print u'button {} released in the position {}'.format(event.button, event.pos)

        # if the mouse is moved
    if event.type == pygame.MOUSEMOTION:
        print(event.pos)
            # prints on the console the pressed buttons, and their position and relative movement at that time
            #print u'pressed buttons {}, position {} and relative movement {}'.format(event.buttons, event.pos, event.rel)
    
    

while display.loop_running():
    # drawing
    post.start_capture()
    sprite.draw()
    post.end_capture()
    post.draw()
    
    ## inputs - mouse
    MX, MY = mouse.position()
    MVX, MVY = mouse.velocity()
    MC = mouse.button_status()
    print('(' + str(MX) + ', ' + str(MY) + ')')
    
    # if mouse click on this frame (any button)
    if MC == 9 or MC == 10 or MC == 12 and MouseClicked == False:
        sprite.unif[9:11] = [MX, MY]    # update iMouse - xposClicked, yposClicked
        #print('(' + str(MX) + ', ' + str(MY) + ')')
        MouseClicked = True
    # while mouse is clicked (button held down)
    if MouseClicked == True:
        sprite.unif[6:8] = [MX, MY]    # update iMouse - xpos, ypos
    # mouse button released    
    if MC == 8 and MouseClicked == True:
        MouseClicked = False
    
    ## inputs - keyboard
    k = kbd.read()
    if k == 27:
        kbd.close()
        mouse1.stop()
        display.stop()
        break
    
    # setting non-interactive uniforms
    iTIME = (time.time() - tm0) * timeScalar    # change the timeScalar to slow time
    sprite.unif[2] = iTIME
    sprite.unif[5] = iFRAME
    
    # updating variables
    iFRAME += 1
    #print(int(FRAME/fps))    # calculate seconds
    #mouseFromPygame()

