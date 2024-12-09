Shader "Unlit/FloorFXShader"
{
    Properties
    {
        [Header(Noise Circle)]
        [Space(10)]
        _CircleTex ("Circle Texture", 2D) = "white" {}
        _NCColor ("Base Color", Color) = (1, 1, 1, 1)
        _NCCenter ("Player Center", Vector) = (0, 0, 0, 0)
        _NCRadius ("Noise Radius",Range(0, 20)) = 2
        _NCBorder ("Radius Width",Range(0, 20)) = 0.4
        _NCAlpha ("Circle Alpha",Range(0,1)) = 1
        [Space(10)]
        
        [Header(Smoke Granade)]
        [Space(10)]
        _SmokeTex ("Smoke Texture", 2D) = "white" {}
        _SGMaskTex ("Grenade Alpha Mask Texture", 2D) = "white" {}
        _SGColor ("Smoke Color", Color) = (1, 1, 1, 1)
        _SGCenter ("Smoke Origin", Vector) = (0, 0, 0, 0)
        _SGRadius ("Smoke Radius",Range(0, 20)) = 2
        _SGAlpha ("Smoke Alpha",Range(0,1)) = 0
        [Toggle] _SGRotation ("Smoke Rotating", Float) = 0
        _SGSpeedX ("Rotation Speed", float) = 0
        
        
        [Space(10)]
        
        [Header(Merge Settings)]
        [Space(10)]
        _Prio ("Merge Priority",Range(0,1)) = 0
        
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend mode", float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend mode", float) = 0
        
        
        
        
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        ZWrite Off
        //Blend SrcAlpha OneMinusSrcAlpha
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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            //Noise Circle Variables
            sampler2D _CircleTex;
            float4 _CircleTex_ST;
            float4 _NCColor;
            float4 _NCCenter;
            float  _NCRadius;
            float  _NCBorder;
            float  _NCAlpha;

            //Smoke Granade Variables
            sampler2D _SmokeTex;
            float4 _SmokeTex_ST;
            sampler2D _SGMaskTex;
            float4 _SGMaskTex_ST;
            float4 _SGColor;
            float4 _SGCenter;
            float  _SGRadius;
            float  _SGAlpha;
            float _SGRotation;
            float _SGSpeedX;
            
            float _Prio;

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
                fixed4 col;
                fixed4 nadeCol;
                
                float2 playerPos = _NCCenter.xy ;
                float distToPlayer = distance(playerPos, i.worldPos);
                
                float2 grenadePos = _SGCenter.xy ;
                float distToNade = distance(grenadePos, i.worldPos);
                                
                /*
                if(dist > _Radius && dist < (_Radius + _Border))
                {
                    float2 gradPos;
                    gradPos.x = 0.5f;
                    gradPos.y = (dist - _Radius)/_Border;
                */
                
                col.a = 0;
                nadeCol.a = 0;


                //Noise Circle Calculation
                if(distToPlayer >= (_NCRadius-_NCBorder/2) && distToPlayer <= (_NCRadius + _NCBorder/2))
                {
                    float2 circleUV;

                    float2 circleCart = i.worldPos - playerPos;
                    circleUV.x = (atan2(circleCart.y,circleCart.x))/UNITY_TWO_PI;                    
                    circleUV.y = (distToPlayer - _NCRadius+(_NCBorder/2))/_NCBorder;                    
                    
                    //Circle painting                    
                    col = tex2D(_CircleTex, circleUV * _CircleTex_ST.xy + _CircleTex_ST.zw) * _NCColor;
                    col.a *= _NCAlpha;
                }

                //Grenade Smoke Calculation
                
                if(distToNade <= _SGRadius)
                {
                    float2 nadeUV;

                    float2 nadeCart = i.worldPos - grenadePos;
                    nadeUV -= 0.5;
                    nadeUV.x = (atan2(nadeCart.y,nadeCart.x))/UNITY_TWO_PI;
                    nadeUV.y = length(nadeCart);

                    nadeUV = nadeUV * _SmokeTex_ST.xy + _SmokeTex_ST.zw;
                    nadeUV.x += _Time.y * _SGSpeedX * _SGRotation;

                    float2 nadeBackCart;
                    sincos(nadeUV.x * UNITY_TWO_PI, nadeBackCart.y, nadeBackCart.x);
                    nadeBackCart *=nadeUV.y;
                    nadeBackCart += 0.5;
                    
                    
                    //Smoke Grenade Painting

                    nadeCol = tex2D(_SmokeTex, nadeBackCart/*i.worldPos * _SmokeTex_ST.xy + _SmokeTex_ST.zw*/) * _SGColor;
                    fixed4 alphaMask = tex2D(_SGMaskTex, nadeCart/2* _SGMaskTex_ST.xy/_SGRadius + 0.5);
                    nadeCol.a *= alphaMask.a;
                    nadeCol.a *= _SGAlpha;
                    
                }

                //Merge Aproximations
                
                if(nadeCol.a > col.a && col.a <= _Prio)
                {
                    col = nadeCol;
                }
                
                
                
                return col;
            }
            ENDCG
        }
    }
}
