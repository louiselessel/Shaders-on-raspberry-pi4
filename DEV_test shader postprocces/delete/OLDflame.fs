
    #include std_head_fs.inc
    #define iResolution unif[0]
    #define iTime unif[1][0]
    #define iTimeDelta unif[1][0]
    #define iScale unif[1][1]
    #define iFrame unif[1][2]
    #define iMouse vec4(unif[2][0], unif[2][1], unif[3][0], unif[3][1])
    #define iDate vec4(unif[4][0], unif[4][1], unif[4][2], unif[5][0])
    
    //#define ownVar1 unif[16]
    //#define ownVar2 unif[19]
    
 


 

 
float noise(vec3 p) //Thx to Las^Mercury
{
    vec3 i = floor(p);
    vec4 a = dot(i, vec3(1., 57., 21.)) + vec4(0., 57., 21., 78.);
    vec3 f = cos((p-i)*acos(-1.))*(-.5)+.5;
    a = mix(sin(cos(a)*a),sin(cos(1.+a)*(1.+a)), f.x);
    a.xy = mix(a.xz, a.yw, f.y);
    return mix(a.x, a.y, f.z);
}

float sphere(vec3 p, vec4 spr)
{
    return length(spr.xyz-p) - spr.w;
}

float flame(vec3 p)
{
    float d = sphere(p*vec3(1.,.5,1.), vec4(.0,-1.,.0,1.));
    return d + (noise(p+vec3(.0,iTime*2.,.0)) + noise(p*3.)*.5)*.25*(p.y) ;
}

float scene(vec3 p)
{
    return min(100.-length(p) , abs(flame(p)) );
}

vec4 raymarch(vec3 org, vec3 dir)
{
    float d = 0.0, glow = 0.0, eps = 0.02;
    vec3  p = org;
    bool glowed = false;
    
    for(int i=0; i<64; i++)
    {
        d = scene(p) + eps;
        p += d * dir;
        if( d>eps )
        {
            if(flame(p) < .0)
                glowed=true;
            if(glowed)
                glow = float(i)/64.;
        }
    }
    return vec4(p,glow);
}

void main()
{
float invScale = 2.5 / iScale; // obviously scale must not be zero!
vec2 offset = vec2(invScale - 1.0) * 1.5;
    vec2 v = -1.0 + 2.0 * gl_FragCoord.xy / iResolution.xy;
    v.x *= iResolution.x/iResolution.y;
    
    vec3 org = vec3(0., -2., 4.);
    vec3 dir = normalize(vec3(v.x*1.6, -v.y, -1.5));
    
    vec4 p = raymarch(org, dir);
    float glow = p.w;
    
    vec4 col = mix(vec4(1.,.5,.1,1.), vec4(0.9,.7,0.7,1.), p.y*.02+.4);
    
    gl_FragColor = mix(vec4(0.), col, pow(glow*2.,1.));
    //gl_FragColor = mix(vec4(1.), mix(vec4(1.,.5,.1,1.),vec4(0.1,.5,1.,1.),p.y*.02+.4), pow(glow*2.,4.));

}


 