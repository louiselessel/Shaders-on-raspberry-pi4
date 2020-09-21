#include std_head_fs.inc
#define iTime unif[1][0]
#define iResolution unif[0]
#define iScale unif[1][1]
#define iMouse unif[3]

varying vec2 texcoordout;

float pixelScalar = 4.0;

// based on https://www.shadertoy.com/view/MsKfz3 by luka712
// https://luka712.github.io/2018/07/01/Pixelate-it-Shadertoy-Unity/

void main(void) {
    vec2 uv = texcoordout;
    
    float onePixelSizeX = 1.0 / iResolution.x;
    float onePixelSizeY = 1.0 / iResolution.y;
    
    float cellSizeX =  pixelScalar * onePixelSizeX;
    float cellSizeY = pixelScalar * onePixelSizeY;
    
    float x = cellSizeX * floor(uv.x / cellSizeX);
    float y = cellSizeY * floor(uv.y / cellSizeY);
    
    float shiftX = (pixelScalar * iScale) / iResolution.x;
    float shiftY = (pixelScalar * iScale) / iResolution.y;
    
    // you may have to change the shift values depending on your
    // iScale and pixelScalar values to get to best alignment
    //shiftX *= 0.5; // 2.0;
    //shiftY *= 0.5; // 2.0; 
    
    // color the pixelation
    gl_FragColor = texture2D(tex0, vec2(x + shiftX , y + shiftY));
    gl_FragColor.a = 1.0;
    
    
    // DEBUG checks
    //gl_FragColor = texture2D(tex0, vec2(x,y));
    //gl_FragColor = vec4(x, y, 0.0, 1.0); 
    //gl_FragColor = vec4(uv.x, 0.0, 0.0, 1.0);
}

