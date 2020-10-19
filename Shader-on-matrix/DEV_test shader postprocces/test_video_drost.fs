
	#include std_head_fs.inc
    #define iResolution unif[0]
    #define iTime unif[0][2]
    #define iTimeDelta unif[1][0]
    #define iScale unif[1][1]
    #define iFrame unif[1][2]
    #define iMouse vec4(unif[2][0], unif[2][1], unif[3][0], unif[3][1])
    #define iDate vec4(unif[4][0], unif[4][1], unif[4][2], unif[5][0])
    
    //#define ownVar1 unif[16]
    //#define ownVar2 unif[19]
	
 


 

 
void main()
{
	float invScale = 1.0 / iScale; // obviously scale must not be zero!
	vec2 offset = vec2(invScale - 1.0) * 0.5;
    // Customizable Parameters
	float repeat = 8.0;			// How many times it should be repeated.
    
    // UV in 0..1 space and also inverted UV (1..0).
    vec2 uv = ( gl_FragCoord.xy / iResolution.xy ) * invScale - offset; // normalization of sampling coordinates - you may have to delete offset if black output. And make sure the scaling is only added to the line that looks something like: vec2 name = gl_FragCoord/iResolution
    vec2 uv_inverted = 1.0 - uv;
    
    // Calculate the offset_multiplier using the current uv.
    // Since droste works by only repeating when both uv parts are above a threshold,
    // we can just use the min() operator twice on it to get the final multiplier.
    vec2 fiter = floor(uv * repeat * 2.0);			// Calculate forward UV multiplier.
    vec2 riter = floor(uv_inverted * repeat * 2.0);	// Calculate reverse UV multiplier.
    vec2 iter = min(fiter, riter);					// iter = min(forward UV, reverse UV)
    float offset_mul = min(iter.x, iter.y);			// min(iter.x, iter.y) - Our Droste multiplier.
    
    // Offset is calculated by taking half the view and dividing it by the amount of repeats,
    //  and then we multiply it by the multiplier above.
    vec2 offset = (vec2(0.5, 0.5) / repeat) * offset_mul;
    
    // The final step is to fix the UVs that are inside a Droste offset back to 0..1 range.
    // You can do this by calculating the new drawable area and divide 1.0 by this area.
    vec2 uv_mul = 1.0 / (vec2(1.0, 1.0) - offset * 2.0);
    
    // Calculate the final UV: Screen UV minus Offset, clamped to 0..1, times UV Multiplier.
    // The clamp could technically be skipped if out-of-bounds UVs are not a problem.
    //vec2 finaluv = clamp(uv - offset, 0.0, 1.0) * uv_mul;
    vec2 finaluv = (uv - offset) * uv_mul;
    
    // And now just read the texture.
	gl_FragColor = texture(iChannel0, finaluv);
}
 