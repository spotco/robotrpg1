﻿Shader "RGBA_AlphaTest" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
        _Emission ("Emmisive Color", Color) = (0,0,0,0)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Bumpmap", 2D) = "bump" {}
	}
	SubShader {
		Tags {"Queue"="AlphaTest"}
		blend SrcAlpha OneMinusSrcAlpha
	    CGPROGRAM
	    #pragma surface surf Lambert
	   
	    sampler2D _MainTex;
	    sampler2D _BumpMap;
	    fixed4 _Color;
	    fixed4 _Emission;
	   
	    struct Input
	    {
	    	float2 uv_MainTex;
	    	float2 uv_BumpMap;
	    };
	   
	    void surf (Input IN, inout SurfaceOutput o)
	    {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
            o.Emission = _Emission.rgb;
            
	    }
	    ENDCG
	}
	Fallback "VertexLit"
}