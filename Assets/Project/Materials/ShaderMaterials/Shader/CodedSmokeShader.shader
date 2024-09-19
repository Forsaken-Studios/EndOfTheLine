Shader "Unlit/CodedSmokeShader"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        _SmokeTex ("Smoke", 2D) = "white" {}
        _Color ("Color (RGBA)", Color) = (1, 1, 1, 1)
        _SpeedX ("Speed X", float) = 0
        _SpeedY ("Speed Y", float) = 0
        [Toggle] _Enable ("IsMoving", Float) = 0
        
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend mode", float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend mode", float) = 0
          
        
        
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend[_SrcBlend][_DstBlend]
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
                //float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                //float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            //sampler2D _MainTex;
            sampler2D _SmokeTex;
            //float4 _MainTex_ST;
            float4 _SmokeTex_ST;
            float4 _Color;

            float _SpeedX;
            float _SpeedY;
            float _Enable;

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz; // Use WorldSpace Position 
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.worldPos.x += _Time.y *_SpeedX * _Enable;
                o.worldPos.y += _Time.y *_SpeedY * _Enable;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                fixed4 col = tex2D(_SmokeTex, i.worldPos * _SmokeTex_ST.xy + _SmokeTex_ST.zw) * _Color;
                
                return col;
            }
            ENDCG
        }
    }
}
