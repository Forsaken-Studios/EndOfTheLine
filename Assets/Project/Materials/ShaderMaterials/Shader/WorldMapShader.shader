Shader "Unlit/WorldMapShader"
{
    Properties
    {
        [Header(Mapping)]
        [Space(10)]
        _MapTex ("Base Map", 2D) = "white" {}
        _MapMaskTex ("Rail Mask", 2D) = "white" {}
  
        [Space(10)]
        [Header(Indicator)]
        [Space(10)]
        _IndicatorTex ("Indicator Texture", 2D) = "white" {}
        _IndicatorColor ("Indicator Color", Color) = (1, 1, 1, 1)
        _IndicatorCenter ("Indicator Center", Vector) = (0, 0, 0, 0)
        _IndicatorRadius ("Indicator Radius (Pixels)",Range(0, 5)) = 2
        
        
    }
    SubShader
    {
        //Tags { "RenderType"="Opaque" }
        
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        LOD 100

        Pass
        {
            CGPROGRAM
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

            sampler2D _MapTex;
            float4 _MapTex_ST;
            sampler2D _MapMaskTex;
            float4 _MapMaskTex_ST;
            sampler2D _IndicatorTex;
            float4 _IndicatorTex_ST;
            float4 _IndicatorColor;
            float4 _IndicatorCenter;
            float  _IndicatorRadius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MapTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MapTex, i.uv);
                fixed4 mask = tex2D(_MapMaskTex, i.uv);
                float2 IndicatorPos = _IndicatorCenter.xy/128 ;
                float distToCenter = distance(IndicatorPos, i.uv);
                float radiusToPixel = _IndicatorRadius/128;


                if(mask.a>0){
/*

                    // Draws Circle
                    if(distToCenter <= (radiusToPixel)){
                        
                        float2 indicatorUV = IndicatorPos-i.uv;
                        fixed4 alphaMask = tex2D(_IndicatorTex, indicatorUV/2* _IndicatorTex_ST.xy+ 0.5);
                        if(alphaMask.a > 0)
                        {
                            col = _IndicatorColor;
                        }
                        
                        
                    }*/

                    // Draws Square
                    float distToCenterX = (IndicatorPos.x - i.uv.x)/(radiusToPixel*2);
                    float distToCenterY = (IndicatorPos.y - i.uv.y)/(radiusToPixel*2);
                    float distAbsToCenterX = abs(distToCenterX)+0.5;
                    float distAbsToCenterY = abs(distToCenterY)+0.5;
                    if((distAbsToCenterX <= 1)&&(distAbsToCenterY<= 1))
                    {
                        float2 indicatorUV;
                        indicatorUV.x = distToCenterX;
                        indicatorUV.y = distToCenterY;
                        
                        fixed4 alphaMask = tex2D(_IndicatorTex, indicatorUV+0.5);
                        if(alphaMask.a>0)
                        {
                            col = _IndicatorColor;
                        }
                        
                        
                    }
                    
                }
                
                
                return col;
            }
            ENDCG
        }
    }
}
