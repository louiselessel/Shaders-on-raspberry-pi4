
# Shaders on the matrix - using python

Based on:
https://github.com/hzeller/rpi-rgb-led-matrix/tree/master/bindings/python

Tested on the raspberry pi 4. Using an Adafruit RGB Matrix HAT + RTC, Adafruit Bonnet and a few of Hzellers recommended boards here https://github.com/hzeller/rpi-rgb-led-matrix/tree/master/adapter. I am currently using the board from electrodragon.



----

## Dependencies:

The installation guide for the RGB Matrix on Adafruit is outdated compared to Hzeller's repo.
(you can't use most of the matrix settings such as GPIO slowdown).
So don't use this: https://learn.adafruit.com/adafruit-rgb-matrix-bonnet-for-raspberry-pi/driving-matrices

I've re-written it in the file "rgb-matrix.sh" included with this repo, until further notice.
The only difference is that it gets a newer verion of Hzeller's code. 


### In terminal:
    cd /home/pi/Desktop/Shaders-on-raspberry-pi4/Shader-on-matrix
    sudo bash rgb-matrix.sh


My pi is set up for the "Adafruit Bonnet" and for "quality" (1,1). Using the board from electrodragon. I've found that to be more stable.

After installation, the hzeller repo has included python examples in the folder: bindings/python/samples
The code in this repo is based on these.


Now there are two things that must be on your pi for this repo to work.


### 1. Install pi3d + make a local copy of the library

Since this code builds upon the code in the "Shaders on raspberry pi4" repo, 
you must first follow the install steps for getting the pi3d library on your pi found here:
https://github.com/louiselessel/Shaders-on-raspberry-pi4#dependencies

Then, to make sure you can use pi3d no matter where you place the matrix code from this repo on your pi,
you must also download the pi3d repo folder from this link https://github.com/tipam/pi3d, 
and place it in the "pi" folder (the folder that also has "Desktop", "Documents", "Pictures" etc. in it), see the second picture.

### In terminal:

    cd /home/pi
    git clone https://github.com/tipam/pi3d.git 



1. Unzip the folder.

![Unzip the folder](https://github.com/louiselessel/Shaders-on-raspberry-pi4/blob/master/Documentation/Screenshot_unzip.png)


2. Place the repo here. Delete the "-master" part of the name. 

![Place pi3d local repo here](https://github.com/louiselessel/Shaders-on-raspberry-pi4/blob/master/Documentation/Screenshot_PlacementOfpi3d.png)



### 2. Then, make sure to install the imaging library called PIL
It should already be installed if you have the full Raspbian OS on your pi.

### In terminal:

    sudo apt-get update && sudo apt-get install python3-dev python3-pillow -y
    make build-python PYTHON=$(which python3)
    sudo make install-python PYTHON=$(which python3)


----

## How to

You can test the code in Thonny by pressing Run, but to take advantage of the settings for the matrix, the code must be run from terminal. 
So cd to the folder of the example you want to run and then run the sudo command below.


### In terminal:
To run the simple example:
```
    cd /home/pi/Desktop/Shader-on-matrix/
    cd Ex_shader-on-matrix_simple
    sudo python3 ./shader-on-matrix-simple.py 
```
    
To run the All Uniforms example:
```
    cd /home/pi/Desktop/Shader-on-matrix/
    cd Ex_shader-on-matrix_All-Uniforms/
    sudo python3 ./shader-on-matrix-All-Uniforms.py
```


You may have to experiment with different values for matrix settings inside the .py file.
For instance it is sometimes needed to throttle back the speed (options.gpio_slowdown) when using a fast Pi. Default is 1.
For Raspberry Pi 3 use a slowdown of 1 to start (use higher values if image still flickers).
For Raspberry Pi 4, use a slowdown of 4 or 2. Try what works best for your display.

There are tons more settings and tweaking mentioned in the README here:
https://github.com/hzeller/rpi-rgb-led-matrix/

NOTICE: These settings will only work if you installed hzellers library directly or through the rgb-matrix.sh file I included with this repo.
If you installaed from adafruits guide, the settings will not all work!


```
#-------------------------------------------------

# Configuration for the matrix
# - More info in ReadMe here https://github.com/hzeller/rpi-rgb-led-matrix
# - https://github.com/hzeller/rpi-rgb-led-matrix/blob/master/bindings/python/rgbmatrix/core.pyx

options = RGBMatrixOptions()
options.rows = 32
options.cols = 32
options.chain_length = 1
options.parallel = 1
options.hardware_mapping = 'regular'  # If you have an Adafruit HAT: 'adafruit-hat'
options.brightness = 50
#options.pwm_bits = 11    #default 11
options.pwm_lsb_nanoseconds = 200 #200
#options.scan_mode = 0    #default 0
#options.multiplexing = 0   #default 0, <1..17>
#options.row_address_type = 0   #default 0, <0..4>
#options.disable_hardware_pulsing = False   # debugging if nothing on panel - sound setting
options.show_refresh_rate = True
#options.inverse_colors = False
#options.led_rgb_sequence = "RGB"
#options.pixel_mapper_config = 
#options.panel_type = "FM6126A"   #Current supported types: FM6126A or FM6127
#options.pwm_dither_bits = 0    #default 0
options.limit_refresh_rate_hz = 200
options.gpio_slowdown = 4
#options.daemon = False    #  if it looks weird, reboot
options.drop_privileges = True

matrix = RGBMatrix(options = options)
#-------------------------------------------------
```




To stop the code from running, in terminal:

    Ctrl + C
