//https://www.shadertoy.com/view/ttsXW7

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

//TEXTURE1:metal.png

/*


	Random Pipe System
	------------------

	Using the standard 2-edge Wang tile concept -- along with some basic layering 
	techniques -- to create a system of pipes, rendered in an oldschool faux 3D 
	style. I'm not sure if this a rendering of wall pipes, or an overhead camera
	sweep of floor pipes. :)

	I put together a simple pipe system based on Truchet concepts a while back,
	which still looks interesting, but it lacks the variable density randomness
	that Wang tile techniques provide.

	Since this was a effectively an upgrade on an earlier example, I wanted to 
	improve the visuals. Not always, but that often involves more effort, which
	translates to more code. Therefore, this is not the most readable example.
	However, I went to the trouble of providing a very basic version to accompany
	this. I've provided a link below, for anyone who doesn't wish to decode the 
	haphazard logical mess to follow. :)

	I intend to produce a 3D single layered version of this, but thought it'd be 
	fun to put together a fake isometric-looking one first. Believe it or not, the
	3D equivalent will be a lot easier to produce, but will probably require some 
	distance field tweaking to keep the frame rate up.



    Simplified pipe version:

	Simple Wang Tile Example - Shane
	https://www.shadertoy.com/view/ttXSzX

	
	Other examples:

    // Put together ages ago. Demofox was doing it before it was cool. :D
	Wang Tiling 2D - demofox 
	https://www.shadertoy.com/view/MssSWs

	// Like all his examples, it's concise and stylish. I put together one of these
	// and the 2-corner version a while back, which I'll put up at some stage.
	2-edge Wang Tiles - srtuss
	https://www.shadertoy.com/view/Wds3z7

    // This one incorporates a few concepts.
	Double Simplex Wang Weave - Shane
	https://www.shadertoy.com/view/tl2GWz


	Wang tile resources:

    // Possibly the best Wang tile resource on the net. 
	http://www.cr31.co.uk/stagecast/wang/intro.html


*/


 
// Standard 2D rotation formula.
mat2 rot2(in float a){ float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }


// IQ's vec2 to float hash.
float hash21(vec2 p){  return fract(sin(dot(p, vec2(27.609, 57.583)))*43758.5453); }


// Cheap and nasty 2D smooth noise function with inbuilt hash function -- based on IQ's 
// original. Very trimmed down. In fact, I probably went a little overboard. I think it 
// might also degrade with large time values.
float n2D(vec2 p) {

	vec2 i = floor(p); p -= i; p *= p*(3. - p*2.); // p *= p*p*(p*(p*6. - 15.) + 10.); //
    
	return dot(mat2(fract(sin(vec4(0, 1, 113, 114) + dot(i, vec2(1, 113)))*43758.5453))*
                vec2(1. - p.y, p.y), vec2(1. - p.x, p.x) );

}


// Texture function.
vec3 doTex(vec2 p){
    
    vec3 tx = texture(iChannel0, p).xyz; // sRGB texture read.
    return tx*tx; // Rough sRGB to linear conversion.
}
    

// Use the unique edge point IDs to produce a Wang tile ID for the tile.
float edges(vec2 ip, vec2[4] ep, float rnd){
    
    // Starting from the left and heading clockwise, generate a unique random number
    // for each edge, then test it against a threshold. If it is above that threshold,
    // flag that edge and use some standard bit encoding to produce an ID for that tile.
    // For a 2-edge system, there will be sixteen combinations in total, each of which
    // are represented by a four bit binary string encoded into integer form.
    //
    // For instance, a tile with an ID of 5 will convert to the binary string "0101,"
    // which will indicate that you need to construct a tile that uses the first (left)
    // edge midpoint and the third (right) edge midpoint. What is constructed is up to 
    // the individual. Since this is a simple example, we'll simply render a line from 
    // the left edge to the right edge. If it were diagonal edges, we could render a
    // curved edge, and so forth.
    //
    // Since tiles share edges, you're guaranteed that neighboring tiles will connect.
    //
    // For a much better explanation that will usually include images, look up two-edge 
    // Wang tiles on the net. There are many references out there, but I prefer the 
    // explanation provided here:
    //
    // 2-edge Wang Tiles
    // http://www.cr31.co.uk/stagecast/wang/2edge.html
    
    // Initial ID: Trivial, and converts to a binary string of "0000," which indicates
    // the cell has no edge points, or an empty tile.
    float id = 0.;
    
    
    // Note: exp2(i) = pow(2., i).
    for(int i = 0; i<4; i++) id += hash21(ip + ep[i])>rnd? exp2(float(i)) : 0.;
    
    /* 
    // The above line is equivalent to the following:
    vec4 e;
    for(int i = 0; i<4; i++) e[i] = hash21(ip + ep[i]);
    
    if(e.x>rnd) id += 1.; // Left edge.
    if(e.y>rnd) id += 2.; // Top edge.
    if(e.z>rnd) id += 4.; // Right edge.
    if(e.w>rnd) id += 8.; // Bottom edge.
	*/ 
    
    return id; // Range [0-15] inclusive.
    
}


// vec4 swap.
//void swap(inout vec2 a, inout vec2 b){ vec2 tmp = a; a = b; b = tmp; }

 // Unsigned distance to the segment joining "a" and "b".
float distLine(vec2 a, vec2 b){
    
	b = a - b;
	float h = clamp(dot(a, b)/dot(b, b), 0., 1.);
    return length(a - b*h);
}


// IQ's signed box formula.
float sBox(vec2 p, vec2 b, float r){
  
  // Just outside lines.
  //p = max(abs(p) - b + r, 0.);
  //return length(p) - r;

  // Inside and outside lines.
  vec2 d = abs(p) - b + r;
  return min(max(d.x, d.y), 0.) + length(max(d, 0.)) - r;
}


// A stretched leaf for the grid nodule.
float leaf(vec2 p, float a){
    
    p *= rot2(6.2831*a); // Rotate.
    p.y = max(abs(p.y) - .125, 0.); // Elongate.

    return (length(p) + abs(p.x))/1.4142; // Leaf.
}

// The grid pattern. Just two overlapping grids of rotated
// leaves, rendered 90 degrees to one another. 
float gridPat(vec2 p, vec2 ip){
    
    //if(mod(ip.x + ip.y , 2.)>.5) p.x = -p.x;;
    
    // Scale the grid, and offset it by half.
    p = p*5. - .5;
    
    // Leaf one.
    vec2 q = p, iq = floor(q); q -= iq + .5;
    float d = leaf(q, 1./8.) - .1;
    
    // Leaf two.
    q = p + vec2(.5, .5), iq = floor(q); q -= iq + .5;
    float d2 = leaf(q, -1./8.) - .1;
    
    /*
    // Not rendering on the edges.
    if(iq.x==-3.) d2 = 1e5;
    if(iq.x==2.) d2 = 1e5;
    if(iq.y==-3.) d2 = 1e5;
    if(iq.y==2.) d2 = 1e5;
    */
    
    return min(d, d2); // Combine the leaves.
    
}

// Random tap rendering in the three and four end-point cells.
float doTap(int iNum, int bend, vec2 ip){
   
    // Checker pattern, so that the taps never sit next
    // to one another.
    float ch = mod(dot(ip, vec2(1)), 2.);
    return (iNum>=3 && bend==0 && hash21(ip + 5.)>.7 && ch>.5)? 1. : 0.;
    //return (iNum>=3 && bend==0 && ch>.5)? 1. : 0.;
    
}

// Random guage rendering in the straight two end-point cells.
float doGuage(int iNum, int bend, vec2 ip){
   
    // Checker pattern, so that the guages never sit next
    // to one another.
    float ch = mod(dot(ip, vec2(1)), 2.);
    return (iNum==2 && bend==0 && hash21(ip + 173.)>.5 && ch>.5)? 1. : 0.;
    
}

// vec4 swap.
//void swap(inout vec2 a, inout vec2 b){ vec2 tmp = a; a = b; b = tmp; }


// Distance field-related struct: Containers are handy, but I try to avoid them in
// shaders for readability sake. However, there were too many variables I wanted 
// to return from the function to avoid its usage.
struct ds{
    
    float grid; // Grid.
    float pat; // Grid pattern.
    
    float ln; // Pipe line.
    float ep; // End point sleaves.
    // Three circles: The round central point for the single end-point
    // cells, the taps, and the guages.
    vec3 ci; 
    
    //float id; // Wang tile ID. Not used here.
    vec2 ip; // Unique grid ID.
    
    int iNum; // Tile end-point number. Range: [0-4].
    int bend; // Pipes can either be straight or curved.
    
};

ds wang(vec2 p){

    // The stuct to hold the Wang tile information to return for rendering. 
    ds di;

   
    di.ip = floor(p); // Grid ID.
    p -= di.ip + .5; // Local coordinates.
    
    vec2 q = p; // Local variable holding variable.
    
    // Grid pattern. This was calculated here, because I'd originally wanted to 
    // bump map it, but I changed my mind later.
    di.pat = gridPat(q, di.ip);
    
    // The grid squares.
    q = abs(p);
    di.grid = abs(max(q.x, q.y) - .5) - .01;
    
    
    // Wang tile construction.
    
    // Four edge midpoints: Clockwise from the left.
    vec2[4] eps = vec2[4](vec2(-.5, 0), vec2(0, .5), vec2(.5, 0), vec2(0, -.5));
    vec2[4] cp = eps; // Holding points.
    
    // Get the Wang tile ID. The random number effects the density of the pipe
    // distribution.
    const float rnd = .45;
    float id = edges(di.ip, eps, rnd);
    
    // Decode each binary digit.
    vec4 bits = mod(floor(id/vec4(1, 2, 4, 8)), 2.);
    
    di.iNum = 0; // Edge point index.
    
    for(int i = 0; i<4; i++){
        // If the edge bit is flagged, add an end point to the array, whilst 
        // increasing the array index. By the way, we could combine more of 
        // these steps in the "edges" function, but I wanted to show the encode 
        // and decode process.
        if(bits[i]>.5) cp[di.iNum++] = eps[i]; 
        
    }
    
    
    di.ep = 1e5; //  Midpoint end-point sleeves.
    di.ln = 1e5; // The pipes themselves.
    di.ci = vec3(1e5); // The footers, taps and guages.
    
    const float lw = .16; // Pipe line width.
 
    
    // Edge point joins and the boxes to represent the pipes.
    vec2 join = vec2(lw*.6, lw + .025);
    vec2 boxLine = vec2(.25 + lw, lw);
    

    
    q = p; // Set "q" to the cell's local coordinates.
    
    // Is the tile going to contain a curved pipe?
    di.bend = 0;
    if(di.iNum==2 && length(cp[0] - cp[1])<.99) di.bend = 1; // Points on diagonal or vertical.
    // Four end point tiles can contain a cross pipe, or two curved pipes, which can be
    // randomly rotated as well.
    if(di.iNum==4 && hash21(di.ip + 7.)>.5){
        // Randomly orient some of the pipe pairs the opposite way for more variation.
        if(hash21(di.ip + 27.)>.5) q.y = -q.y; // swap(cp[1], cp[3]);
        di.bend = 1;
    }
    
    for(int i = 0; i<4; i++){
        if(bits[i]>.5){
            di.ep = min(di.ep, sBox(q - eps[i], join, .035));
            if(di.bend==0) di.ln = min(di.ln, sBox(q - eps[i]/2., boxLine, lw));
        }
        
        // Because we're heading clockwise, we need to reorient the the joins, etc.
        join = join.yx;
        boxLine = boxLine.yx;
    } 
    
    // If necessary, construct one curved pipe between two end-points, or two
    // if there are four end-points.
    if(di.bend==1){
        
        vec2 pnt;
        float rf = di.iNum==2? .3 : .4; // Sharper bends for just one curved pipe.
        pnt.x = abs(cp[0].x)>abs(cp[1].x)? cp[0].x : cp[1].x;
        pnt.y = abs(cp[0].y)>abs(cp[1].y)? cp[0].y : cp[1].y;
        //ln = min(ln, abs(length(q - pnt) - .5) - lw);
        di.ln = min(di.ln, abs(sBox(q - pnt, vec2(.5), rf)) - lw);
        
        if(di.iNum==4){ // The second curved pipe, if applicable.
            pnt.x = abs(cp[2].x)>abs(cp[3].x)? cp[2].x : cp[3].x;
            pnt.y = abs(cp[2].y)>abs(cp[3].y)? cp[2].y : cp[3].y;
            //ln = min(ln, abs(length(q - pnt) - .5) - lw);
            di.ln = min(di.ln, abs(sBox(q - pnt, vec2(.5), rf)) - lw);
        }

    }    
    
    // Construct the pipe footers at the center of the single end-point tiles.
    if(di.iNum==1) di.ci.x = min(di.ci.x, length(q) - lw - .095);
    
    // Tap... at some of the junctions? It seems like a pipe system thing to do. :D
    if(doTap(di.iNum, di.bend, di.ip)>.5){
       
        
        // Randomly oriented, to show they've been turned.
        q = rot2(3.14159*hash21(di.ip + 9.))*p;
       
        // The main body of the tap.
        di.ci.y = min(di.ci.y, length(q) - .24);
        
        // Tap nodules, spread around in a hexagonal fashion.
        //
        // Standard repeat polar cells.
     	const float rad = .22;
    	const float aNum = 6.;
    	float a = atan(q.y, q.x);
        float ia = floor(a/6.283*aNum) + .5; // .5 to center cell.
        q = rot2(ia*6.283/aNum)*q;
        q.x -= rad;
        
        float s = sBox(q, vec2(.08, .08), .06);
        di.ci.y = min(di.ci.y, s);
  
    }
    
    // Putting a pressure guage, or something like that, on some of the straight pipes.
    if(doGuage(di.iNum, di.bend, di.ip)>.5){
        
        di.ci.z = min(di.ci.z, length(q) - .3);
 
    }
    
    // Return the distance field-related struct.
    return di;
    
}


// Shorthand for this particular expression, which gets used a lot.
#define ss(a, b) 1. - smoothstep(0., a, b)

void main(){

    // Aspect correct screen coordinates. Setting a minumum resolution on the
    // fullscreen setting in an attempt to keep things relatively crisp.
    float iRes = min(800., iResolution.y);
    vec2 uv = (gl_FragCoord.xy - iResolution.xy*.5)/iRes;
    
    // Scaling and translation.
    const float gSc = 6.;
    vec2 p = uv*gSc - vec2(-iTime, cos(iTime/8.)*2.);
    
    // Smoothing factor, based on resolution and scaling factor.
    float sf = 1./iRes*gSc;
    
    // Taking some Wang tile samples, for rendering.
    vec2 e = vec2(.015, .03);
    float le = length(e);
    
    ds di = wang(p); // Standard sample.
    ds di2 = wang(p - e); // Near offset for highlight calulations.
    ds dis = wang(p + 5.333*e); // Opposite, larger offset for shadows.
    
    
    ds diHi; // Holding container for highlight calculations.

    // Background texture.
    vec3 txPat = doTex(p/gSc + e);
    // Scene color, pipe color and end-point sleeve color.
    vec3 col = txPat*2.*vec3(1, .85, .7);
    vec3 lCol = vec3(1, .7, .4)*mix(vec3(1), txPat*3., .5);
    vec3 sCol = vec3(1, .6, .3);

    
    
    vec3 tCol = col; // Temporary holding color.
    
    // Background grid pattern.
    //col = mix(col, vec3(0), (ss(sf*6.*4.,  max(di.pat - .02*6., -(di.grid - .07))))*.5);
    //col = mix(col, vec3(0), (ss(sf*6.,  max(di.pat - .02*6., -(di.grid - .07))))*.75);
    //col = mix(col, tCol*2., (ss(sf*6., max(di.pat, -(di.grid - .07))))*.7);
    col = mix(col, vec3(0), (ss(sf*6.*4., di.pat - .02*6.))*.5);
    col = mix(col, vec3(0), (ss(sf*6.,  di.pat - .02*6.))*.75);
    col = mix(col, tCol*2., (ss(sf*6., di.pat))*.7);
    
    // The grid.    
    diHi.grid = max(smoothstep(0., sf*4., di2.grid - .005) - 
                smoothstep(0., sf*4., di.grid - .005), 0.)/le*.015;
    col += diHi.grid;
    col = mix(col, vec3(0), (ss(sf*4., di.grid - .02))*.5);
    col = mix(col, vec3(0), (ss(sf, di.grid)));
    //col = mix(col, vec3(0), ss(sf, abs(di.grid - .07) - .01));
    
    // The drop shadow consists of all the combined major elements. we
    // take the minimum of all, then render the blurred silhouette onto
    // ground layer. Without it, this example would really lack depth.
    // Comment the shadow layer out, and you'll see what I mean.
    float shadow = min(min(min(dis.ci.x, dis.ci.y), dis.ci.z), dis.ln);
    shadow = min(shadow, dis.ep);
    col = mix(col, vec3(0), (ss(sf*8., shadow - .02))*.6);
    
    // Pipe footers, at the center of the single end-point tiles.
    col = mix(col, vec3(0), (ss(sf*4., di.ci.x - .02))*.5);
    col = mix(col, vec3(0), ss(sf, di.ci.x));
    col = mix(col, lCol, ss(sf, di.ci.x + .03));
    col = mix(col, vec3(0), ss(sf, abs(di.ci.x + .06) - .01));
    
    
    // Pipes with fake AO and highlights.
    vec3 txLn = doTex(p/gSc);
    txLn *= vec3(1, .85, .7);
    float pat2 = clamp(cos(di.ln*6.2831*24.) + .75, 0., 1.)*.2 + .9;
    float sh = max(.35 - (di.ln + .03)*12., 0.);
    diHi.ln = max(di.ln - di2.ln, 0.)/le;
    col = mix(col, vec3(0), (ss(sf*4., di.ln - .04))*.5);
    col = mix(col, vec3(0), ss(sf, di.ln));
    col = mix(col, txLn*(diHi.ln + sh*sh*.25 + .25)*pat2, ss(sf, di.ln + .025));
    col = mix(col, col*2., (ss(sf*4., di2.ln + .125))*.8);
    //col = mix(col, vec3(0), ss(sf, abs(di.ln + .12) - .01));
    
    
    // Pipe joins, with fake AO and highlights.
    sh = max(.05 - (di.ln + .02)*12., 0.);
    //diHi.ep = max(di.ep - di2.ep, 0.)/le;
    col = mix(col, vec3(0), (ss(sf*4., di.ep - .02))*.5);
    col = mix(col, vec3(0), ss(sf, di.ep));
    col = mix(col, sCol*(diHi.ln*diHi.ln + sh*sh*.5 + .25)/2., ss(sf, di.ep + .025));
    col = mix(col, col*3., (ss(sf*4., max(di.ep, di2.ln + .125)))*.8);
    //col = mix(col, vec3(0), (ss(sf, abs(di.ep + .04) - .01))*.75);
    col = mix(col, vec3(0), ss(sf, max(di.ep, di.grid)));

     
    // Flow tap, or whatever it's called. :)
    if(doTap(di.iNum, di.bend, di.ip)>.5){
        
        vec2 q = fract(p) - .5;
        
        // The main tap background.
        diHi.ci.y = max(di.ci.y - di2.ci.y, 0.)/length(e);
        sh = max(.75 - - di.ci.y*4., 0.);
        col = mix(col, vec3(0), (ss(sf*4., di.ci.y - .04))*.5);
        col = mix(col, vec3(0), ss(sf, di.ci.y));
        col = mix(col, lCol*(diHi.ci.y + sh*sh*.1 + .5), ss(sf, di.ci.y + .03));
         
        // Extra rings.
        col = mix(col, vec3(1), (ss(sf, abs(length(q) - .12) - .02))*.2);
        col = mix(col, vec3(0), ss(sf, abs(di.ci.y + .16) - .015));
        col = mix(col, vec3(0), ss(sf, length(q) - .03));

        // Subtle highlights.
        col = mix(col, col*2., (ss(sf*4., di.ci.y + .125))*.7);
        col = mix(col, vec3(0), ss(sf, abs(di.ci.y + .08) - .01));
        
        
    }
    
    // Guage, or dial. A lot of it is made up on the spot, with a touch of common sense thrown in. :)
    // There'd be more efficient ways to get this done, but not too many pixels are effected, plus
    // this isn't a taxing example to begin with.
    if(doGuage(di.iNum, di.bend, di.ip)>.5) {
        
        // Local coordinates.
        vec2 q = fract(p) - .5;

        // Backface with a bit of highlighting.
        diHi.ci.z = max(di.ci.z - di2.ci.z, 0.)/length(e);
        sh = max(.75 - di.ci.z*4., 0.);
        col = mix(col, vec3(0), (ss(sf*4., di.ci.z - .04))*.5);
        col = mix(col, vec3(0), ss(sf, di.ci.z));
        col = mix(col, lCol*(diHi.ci.z + sh*sh*.1 + .5), ss(sf, di.ci.z + .03));
        
        
        // More rings in the center.
    	col = mix(col, col*1.6, ss(sf*4., di.ci.z + .125));
    	col = mix(col, vec3(0), ss(sf, abs(di.ci.z + .08) - .01));
       
        col = mix(col, vec3(0), (ss(sf, length(q) - .08))*.5);
        col = mix(col, vec3(0), ss(sf, abs(length(q) - .1) - .01));
        col = mix(col, vec3(0), ss(sf, length(q) - .05));
        
        
        // Constructing the red indicator at a random angle, and providing some animation,
        q = rot2(6.2831*hash21(di.ip + 31.) + (hash21(di.ip + 19.)*.8 + .2)*sin(iTime))*q;
        float ind = distLine(q - vec2(0, -.005), q - vec2(0, .16)) - .0025;
        
        // Constructing the clock-like markings on the dial using standard repeat polar
        // coordinates.
        q = fract(p) - .5;
    	const float rad = .16;
    	const float aNum = 12.;
        q = rot2(3.14159/aNum)*q;
    	float a = atan(q.y, q.x);
        float ia = floor(a/6.283*aNum) + .5; // .5 to center cell.
        ia = ia*6.283/aNum;
        q = rot2(ia)*q;
        q.x -= rad;
        
        // Markings.
        float mark = sBox(q, vec2(.025, .015), 0.);
        col = mix(col, vec3(.5), ss(sf, mark - .015));
        col = mix(col, vec3(0), ss(sf, mark));
        
        // Indicator.        
        col = mix(col, vec3(0), ss(sf, ind - .025));
        col = mix(col, vec3(.5, 0, 0), ss(sf, ind));
        
        
    }
 
    
    // Very subtle sepia tone with a sprinkling of noise, just to even things up a bit more.
    col *= vec3(1.1, 1, .9)*(n2D(p*64.)*.8 + .6);
  
    // Subtle vignette.
    uv = gl_FragCoord.xy/iResolution.xy;
    col *= pow(16.*(1. - uv.x)*(1. - uv.y)*uv.x*uv.y, 1./16.)*1.05;
    
    // Rough gamma correction before presenting to the screen.
    fragColor = vec4(sqrt(max(col, 0.)), 1);

}
