Shader "Custom/TransparentTextureArray" {
Properties {
    _MainTex ("Base (RGB)", 2DArray) = "white" {}
}

SubShader {
    Tags { "RenderType" = "Opaque" }
    LOD 100

    Pass {
        CGPROGRAM

        #pragma vertex vert
        #pragma fragment frag
        #pragma target 3.5
        #pragma multi_compile_fog
        
        #pragma require 2darray
         
        UNITY_DECLARE_TEX2DARRAY(_MainTex);

        #include "UnityCG.cginc"

        struct appdata_t {
            float4 vertex : POSITION;
            float3 texcoord : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f {
            float4 vertex : SV_POSITION;
            float3 texcoord : TEXCOORD0;
            UNITY_FOG_COORDS(1)
            UNITY_VERTEX_OUTPUT_STEREO
        };

        float4 _MainTex_ST;

        v2f vert (appdata_t v)
        {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.texcoord.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
            o.texcoord.z = v.texcoord.z;
            UNITY_TRANSFER_FOG(o,o.vertex);
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            fixed4 col = UNITY_SAMPLE_TEX2DARRAY(_MainTex, i.texcoord);
            if (col.a < 0.1) discard;
            UNITY_APPLY_FOG(i.fogCoord, col);
            UNITY_OPAQUE_ALPHA(col.a);
            return col;
        }
        
        ENDCG
    }
}

}