Shader "Custom/36_Hologram"
{
    Properties
    {
        _RimColor("Rim Color",Color) = (0,0.5,0.5,0.0)
        _RimPower("Rim Power",Range(0.5,8.0)) = 3.0
         _MainTex("AAlbedo (RGB)", 2D) = "white" {}
        _Scale("Scale", Float) = 1.0
          _Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Pass{
            Zwrite On
            ColorMask 0
        }
        CGPROGRAM
        #pragma surface surf Lambert alpha:fade
       float4 _RimColor;
       float _RimPower;
        sampler2D _MainTex;
        float _Scale;
               fixed4 _Color;
        struct Input
        {
            float3 viewDir;
            float3 worldNormal;
            float3 worldPos;
            float2 uv_MainTex;
        };
        void surf (Input IN, inout SurfaceOutput o)
        {
            float2 UV;
            fixed4 c;

            if (abs(IN.worldNormal.x) > 0.5) {
                UV = IN.worldPos.yz; 
                c = tex2D(_MainTex, UV  * _Scale);
            }
            else if (abs(IN.worldNormal.z) > 0.5) {
                UV = IN.worldPos.xy;
                c = tex2D(_MainTex, UV * _Scale); 
            }
            else {
                UV = IN.worldPos.xz;
                c = tex2D(_MainTex, UV * _Scale); 
            }
            o.Albedo = c.rgb * _Color;
            half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            o.Emission = _RimColor.rgb * pow(rim, _RimPower) * 10;
            o.Alpha = pow(rim, _RimPower);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
