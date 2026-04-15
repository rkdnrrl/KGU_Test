Shader "Custom/URP_Lit_Outline_WebGLSafe"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)

        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0,0.05)) = 0.01
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" }

        // =========================
        // ✅ WebGL Safe Lit
        // =========================
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseColor;

            Varyings vert (Attributes v)
            {
                Varyings o;

                VertexPositionInputs pos = GetVertexPositionInputs(v.positionOS.xyz);
                VertexNormalInputs normal = GetVertexNormalInputs(v.normalOS);

                o.positionHCS = pos.positionCS;
                o.positionWS = pos.positionWS;
                o.normalWS = normal.normalWS;
                o.uv = v.uv;

                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half3 normalWS = normalize(i.normalWS);

                // 👉 메인 라이트 (URP 공식)
                Light mainLight = GetMainLight();

                half NdotL = saturate(dot(normalWS, mainLight.direction));

                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                half3 albedo = tex.rgb * _BaseColor.rgb;

                // 👉 Ambient (SH 대신 안정 방식)
                half3 ambient = 0.3 * albedo;

                // 👉 Diffuse
                half3 diffuse = albedo * mainLight.color * NdotL;

                return half4(ambient + diffuse, _BaseColor.a);
            }

            ENDHLSL
        }

        // =========================
        // ✅ Outline
        // =========================
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="SRPDefaultUnlit" }

            Cull Front
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM

            #pragma vertex vert_outline
            #pragma fragment frag_outline

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float _OutlineThickness;
            float4 _OutlineColor;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert_outline (Attributes v)
            {
                Varyings o;

                float3 posWS = TransformObjectToWorld(v.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(v.normalOS);

                posWS += normalWS * _OutlineThickness;

                o.positionHCS = TransformWorldToHClip(posWS);

                return o;
            }

            half4 frag_outline (Varyings i) : SV_Target
            {
                return _OutlineColor;
            }

            ENDHLSL
        }
    }
}