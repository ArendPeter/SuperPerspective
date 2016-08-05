// simple "dissolving" shader by genericuser (radware.wordpress.com)
// clips materials, using an image as guidance.
// use clouds or random noise as the slice guide for best results.
Shader "Custom Shaders/DissolveShader" {
	Properties{
		_MainTex("Texture (RGB)", 2D) = "white" {}
	_SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
	_SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0.5
	
	_DetailMap("Detail Map", 2D) = "gray" {}
	_DetailRange("Detail Range", float) = 2

	_EmissionAmount("Emission Amount", Range(0.0, 1.0)) = 0.5
	[HDR] _EmissionColor("Emission Color", Color) = (0,0,0)

	_FollowsScreen("Should Projection Map", float) = 1
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		Cull Off
		CGPROGRAM
		//if you're not planning on using shadows, remove "addshadow" for better performance
#pragma surface surf Lambert addshadow
		struct Input {
		float2 uv_MainTex;
		float2 uv_SliceGuide;
		float _SliceAmount;
		float2 uv_DetailMap;
		float4 screenPos;
	};
	sampler2D _MainTex;
	sampler2D _SliceGuide;
	float _SliceAmount;
	float _DetailRange;
	float _EmissionAmount;
	sampler2D _DetailMap;
	float4 _EmissionColor;
	float _FollowsScreen;


	void surf(Input IN, inout SurfaceOutput o) {
		clip(tex2D(_SliceGuide, IN.uv_SliceGuide).rgb - _SliceAmount);
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;

		

		//if(_FollowsScreen > 0) {o.Albedo *= tex2D(_DetailMap, (IN.screenPos.xy / IN.screenPos.w)).rgb;}		
		if(_FollowsScreen <= 0) {o.Albedo *= tex2D(_DetailMap, IN.uv_DetailMap).rgb *_DetailRange;}
		
		o.Emission = _EmissionColor.rgba * _EmissionAmount;


	}
	ENDCG
	}
		Fallback "Diffuse"
}