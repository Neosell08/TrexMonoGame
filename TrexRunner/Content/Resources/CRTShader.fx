//Isak's CRTShader. Not made by me


#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float Time;
float2 ScreenSize = float2(1920, 1080);
float PixelSize = 8;
float ChromaticStrength = 0.02f;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};
const float PincushionAmount = 0.02;

float4 Pixelate(VertexShaderOutput input) : COLOR
{
	
    float2 onePixel = float2(8 / 1920, 8 / 1080);// / 2;
	// 256
    //float2 ratio = float2(1920 / 4, 1080 / 4); //  (ScreenSize / 8);
    float2 ratio = float2(480, 270); //  (ScreenSize / 8);
	
    return tex2D(SpriteTextureSampler, floor(input.TextureCoordinates * ratio) / ratio + onePixel + onePixel);
    return float4((floor(input.TextureCoordinates * ratio) / 256).xy, 0, 1);
    return tex2D(SpriteTextureSampler, input.TextureCoordinates);
    //return tex2D(SpriteTextureSampler, floor(input.TextureCoordinates * ScreenSize) / ScreenSize);
}

float4 Blur(VertexShaderOutput input) : COLOR
{
	return (
	tex2D(SpriteTextureSampler, input.TextureCoordinates) + 
	tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.0015,0)) + 
	tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-0.0015,0)) +
	tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, 0.0015)) + 
	tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, -0.0015))
	) 
	/ 5;
}

float4 Posterize(VertexShaderOutput input) : COLOR
{
	return floor(tex2D(SpriteTextureSampler, input.TextureCoordinates) * 8.0) / 8.0;
}


float4 ChromaticShader(VertexShaderOutput input) : COLOR
{
	float dist = 0.1 + pow(length(input.TextureCoordinates - 0.5), 3);

	

	float2 redOffset = float2(1, 0) * ChromaticStrength;
    float2 greenOffset = float2(0, 0) * ChromaticStrength;
    float2 blueOffset = float2(-1, 0) * ChromaticStrength;

	float4 color = 1;

	color.r = (tex2D(SpriteTextureSampler, input.TextureCoordinates + redOffset * dist) * input.Color).r;
	color.g = (tex2D(SpriteTextureSampler, input.TextureCoordinates + blueOffset * dist) * input.Color).g;
	color.b = (tex2D(SpriteTextureSampler, input.TextureCoordinates + greenOffset * dist) * input.Color).b;

	return color;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	
	float dist = 0.1 + pow(length(input.TextureCoordinates - 0.5), 2);
	float2 ScanCoord = (input.TextureCoordinates - 0.5) * 1 + 0.5;

	if (ScanCoord.x < 0) return float4(0, 0, 0, 1);
	if (ScanCoord.x > 1) return float4(0, 0, 0, 1);
	if (ScanCoord.y < 0) return float4(0, 0, 0, 1);
	if (ScanCoord.y > 1) return float4(0, 0, 0, 1);

	float scanLine = sin(radians((Time*50 - ScanCoord.y * 50) * 500));

	if (scanLine > 0) scanLine = pow(scanLine, 5);

	float4 baseColor = tex2D(SpriteTextureSampler, ScanCoord) * input.Color;

    if (scanLine < 0.5)
        scanLine = 0;
	
	return float4(lerp(baseColor.rgb, 1, scanLine/12), 1);
}

float4 VingetteShader(VertexShaderOutput input) : COLOR
{
	float dist = pow(length(input.TextureCoordinates - 0.5), 1.5);
	return tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color * (0.9 - dist);
}

//technique Pixelate2
//{
//    pass P0
//    {
//        PixelShader = compile PS_SHADERMODEL Pixelate();

//    }
//};

//technique Posterize2
//{
//    pass P0
//    {
//        PixelShader = compile PS_SHADERMODEL Posterize();
//    }
//};

//technique Chromatic
//{
//	pass P0
//	{
//		PixelShader = compile PS_SHADERMODEL ChromaticShader();
//	}
//};

//technique Blur1
//{
//	pass P0
//	{
//		PixelShader = compile PS_SHADERMODEL Blur();
//	}
//};
//technique Blur2
//{
//    pass P0
//    {
//        PixelShader = compile PS_SHADERMODEL Blur();
//    }
//};
technique Scanlines
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL Blur();
    }
    pass P1
    {
        PixelShader = compile PS_SHADERMODEL MainPS();

    }
    pass P2
    {
        PixelShader = compile PS_SHADERMODEL ChromaticShader();
    }
};

technique Vingette
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL VingetteShader();
	}
};