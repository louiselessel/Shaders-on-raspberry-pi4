#include std_head_fs.inc
#define iTime unif[1][0]
#define iResolution unif[0]
#define iScale unif[1][1]
#define iMouse unif[3]

varying vec2 texcoordout;

float pixelScalar = 10.0;

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
    
    // find edges based on iScale
    // uv.x at 0.8 is going to land at new uv.x = 0.875, if iScale is 0.8
    
    // DEBUG ---
    //gl_FragColor = vec4(0.0, 0.0, uv.x, 1.0);
    
    float invScale = 1.0 / iScale; 
    vec2 offset = vec2(invScale - 1.0) * 0.5;
    
    // edge uv is usually at 0.99 -> but now it lands at
    float uvAtEdge = 0.9;
    //float edgeUV = (invScale / uvAtEdge) - offset.x;
    //float edgeUV = invScale - offset.x * uvAtEdge;
    float edgeUV = 0.9;
    // so we get it from there
    
    //float scaledUV_x = iScale * uv.x;
    
    
    // if half
    if (uv.x <= 0.5) {
        // color the pixelation
        gl_FragColor = texture2D(tex0, vec2(x + shiftX , y + shiftY));
    }
    else if ((uv.x >= edgeUV) && (uv.x <= edgeUV+0.02)) {
        gl_FragColor = vec4(0.0, 0.0, uv.x, 1.0);
    }
    // original pixel
    else {
        gl_FragColor = texture2D(tex0, vec2(uv.x, uv.y));
    }
    
    
    
    // straight UV strangeness
    // gl_FragColor = vec4(uv.x,0.0, 0.0, 1.0);
    
    gl_FragColor.a = 1.0;
    
    
    // DEBUG checks
    //gl_FragColor = texture2D(tex0, vec2(x,y));
    //gl_FragColor = vec4(x, y, 0.0, 1.0); 
    //gl_FragColor = vec4(uv.x, 0.0, 0.0, 1.0);
}

