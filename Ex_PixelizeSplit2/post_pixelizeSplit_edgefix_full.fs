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
    
    // ------- Take care of edges
    // because uv gets changed with the iScale downscaling,
    // we need to find the new edge uv position, to recolor edges
    
    // Figure out the scaling for X
    float ratio_Res_pixelScalarX = iResolution.x/pixelScalar;        // 400/40 = 10
    float outsiders = 1.0 - iScale;                                 // 1 - 0.6 = .4 is how many we don't see
    float half_outsidersX = outsiders * ratio_Res_pixelScalarX * 0.5; // .4 * 10 * 0 .5 = 2 How many on either side of center
    float amtCellsX = iResolution.x / pixelScalar;                   // 10. However we only see iScale amt. so 0.6 = 6
    
    // and Y
    float ratio_Res_pixelScalarY = iResolution.y/pixelScalar;                                        
    float half_outsidersY = outsiders * ratio_Res_pixelScalarY * 0.5;
    float amtCellsY = iResolution.y / pixelScalar;                   

    
    // Where is the new right edge: pixel + (move to last cell) - (move back onto screen)
    float edgeUV_r = onePixelSizeX + (cellSizeX * amtCellsX) - (cellSizeX * half_outsidersX);
    
    // Left edge: pixel + (move onto screen)
    float edgeUV_l = onePixelSizeX + (cellSizeX * half_outsidersX);
    
    // Bottom edge: pixel + (move onto screen)
    float edgeUV_b = onePixelSizeY + (cellSizeY * half_outsidersY);
    
    // Top edge: pixel + (move to last cell) - (move back onto screen)
    float edgeUV_t = onePixelSizeY + (cellSizeY * amtCellsY) - (cellSizeY * half_outsidersY);
    
    
    // Figure out how wide the line should be
    float linewidthX = onePixelSizeX  + onePixelSizeX*0.5;
    float linewidthY = onePixelSizeY  + onePixelSizeY*0.5;


    // -------- Color the pixels
    
    // if half - make pixelated
    if (uv.x <= 0.5) {
        // color the pixelation
        //gl_FragColor = texture2D(tex0, vec2(x + shiftX , y + shiftY));
        gl_FragColor = texture2D(tex0, vec2(uv.x, uv.y)); // original
    }
    // Original pixels
    else {
        gl_FragColor = texture2D(tex0, vec2(uv.x, uv.y));
        //gl_FragColor = vec4(x, y, 0.0, 1.0); 
    }
    
    // Draw edges last
    // X
    if ((uv.x >= edgeUV_r - linewidthX)) {// && (uv.y >= 0.5)) {
        gl_FragColor = vec4(0.0, 0.0, 1.0, 1.0);
        //gl_FragColor = texture2D(tex0, vec2(uv.x-onePixelSizeX, uv.y));
        //gl_FragColor.a = 1.0;
    }
    if (uv.x <= edgeUV_l) {
        gl_FragColor = vec4(0.0, 0.0, 1.0, 1.0);
        //gl_FragColor = texture2D(tex0, vec2(uv.x+onePixelSizeX, uv.y));
        //gl_FragColor.a = 1.0;
    }
    // Y
    if (uv.y <= edgeUV_b) {
        gl_FragColor = vec4(0.0, 0.0, 1.0, 1.0);
        //gl_FragColor = texture2D(tex0, vec2(uv.x, uv.y+onePixelSizeY));
        //gl_FragColor.a = 1.0;
    }
    if (uv.y >= edgeUV_t - linewidthY) {
        gl_FragColor = vec4(0.0, 0.0, 1.0, 1.0);
        //gl_FragColor = texture2D(tex0, vec2(uv.x, uv.y-onePixelSizeY));
        //gl_FragColor.a = 1.0;
    }
    
    
    /*
    if ((uv.x >= edgeUV_r - linewidth) && (uv.x <= edgeUV)) {
        gl_FragColor = vec4(0.0, 0.0, 1.0, 1.0);
    }
    */
    
    
    // straight UV strangeness
    // gl_FragColor = vec4(uv.x,0.0, 0.0, 1.0);
    
    //gl_FragColor.a = 1.0;
    
    
    // DEBUG checks
    //gl_FragColor = texture2D(tex0, vec2(x,y));
    //gl_FragColor = vec4(x, y, 0.0, 1.0); 
    //gl_FragColor = vec4(uv.x, 0.0, 0.0, 1.0);
}

