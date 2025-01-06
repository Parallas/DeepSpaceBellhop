#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float GameTime;
float WobbleInfluence;
float HueShiftInfluence;
float FlippyInfluence;

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

#include "voronoi.hlsl"

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float3 FCCYIQFromSRGB(float3 srgb)
{
    float3 yiq = float3(
        srgb.r * 0.30 + srgb.g * 0.59 + srgb.b * 0.11,
        srgb.r * 0.599 + srgb.g * -0.2773 + srgb.b * -0.3217,
        srgb.r * 0.213 + srgb.g * -0.5251 + srgb.b * 0.3121
    );

    return yiq;
}

float3 SRGBFromFCCYIQ(float3 yiq)
{
    float3 srgb = float3(
        yiq.x + yiq.y * 0.9469 + yiq.z * 0.6236,
        yiq.x + yiq.y * -0.2748 + yiq.z * -0.6357,
        yiq.x + yiq.y * -1.1 + yiq.z * 1.7
    );

    return srgb;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 inputUv = input.TextureCoordinates;
    float2 uv = lerp(inputUv, float2(inputUv.x, 1 - inputUv.y), FlippyInfluence);
	
    float voronoi1 = voronoi3D_float(float3(uv, GameTime / 10), 5).x * 0.02;

    float2 final_uv = lerp(uv, uv + voronoi1, WobbleInfluence);
    float4 col = tex2D(SpriteTextureSampler, final_uv).rgba * input.Color.rgba;

    float3 yiq = FCCYIQFromSRGB(col.rgb);
    float2x2 rot_matrix = float2x2(
        cos(GameTime), -sin(GameTime),
        sin(GameTime), cos(GameTime)
    );
    yiq.yz = mul(rot_matrix, yiq.yz);
    float3 rgb = SRGBFromFCCYIQ(yiq);
    float3 rgb_lerped = lerp(col.rgb, rgb, HueShiftInfluence);

    return float4(rgb_lerped, col.a);
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
