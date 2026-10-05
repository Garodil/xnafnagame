sampler s0;
texture Mask;
texture Light;
sampler mask_sampler = sampler_state{Texture = Mask;};
sampler light_sampler = sampler_state{Texture = Light;};

float4 CalculateVisibilityFunction(float2 coords: TEXCOORD0) : COLOR0
{
    float4 maskValue = tex2D(mask_sampler, coords);
    float4 light = tex2D(light_sampler, coords);
    float4 worldColor = tex2D(s0, coords); 
    return worldColor * maskValue * light;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 CalculateVisibilityFunction();
    }
}