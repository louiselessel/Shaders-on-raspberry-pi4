
## NOTES

"""
The path to the shaders that come with the pi3d library on your pi, in case you wanna have a look:
/usr/local/lib/python3.7/dist-packages/pi3d
"""


"""
HOW TO
pass uniforms into postprocessing shaders - a post proccesing named 'post'
see example in Ex_Pixelize folder

post.draw({0:W, 1:H, 4:SCALE, 6:0.9, 7:0.5})

"""

"""
In Pi3d these are used as follows, but since I am rendering shaders flat and dont need lighting or anything else...
I use their addresses differently.
Left side in .fs file, right side in .py file.

Adresses for passing in own uniforms

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
