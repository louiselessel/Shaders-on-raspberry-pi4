// The MIT License
// Copyright © 2019 Inigo Quilez
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


// How to sample from a fake 3D texture made of multiple 2D textures, without
// branching in systems that do not support texture arrays, texture layers or
// 3D textures. You need to sample all the Z layers, but that can be faster
// than conditionally sampling only the 2 closest Z layers when the size of
// the texture in the Z direction is small. It can also prevent mipmapping
// issues if diferent pixels in screen space are sampling from the virtual 3D
// texture with different Z coordinates.
//
// Change METHOD to compare the following techniques:
//
// 0: branchless  sampling, branchless  selection
// 1: branchless  sampling, conditional selection
// 2: conditional sampling, branchless  selection
//
#define METHOD 0

#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 iMouse;

uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform sampler2D iChannel3;

out vec4 fragColor;


void main()
{
    // 3D texture coordinates to sample from
    vec3 u = vec3(gl_FragCoord.xy/iResolution.xy, fract(iTime*0.34) );
    
    float w = u.z*4.0;

    // branchless sampling, branchless selection
    #if METHOD==0
    { 
        vec4 colZ0 = texture( iChannel0, u.xy );
        vec4 colZ1 = texture( iChannel1, u.xy );
        vec4 colZ2 = texture( iChannel2, u.xy );
        vec4 colZ3 = texture( iChannel3, u.xy );

        fragColor = colZ0*max(1.0-abs(w-0.0),0.0) + 
                    colZ1*max(1.0-abs(w-1.0),0.0) +
                    colZ2*max(1.0-abs(w-2.0),0.0) +
                    colZ3*max(1.0-abs(w-3.0),0.0) +
                    colZ0*max(1.0-abs(w-4.0),0.0); 
    }
    #endif
    
    // branchless sampling, conditional selection
    #if METHOD==1
    {
        vec4 colZ0 = texture( iChannel0, u.xy );
        vec4 colZ1 = texture( iChannel1, u.xy );
        vec4 colZ2 = texture( iChannel2, u.xy );
        vec4 colZ3 = texture( iChannel3, u.xy );

        float iz = floor(w);
        float fz = fract(w);

             if( iz<1.0 ) fragColor = mix( colZ0, colZ1, fz );
        else if( iz<2.0 ) fragColor = mix( colZ1, colZ2, fz );
        else if( iz<3.0 ) fragColor = mix( colZ2, colZ3, fz );
        else              fragColor = mix( colZ3, colZ0, fz );
    }
    #endif
    
    // conditional sampling, branchless selection
    #if METHOD==2
    {
        float iz = floor(w);
        float fz = fract(w);

        vec4 colA, colB;
             if( iz<1.0 ) { colA = texture( iChannel0, u.xy ); colB = texture( iChannel1, u.xy ); }
        else if( iz<2.0 ) { colA = texture( iChannel1, u.xy ); colB = texture( iChannel2, u.xy ); }
        else if( iz<3.0 ) { colA = texture( iChannel2, u.xy ); colB = texture( iChannel3, u.xy ); }
        else              { colA = texture( iChannel3, u.xy ); colB = texture( iChannel0, u.xy ); }

        fragColor = mix( colA, colB, fz );
    }
    #endif
}
