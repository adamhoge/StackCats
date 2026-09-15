Shader "TSF/BaseOutline1"
{
    Properties
    {
        [Toggle(_TEX_ON)] _DetailTex("Enable Detail texture", Float) = 0
        [MainTexture] _MainTex("Detail", 2D) = "white" {}
        _ToonShade("Shade", 2D) = "white" {}
        [Toggle(_COLOR_ON)] _TintColor("Enable Color Tint", Float) = 0
        [MainColor] _Color("Base Color", Color) = (1, 1, 1, 1)
        [Toggle(_VCOLOR_ON)] _VertexColor("Enable Vertex Color", Float) = 0
        _Brightness("Brightness 1 = neutral", Float) = 1.0
        _OutlineColor("Outline Color", Color) = (0.5, 0.5, 0.5, 1.0)
        _Outline("Outline width", Float) = 0.01
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        // Base Pass
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _TEX_ON
            #pragma shader_feature _COLOR_ON
            #pragma shader_feature _VCOLOR_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvn : TEXCOORD1;
                float4 vertexColor : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_ToonShade);
            SAMPLER(sampler_ToonShade);

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float4 _MainTex_ST;
                float4 _ToonShade_ST;
                float _Brightness;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

                // Transform normal to view space and encode as UV coordinates
                float3 normalVS = mul((float3x3)UNITY_MATRIX_IT_MV, normalize(IN.normalOS));
                normalize(normalVS);
                normalVS = normalVS * 0.5 + 0.5;
                OUT.uvn = normalVS.xy;

                #ifdef _TEX_ON
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                #else
                OUT.uv = float2(0, 0);
                #endif

                OUT.vertexColor = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 toonShade = SAMPLE_TEXTURE2D(_ToonShade, sampler_ToonShade, IN.uvn);

                #ifdef _COLOR_ON
                toonShade *= _Color;
                #endif

                #ifdef _TEX_ON
                half4 detail = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half4 result = toonShade * detail * _Brightness;
                #else
                half4 result = toonShade * _Brightness;
                #endif

                #ifdef _VCOLOR_ON
                result *= IN.vertexColor;
                #endif

                return result;
            }
            ENDHLSL
        }

        // Outline Pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            
            Cull Front
            ZWrite On

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                float _Outline;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionOS = IN.positionOS.xyz;
                positionOS.xyz += normalize(IN.normalOS.xyz) * _Outline * 0.01;
                OUT.positionHCS = TransformObjectToHClip(positionOS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}