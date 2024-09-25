#define PI 3.14159265359

sampler2D _MainTex;
float4 _MainTex_TexelSize;

float _FrameCount;
float2 _OutputRes;
float _FrameTime;

#define SCANLINES (_OutputRes.y)
#define FPS (30)
#define FRAME_COUNT (floor(_FrameTime * FPS))


float3 RGBtoYUV(float3 rgb)
{
    // https://en.wikipedia.org/wiki/YUV#Conversion_to/from_RGB
    const float3x3 matSDTV = float3x3(
        0.299, 0.587, 0.114,
        -0.14713, -0.28886, 0.436,
        0.615, -0.51499, -0.10001
    );

    const float3x3 matHDTV = float3x3(
        0.2126, 0.7152, 0.0722,
        -0.09991, -0.33609, 0.436,
        0.615, -0.55861, -0.05639
    );

    return mul(matSDTV, rgb);
}

float3 YUVtoRGB(float3 yuv)
{
    const float3x3 matSDTV = float3x3(
        1, 0, 1.13983,
        1, -0.39465, -0.58060,
        1, 2.03211, 0
    );

    const float3x3 matHDTV = float3x3(
        1, 0, 1.28033,
        1, -0.21482, -0.38059,
        1, 2.12798, 0
    );

    return mul(matSDTV, yuv);
}

float3 RGBtoYIQ(float3 rgb)
{
    const float3x3 mat = float3x3(
        0.299, 0.587, 0.114,
        0.5959, -0.2746, -0.3213,
        0.2115, -0.5227, 0.3112
    );

    return mul(mat, rgb);
}
float3 YIQtoRGB(float3 yiq)
{
    const float3x3 mat = float3x3(
        1.0, 0.956, 0.619,
        1.0, -0.272, -0.647,
        1.0, -1.106, 1.703
    );

    return mul(mat, yiq);
}

float3 RGBtoYCbCr(float3 rgb)
{
    const float3x3 mat = float3x3(
        0.299, 0.587, 0.114,
        -0.168736, -0.331264, 0.5,
        0.5, -0.418688, -0.081312
    );

    return mul(mat, rgb);
}
float3 YCbCrtoRGB(float3 yCbCr)
{
    const float3x3 mat = float3x3(
        1, 0, 1.402,
        1, -0.34414, -0.71414,
        1, 1.77200, 0
    );

    return mul(mat, yCbCr);
}

// XYZ matrices from https://www.cs.rit.edu/~ncs/color/t_convert.html
float3 RGBtoXYZ(float3 rgb)
{
    const float3x3 mat = float3x3(
        0.412453, 0.357580, 0.180423,
        0.212671, 0.715160, 0.072169,
        0.019334, 0.119193, 0.950227
    );

    return mul(mat, rgb);
}
float3 XYZtoRGB(float3 xyz)
{
    const float3x3 mat = float3x3(
        3.240479, -1.537150, -0.498535,
        -0.969256, 1.875992, 0.041556,
        0.055648, -0.204043, 1.057311
    );

    return mul(mat, xyz);
}

// From here https://stackoverflow.com/questions/15095909/from-rgb-to-hsv-in-opengl-glsl/17897228#17897228
float3 RGBtoHSV(float3 c)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
float3 HSVtoRGB(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}


float _Sharpness;
float _ChromaFreq;
float _PhaseOffset;
float _FramePhaseOffset;

#define CHROMA_FREQ _ChromaFreq
#define CHROMA_AMP 1.5

#define MAX_AMP (1.0 + CHROMA_AMP)


float PackSignal(float signal)
{
    return signal / (MAX_AMP) + 0.5;
}
float3 PackSignal(float3 signal)
{
    return float3(
        PackSignal(signal.x),
        PackSignal(signal.y),
        PackSignal(signal.z)
    );
}
float UnpackSignal(float signal)
{
    return (signal - 0.5) * (MAX_AMP);
}
float3 UnpackSignal(float3 signal)
{
    return float3(
        UnpackSignal(signal.x),
        UnpackSignal(signal.y),
        UnpackSignal(signal.z)
    );
}


float GetChromaPhase(float2 uv)
{
    float scanline = floor(uv.y * SCANLINES);
    float scanlineOffset = 0.5;

    float phase = uv.x * CHROMA_FREQ +
        _PhaseOffset +
        scanline * scanlineOffset +
        FRAME_COUNT * 0.5;

    return phase * 2*PI;
}

float Edge(float2 uv)
{
    return saturate(1.0 - abs(floor(uv.x)));
}
float3 SampleSignal(float2 uv)
{
    return UnpackSignal( clamp(tex2Dlod(_MainTex, float4(uv, 0, 0)).rgb, -MAX_AMP, MAX_AMP) ) * Edge(uv);
}

float3 Filter(float2 uv)
{
    const int samples = 12;

    const float range = 1 / CHROMA_FREQ;
    const float dx = range / samples;

    float3 signal = 0;

    uv.x -= range/2;

    [unroll]
    for(int i = 0; i <= samples; i++)
    {
        // Trapezoidal rule
        float weight = (i == 0 || i == samples) ? 0.5 : 1;

        signal += SampleSignal(uv) * weight;
        uv.x += dx;
    }

    return signal / samples;
}

float4 Encode(float3 rgb, float2 uv)
{
    uv.y = (floor(uv.y * SCANLINES) + 0.5) / SCANLINES;

    float3 yiq = RGBtoYIQ(rgb);

    yiq.yz *= CHROMA_AMP;

    float phase = GetChromaPhase(uv);
    float i_mod, q_mod;
    sincos(phase, q_mod, i_mod);

    float chroma_signal = yiq.y * i_mod + yiq.z * q_mod;

    float s = yiq.x + chroma_signal;
    float3 signal = float3(s, s * i_mod, s * q_mod);;

    signal = PackSignal(signal);

    return float4(signal, 1);
}

float4 Decode(float2 uv)
{
    uv.y = (floor(uv.y * SCANLINES) + 0.5) / SCANLINES;

    float3 raw = Filter(uv);

    float luma = raw.x;
    float2 chroma = raw.yz * 2 / CHROMA_AMP;

    float3 yiq = float3(luma, chroma);
    float3 rgb = saturate(YIQtoRGB(yiq));

    // Gamma
    rgb = pow(rgb, 1.25);

    return float4(rgb, 1);
}