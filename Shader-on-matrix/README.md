
# Shaders on the matrix - using python

Based on:
https://github.com/hzeller/rpi-rgb-led-matrix/tree/master/bindings/python

Tested on the raspberry pi 4. Using an Adafruit RGB Matrix HAT + RTC for Raspberry Pi
(https://www.adafruit.com/product/2345)



----

## Dependencies:

Follow this installation guide for the RGB Matrix

https://learn.adafruit.com/adafruit-rgb-matrix-bonnet-for-raspberry-pi/driving-matrices

My pi is set up for the "Adafruit HAT" and for "convenience".
After installation, this library has included python examples in the folder: bindings/python/samples
The code in this repo is based on these.


Now there are two things that must be on your pi for this repo to work.


### 1. Install pi3d + make a local copy of the library

Since this code builds upon the code in the "Shaders on raspberry pi4" repo, 
you must first follow the install steps for getting the pi3d library on your pi found here:
https://github.com/louiselessel/Shaders-on-raspberry-pi4#dependencies

Then, to make sure you can use pi3d no matter where you place the matrix code from this repo, you must also download the pi3d repo folder from the link below, and place it in the pi folder (folder that also has Desktop, Documents etc in it).
https://github.com/tipam/pi3d


Place the repo here. Delete the "-master" part of the name.

![Place pi3d local repo here](https://github.com/louiselessel/Shaders-on-raspberry-pi4/blob/master/Documentation/Screenshot_PlacementOfpi3d.png)



### 2. Then, install the imaging library called PIL

### In terminal:

    $ sudo apt-get update && sudo apt-get install python3-dev python3-pillow -y
    $ make build-python PYTHON=$(which python3)
    $ sudo make install-python PYTHON=$(which python3)


----

## How to

You can test the code in Thonny by pressing Run, but to take advantage of the settings for the matrix, the code must be run from terminal. So cd to the folder of this repo and then run the sudo command below.



### In terminal:
To run the simple example:
    $ cd /home/pi/Desktop/Shader-on-matrix/
    $ cd Ex_shader-on-matrix_simple
    $ sudo python3 ./shader-on-matrix-simple.py --led-gpio-mapping=adafruit-hat --led-gpio-slowdown=7

To run the All Uniforms example:
    $ cd /home/pi/Desktop/Shader-on-matrix/
    $ cd Ex_shader-on-matrix_All-Uniforms/
    $ sudo python3 ./shader-on-matrix-All-Uniforms.py --led-gpio-mapping=adafruit-hat --led-gpio-slowdown=7



You may have to experiment with different values for the slowdown
--led-slowdown-gpio=(0…n)

This is sometimes needed to throttle back the speed when using a fast Pi. Default is 1.
For Raspberry Pi 3 use a slowdown of 1 to start (use higher values if image still flickers). For Raspberry Pi 4, use a slowdown of 4 (Though I found 7 is sometimes good). Older Pi models might work with 0, try it.

More info here https://learn.adafruit.com/adafruit-rgb-matrix-bonnet-for-raspberry-pi/driving-matrices.


There are tons more settings and tweaking mentioned in the README here:
https://github.com/hzeller/rpi-rgb-led-matrix/



To stop the code from running, in terminal:

    $ Ctrl + C
