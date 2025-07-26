Shader "Custom/Outline" {
  Properties {
    _Color      ("Color",        Color)           = (1.0, 1.0, 1.0, 0.9)
    _Glossiness ("Smoothness",   Range(0.0, 1.0)) = 0.0
    _MainTex    ("Albedo (RGB)", 2D)              = "white" {}
    _Metallic   ("Metallic",     Range(0.0, 1.0)) = 0.0
    _Thickness  ("Width",        Float)           = 2
  }

  SubShader {
    Tags {
      "Queue"      = "Transparent"
      "RenderType" = "Opaque"
    }

    Blend  SrcAlpha OneMinusSrcAlpha
    Cull   Front
    LOD    200
    ZWrite Off

    Pass {
      CGPROGRAM
        #pragma fragment frag
        #pragma vertex   vert
        // #pragma target   3.0
        // #pragma surface surf Standard fullforwardshadows

        #include "UnityCG.cginc"

        /* … */
        struct input { float4 position : POSITION;    float2 uv : TEXCOORD0; float3 normal : NORMAL; };
        struct v2f   { float4 position : SV_POSITION; float2 uv : TEXCOORD0; };

        /* … */
        fixed4    _Color;
        sampler2D _MainTex;
        float4    _MainTex_ST;

        /* … */
        float4 frag(v2f transformation) : SV_Target {
          fixed4 tex = tex2D(_MainTex, transformation.uv) * _Color;
          return tex;
        }

        v2f vert(input vertex) {
          v2f transformation;

          // …
          transformation.position = UnityObjectToClipPos(vertex.position + (normalize(vertex.normal) * 0.03));
          transformation.uv       = TRANSFORM_TEX(vertex.uv, _MainTex);

          return transformation;
        }
      ENDCG
    }
  }
}
