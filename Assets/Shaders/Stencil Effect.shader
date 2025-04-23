Shader "Custom/Stencil_Effect"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _XMin ("Local X Min", Float) = -0.5
        _XMax ("Local X Max", Float) = 0.5
        _ZMin ("Local Z Min", Float) = -0.5
        _ZMax ("Local Z Max", Float) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }

        Stencil
        {
            Ref 1
            Comp equal
            Pass Keep
        }

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        sampler2D _MainTex;
        float _XMin, _XMax, _ZMin, _ZMax;

        struct Input
        {
            float2 uv_MainTex;
            float3 localPos;
        };

        // Transform the vertex position into object space
        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
            float4 localPos4 = mul(unity_WorldToObject, worldPos);
            o.localPos = localPos4.xyz;
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Clip fragments outside the local boundaries (relative to object center)
            if (IN.localPos.x < _XMin || IN.localPos.x > _XMax ||
                IN.localPos.z < _ZMin || IN.localPos.z > _ZMax)
            {
                clip(-1);
            }
            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}