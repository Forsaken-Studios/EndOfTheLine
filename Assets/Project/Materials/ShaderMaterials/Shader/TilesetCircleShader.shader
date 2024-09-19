Shader "Unlit/TilesetCircleShader"
{
    Properties
    {
        _CircleTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Area Color", Color) = (1, 1, 1, 1)
        _Center ("Center", Vector) = (0, 0, 0, 0)
        _Radius ("Radius",Range(0, 20)) = 75
        _Border ("Border",Range(0, 20)) = 12
        _Alpha ("Alpha",Range(0,1)) = 1
        
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        //Blend[_SrcBlend][_DstBlend]
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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _CircleTex;
            float4 _CircleTex_ST;
            float4 _Color;

            float4 _Center;
            float _Radius;
            float _Border;
            float _Alpha;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz; // Use WorldSpace Position 
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 centering = _Center.xy ;
                
                fixed4 col;

                float dist = distance(centering, i.worldPos);
                /*
                if(dist > _Radius && dist < (_Radius + _Border))
                {
                    float2 gradPos;
                    gradPos.x = 0.5f;
                    gradPos.y = (dist - _Radius)/_Border;
                */
                if(dist > (_Radius-_Border/2) && dist < (_Radius + _Border/2))
                {
                    float2 gradPos;

                    //gradPos.x = 0.5f;

                    float2 cartesian = i.worldPos - centering;
                    gradPos.x = (atan2(cartesian.y,cartesian.x))/UNITY_TWO_PI;
                    
                    gradPos.y = (dist - _Radius+(_Border/2))/_Border;

                    
                    
                    //Circle painting
                    
                    col = tex2D(_CircleTex, gradPos * _CircleTex_ST.xy + _CircleTex_ST.zw) * _Color;
                    col.a *= _Alpha;

                }else
                {
                    col.a = 0;
                }


                
                
                
                return col;
            }
            ENDCG
        }
    }
}
