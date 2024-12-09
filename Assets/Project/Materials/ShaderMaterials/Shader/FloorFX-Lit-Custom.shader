Shader "Universal Render Pipeline/2D/FloorFX-Sprite-Lit-Custom"
{
    Properties
    {
        /*
        _MainTex("Diffuse", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        
        */
        
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
        
        
        
        _NormalMap("Normal Map", 2D) = "bump" {}

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _Color("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend[_SrcBlend][_DstBlend]
        //Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment

            //#include "UnityCG.cginc"

            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __
            #pragma multi_compile _ DEBUG_DISPLAY

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2  uv          : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                half4   color       : COLOR;
                float2  uv          : TEXCOORD0;
                half2   lightingUV  : TEXCOORD1;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS  : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
                float3 worldPos : TEXCOORD3;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            //Noise Circle Variables
            TEXTURE2D(_CircleTex);
            SAMPLER(sampler_CircleTex);
            //sampler2D _CircleTex;
            float4 _CircleTex_ST;
            float4 _NCColor;
            float4 _NCCenter;
            float  _NCRadius;
            float  _NCBorder;
            float  _NCAlpha;

            //Smoke Granade Variables
            TEXTURE2D(_SmokeTex);
            SAMPLER(sampler_SmokeTex);
            //sampler2D _SmokeTex;
            float4 _SmokeTex_ST;
            TEXTURE2D(_SGMaskTex);
            SAMPLER(sampler_SGMaskTex);
            //sampler2D _SGMaskTex;
            float4 _SGMaskTex_ST;
            float4 _SGColor;
            float4 _SGCenter;
            float  _SGRadius;
            float  _SGAlpha;
            float _SGRotation;
            float _SGSpeedX;
            
            float _Prio;
            

            /*
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            half4 _MainTex_ST;
            */
            float4 _Color;
            half4 _RendererColor;

            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.worldPos = mul(unity_ObjectToWorld, float4(v.positionOS.xyz, 1.0)).xyz; // Use WorldSpace Position
                
                o.positionCS = TransformObjectToHClip(v.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(v.positionOS);
                #endif
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);

                o.color = v.color * _Color * _RendererColor;
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            half4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {

                float4 col;
                float4 nadeCol;
                
                float2 playerPos = _NCCenter.xy ;
                float distToPlayer = distance(playerPos, i.worldPos);
                
                float2 grenadePos = _SGCenter.xy ;
                float distToNade = distance(grenadePos, i.worldPos);
                
                col.a = 0;
                nadeCol.a = 0;

                SurfaceData2D surfaceData;
                InputData2D inputData;

                float UNITY_TWO_PI = 6.28318530718f;

                //Noise Circle Calculation
                if(distToPlayer >= (_NCRadius-_NCBorder/2) && distToPlayer <= (_NCRadius + _NCBorder/2))
                {
                    float2 circleUV;

                    float2 circleCart = i.worldPos - playerPos;
                    circleUV.x = (atan2(circleCart.y,circleCart.x))/UNITY_TWO_PI;                    
                    circleUV.y = (distToPlayer - _NCRadius+(_NCBorder/2))/_NCBorder;                    
                    
                    //Circle painting                    
                    col = SAMPLE_TEXTURE2D(_CircleTex,sampler_CircleTex, circleUV * _CircleTex_ST.xy + _CircleTex_ST.zw) * _NCColor;
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

                    nadeCol = SAMPLE_TEXTURE2D(_SmokeTex, sampler_SmokeTex, nadeBackCart/*i.worldPos * _SmokeTex_ST.xy + _SmokeTex_ST.zw*/) * _SGColor;
                    half4 alphaMask = SAMPLE_TEXTURE2D(_SGMaskTex, sampler_SGMaskTex, nadeCart/2* _SGMaskTex_ST.xy/_SGRadius + 0.5);
                    nadeCol.a *= alphaMask.a;
                    nadeCol.a *= _SGAlpha;

                    InitializeSurfaceData(nadeCol.rgb, nadeCol.a, surfaceData);
                    InitializeInputData(nadeBackCart, i.lightingUV, inputData);

                    nadeCol = CombinedShapeLightShared(surfaceData, inputData);
                    
                }

                //Merge Aproximations
                
                if(nadeCol.a > col.a && col.a <= _Prio)
                {
                    col = nadeCol;
                }
                
                /*
                const half4 main = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                const half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);
                */
                

                

                return col;
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "NormalsRendering"}

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                float4 tangent      : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS      : SV_POSITION;
                half4   color           : COLOR;
                float2  uv              : TEXCOORD0;
                half3   normalWS        : TEXCOORD1;
                half3   tangentWS       : TEXCOORD2;
                half3   bitangentWS     : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            half4 _NormalMap_ST;  // Is this the right way to do this?

            Varyings NormalsRenderingVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = TRANSFORM_TEX(attributes.uv, _NormalMap);
                o.color = attributes.color;
                o.normalWS = -GetViewForwardDir();
                o.tangentWS = TransformObjectToWorldDir(attributes.tangent.xyz);
                o.bitangentWS = cross(o.normalWS, o.tangentWS) * attributes.tangent.w;
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"

            half4 NormalsRenderingFragment(Varyings i) : SV_Target
            {
                const half4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                const half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv));

                return NormalsRenderingShared(mainTex, normalTS, i.tangentWS.xyz, i.bitangentWS.xyz, i.normalWS.xyz);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent"}

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS      : SV_POSITION;
                float4  color           : COLOR;
                float2  uv              : TEXCOORD0;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS  : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float4 _Color;
            half4 _RendererColor;

            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(v.positionOS);
                #endif
                o.uv = TRANSFORM_TEX(attributes.uv, _MainTex);
                o.color = attributes.color * _Color * _RendererColor;
                return o;
            }

            float4 UnlitFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                #if defined(DEBUG_DISPLAY)
                SurfaceData2D surfaceData;
                InputData2D inputData;
                half4 debugColor = 0;

                InitializeSurfaceData(mainTex.rgb, mainTex.a, surfaceData);
                InitializeInputData(i.uv, inputData);
                SETUP_DEBUG_DATA_2D(inputData, i.positionWS);

                if(CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                {
                    return debugColor;
                }
                #endif

                return mainTex;
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
