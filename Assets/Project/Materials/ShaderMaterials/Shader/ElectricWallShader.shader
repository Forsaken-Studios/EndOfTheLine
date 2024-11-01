Shader "Unlit/ElectricWallShader"
{
    Properties
    {
        [Header(Electricity Wall)]
        [Space(10)]
        _ElecTex ("Electricity Texture", 2D) = "white" {}
        _ElecColor ("Electricity Color", Color) = (1, 1, 1, 1)
        _SpawnPoints ("Spawn Points", Vector) = (0, 0, 0, 0)
        _ElecSpeed ("Electricity Speed", Float) = 0
    }
    SubShader
    {
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

            sampler2D _ElecTex;
            float4 _ElecTex_ST;
            float4 _ElecColor;
            float4 _SpawnPoints;
            float _ElecSpeed;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float2 startPoint = float2(_SpawnPoints.x, _SpawnPoints.y);
                float2 endPoint   = float2(_SpawnPoints.z, _SpawnPoints.w);

                float wallLength = distance(startPoint,endPoint);
                float2 lengthOffset = _ElecTex_ST.xy;
                lengthOffset.y *= wallLength; //Parametrize Texture indepentend to length-scalling
                o.uv = v.uv* lengthOffset +_ElecTex_ST.zw;//TRANSFORM_TEX(v.uv, _ElecTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv.y += _Time.y * _ElecSpeed;
                
                fixed4 col = tex2D(_ElecTex, uv) *_ElecColor;
                return col;
            }
            ENDCG
        }
    }
}
