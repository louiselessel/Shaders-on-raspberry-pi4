import pi3d

#### important note
# hit cmd + ESC to get Geany back up, then click stop, to stop the application

BACKGROUND_COLOR = (0.0, 0.0, 0.0, 0.0)
DISPLAY = pi3d.Display.create(background=BACKGROUND_COLOR, frames_per_second=40, use_glx=True)
HWIDTH, HHEIGHT = DISPLAY.width / 2.0, DISPLAY.height / 2.0
#CAMERA = pi3d.Camera(is_3d=False)

shader = pi3d.Shader("shaders/uv_sprite")

ball = pi3d.Sphere(z = 5.0)

# listen for keys
keys = pi3d.Keyboard()

# start the display loop
while DISPLAY.loop_running():
    # store keys
    k = keys.read()
    if k == 27: # if hit ESC
        keys.close()
        DISPLAY.destroy()
        break
    ball.draw()
