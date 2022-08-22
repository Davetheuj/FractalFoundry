Shader "Foundry/Mandelbrot"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Area("Area", vector) = (0,0,4,4) 
        _Angle ("Angle", range(-3.1415, 3.1415)) = 0
        _Iter ("Iter", float) = 1000
        _Color ("Color", range(0,1)) =.5
        _Repeat ("Repeat", float) = 1
        _IsGradient ("IsGradient", float) = 0
        _RValue ("RValue", float) = 70
        _GValue ("GValue", float) = .5
        _BValue ("BValue", float) = .5
        _JuliaNValue("NValue", float) = 3
        _FractalType ("FractalType", int) = 0
        _Tolerance ("Tolerance", float) = 0.000001
        _XMod ("XMod", float) = 1
        _YMod ("YMod", float) = 1

    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
//#pragma exclude_renderers d3d11 gles
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            float4 _Area;
            float _Angle;
            sampler2D _MainTex;
            float _Iter;
            float _Color;
            float _Repeat;
            float _IsGradient;
            float _RValue;
            float _GValue;
            float _BValue;
            int _FractalType;
            float2 difference;//newton
            float _Tolerance;//newton
            
            float _XMod; //XMod and Ymod allow for slight variations within the fractals overall design

            float _YMod;

            float4 col;
           
            float2 rot(float2 p, float2 pivot, float a){
            float s = sin(a);
            float c = cos(a);
            p -= pivot;
            p = float2(p.x*c - p.y*s ,p.x*s+p.y*c);
            p+= pivot;

            return p;
            }

            float2 CDiv (float2 numerator, float2 denominator)
{
   return  float2(((numerator.x * denominator.x)+(numerator.y*denominator.y)/(denominator.x*denominator.x+denominator.y*denominator.y)), ((numerator.y*denominator.x-numerator.x*denominator.y)/(denominator.x*denominator.x+denominator.y*denominator.y)));


}


float2 CMul (float2 a, float2 b)
{
   return float2(a.x*b.x-a.y*b.y,a.x*b.y+b.x*a.y);
}

            fixed4 frag (v2f i) : SV_Target
            {
              float2  c = _Area.xy + (i.uv-.5)*_Area.zw;
              
              c = rot(c,_Area.xy,_Angle);
            
              float2 z;
              float zy;
              float zx;
              float xTemp;

             if(_FractalType <=1){
              
              float n =3;
             
              for(int iter; iter< _Iter; iter++){
             
              if(_FractalType == 0){
              z = float2(z.x*z.x-z.y*z.y*_XMod,2*z.x*z.y*_YMod) + c; //MANDELBROT
              }
             /*xTemp = pow(z.x*z.x+z.y*z.y, n/2) * cos(n *atan2(z.y,z.x)) + c.x;
             zy = pow(z.x*z.x+z.y*z.y, n/2) * sin(n *atan2(z.y,z.x)) + c.y;
             zx = xTemp;
             z = float2(zx,zy);*/

             else if(_FractalType == 1){
             /*BURNING SHIP*/
             xTemp = z.x*z.x-z.y*z.y+c.x;
             z.y = abs(2*z.x*z.y)*_YMod + c.y ;
             z.x = abs(xTemp)*_XMod;
             }
            
           
              if(length(z) >25){
              break;
              }
              }
             if(iter > _Iter){
              return 0;
              }

              
              float m = pow((iter/_Iter),.3);
              if(_IsGradient < 1){
              col = sin(float4(_RValue,_GValue,_BValue,1)*m*_Repeat)*.5 + .5; //procedural colors
             }
             else{
              col = tex2D(_MainTex, float2(m*_Repeat, _Color));
             }
              return col;
            }//end of [-infity,1] fractals 
            else if(_FractalType == 2){

            float2 v = 0;
            float m = 0;
            float r = 100;

            for(int n = 0; n<_Iter; n++){
            v = float2(v.x*v.x-v.y*v.y*v.y,v.x*v.y*2)+c;

            if(dot(v,v)<(r*r-1)){
            m++;
            }
            v = clamp(v,-r,r);
            
            
            
            }//end of for loop

            if(m == _Iter){
            return float4(0,0,0,1);
            }else{
            return float4(sin(m/4) ,sin(m/5),sin(m/7),1)/4 + .75f;
            }


            return(0,0,0,1);
            }
            
            
            
            
            
            
            
            
            else{
            return float4(0,0,0,1);
                   
            
           
            
           }
           // return iter;
            }
            ENDCG
        }
    }
}
