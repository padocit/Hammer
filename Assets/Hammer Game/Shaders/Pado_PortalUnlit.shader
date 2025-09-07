Shader "Unlit/Pado_PortalUnlit"
{
    Properties
    {
        _InnerColor("Inner Color", Color) = (0.85, 0.95, 1, 1)
        _RimColor("Rim Color", Color) = (0.45, 0.2, 1, 1)
        _Opacity("Opacity", Range(0,1)) = 0.85

        _OuterRadius("Outer Radius", Range(0.1,0.7)) = 0.45
        _EdgeWidth("Edge Width", Range(0.01,0.5)) = 0.15
        _CenterFade("Center Fade", Range(0.0,0.5)) = 0.15

        _SwirlStrength("Swirl Strength", Range(0,6)) = 2.0
        _ScrollSpeed("Scroll Speed", Range(-6,6)) = 1.5
        _Ripple("Radial Ripple", Range(0,10)) = 1.0
        _RippleFreq("Ripple Freq", Range(0,20)) = 6.0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalRenderPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Pass
        {
            Name "Unlit"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_Position;
                float2 uv          : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _InnerColor;
                float4 _RimColor;
                float  _Opacity;

                float  _OuterRadius;
                float  _EdgeWidth;
                float  _CenterFade;

                float  _SwirlStrength;
                float  _ScrollSpeed;
                float  _Ripple;
                float  _RippleFreq;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float2 rotate(float2 p, float a)
            {
                float s = sin(a), c = cos(a);
                return float2(c*p.x - s*p.y, s*p.x + c*p.y);
            }

            half4 frag (Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                // UV를 중심(0,0) 기준의 로컬 좌표로 변환
                float2 uv = IN.uv - 0.5;

                // 극좌표
                float r = length(uv);
                float ang = atan2(uv.y, uv.x);

                // 시간
                float t = _Time.y * _ScrollSpeed;

                // 스월(소용돌이) + 라디얼 리플
                float swirl = _SwirlStrength * (0.25 + 0.75*smoothstep(0.0, _OuterRadius, r));
                ang += swirl * sin(t + r * 6.28318);
                r += (_Ripple > 0 ? (sin(r * _RippleFreq * 6.28318 - t*2.0) * 0.01 * _Ripple) : 0);

                // 다시 데카르트로 살짝 왜곡 적용 (시각적 흐름)
                float2 warped = float2(cos(ang), sin(ang)) * r;

                // 링 마스크 (가장자리 부드럽게)
                float outer = smoothstep(_OuterRadius, _OuterRadius - _EdgeWidth, r);
                // 중심부 투명 페이드
                float center = smoothstep(_CenterFade, 0.0, r);
                float mask = saturate(outer * center);

                // 반짝이는 그라데이션 (r 기반)
                float grad = saturate((r) / (_OuterRadius + 1e-5));
                // 살짝 살아 움직이는 느낌
                grad += 0.08 * sin(ang*4.0 + t*2.2);
                grad = saturate(grad);

                float3 col = lerp(_InnerColor.rgb, _RimColor.rgb, grad);

                // 알파
                float alpha = mask * _Opacity;

                return half4(col, alpha);
            }
            ENDHLSL
        }
    }
    FallBack Off
}