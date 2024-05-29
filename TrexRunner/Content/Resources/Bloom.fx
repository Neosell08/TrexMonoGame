#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};
float2 WindowSize;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

#define colorRange 1.0

float3 jodieReinhardTonemap(float3 c)
{
    float l = dot(c, float3(0.2126, 0.7152, 0.0722));
    float3 tc = c / (c + 1.0);

    return lerp(c / (l + 1.0), tc, tc);
}

float3 bloomTile(float lod, float2 offset, float2 uv)
{
    return tex2D(SpriteTextureSampler, uv * exp2(-lod) + offset).rgb;
}

float3 getBloom(float2 uv)
{

    float3 blur = float3(0.0, 0.0, 0.0);

    blur = pow(abs(bloomTile(2., float2(0.0, 0.0), uv)), 2.2) + blur;
    blur = pow(abs(bloomTile(3., float2(0.3, 0.0), uv)), 2.2) * 1.3 + blur;
    blur = pow(abs(bloomTile(4., float2(0.0, 0.3), uv)), 2.2) * 1.6 + blur;
    blur = pow(abs(bloomTile(5., float2(0.1, 0.3), uv)), 2.2) * 1.9 + blur;
    blur = pow(abs(bloomTile(6., float2(0.2, 0.3), uv)), 2.2) * 2.2 + blur;

    return blur * colorRange;
}

float4 mainImage(VertexShaderOutput input) : COLOR
{
    float2 uv = input.Position.xy / WindowSize.xy;
    
    float4 color = pow(tex2D(SpriteTextureSampler, input.TextureCoordinates.xy).rgba * colorRange, 2.2);
    color = pow(abs(color), 2.2);
    color += float4(pow(getBloom(input.TextureCoordinates.xy), 2.2), 0);
    color = pow(abs(color), 1.0 / 2.2);
    
    color = float4(jodieReinhardTonemap(color.rgb), 0);
    
    return color;
}

technique SpriteDrawing
{
	pass P0
	{
        PixelShader = compile PS_SHADERMODEL mainImage();
    }
};