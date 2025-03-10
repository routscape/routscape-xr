Shader "Custom/Stencil_Mask"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _StencilMask ("Stencil Mask", Range(0, 255)) = 1
        _XMin ("Local X Min", Float) = -0.5
        _XMax ("Local X Max", Float) = 0.5
        _ZMin ("Local Z Min", Float) = -0.5
        _ZMax ("Local Z Max", Float) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Geometry-1"
        }
        LOD 200

        Stencil
        {
            Ref 1
            Comp Never
            Fail Replace
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _XMin, _XMax, _ZMin, _ZMax;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 localPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float4 localPos4 = mul(unity_WorldToObject, worldPos);
                o.localPos = localPos4.xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (i.localPos.x < _XMin || i.localPos.x > _XMax ||
                    i.localPos.z < _ZMin || i.localPos.z > _ZMax)
                {
                    clip(-1);
                }
                return _Color;
            }
            ENDCG
        }
    }
}