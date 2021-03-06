////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/TV_Horror" { 
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
}
SubShader
{
Pass
{
ZTest Always
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma target 3.0
#pragma glsl
#include "UnityCG.cginc"
uniform sampler2D _MainTex;
uniform sampler2D Texture2;
uniform float _TimeX;
uniform float _Value;
uniform float _Value2;
uniform float _Value3;
uniform float4 _ScreenResolution;
struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;

};
struct v2f
{
half2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
fixed4 color    : COLOR;
};
v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
float2 uv2 = i.texcoord.xy;
_TimeX=floor(_TimeX*16)/16;
_TimeX=_TimeX*sin(_TimeX*8)*8;
uv.x += uv.y*sin(_TimeX * 4.0)/128;
uv.y += uv.y*cos(_TimeX * 4.0)/256;
float4 txt2=tex2D(Texture2, uv);
uv2=lerp(i.texcoord.xy,uv,_Value/4);
float4 txt=tex2D(_MainTex, uv2);
float bw=(txt.r+txt.g+txt.b)/3;
txt=lerp(txt,bw,_Value);
txt+=lerp(txt,txt2,_Value/4);
txt*=lerp(txt,txt*txt2,_Value);
return  txt;
}
ENDCG
}
}
}
