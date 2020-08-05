#include std_head_fs.inc
#define iTime unif[1][0]
#define iResolution unif[0]
#define iScale unif[1][1]
#define iMouse unif[3]

varying vec2 texcoordout;

float pixelScalar = 1.0;

// based on https://www.shadertoy.com/view/MsKfz3

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
    
    // if there is a remainder on the scaling just color it the next pixel
    //gl_FragColor = vec4(uv.x, 0.0, 0.0, 1.0); 
    
    // else
    // color the pixelation
    gl_FragColor = texture2D(tex0, vec2(x + shiftX , y + shiftY));
    
    
    
    
    // DEBUG checks
    //gl_FragColor = texture2D(tex0, vec2(x,y));
    //gl_FragColor = vec4(x, y, 0.0, 1.0); 
    

    /*
    if (uv.y < 0.46) {
    //if (uv.y < 0.1) {
		//gl_FragColor = vec4(0.0, 0.0, 1.0, 1.0);
		gl_FragColor = texture2D(tex0, vec2(x,y+0.005));
		//gl_FragColor = texture2D(tex0, vec2(x,y));
	}
	// else if (uv.x < 0.45) {
	else if (uv.x < 0.46) {
		gl_FragColor = vec4(0.0, 0.0, 1.0, 1.0);
		//gl_FragColor = texture2D(tex0, vec2(x,y));
    }
    else {  
		gl_FragColor = texture2D(tex0, vec2(x,y));
    }
    */
    
    gl_FragColor.a = 1.0;
}

