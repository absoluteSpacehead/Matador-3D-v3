using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView, RequireComponent(typeof(Camera))]
public class AnalogVideo : MonoBehaviour
{
	#pragma warning disable
	[SerializeField] float chromaFreq = 170.666f;
	[SerializeField, Range(1, 4096)] int horizontalResolution = 2048;
	#pragma warning restore

	private Material mat;

	private const int
		encodePass = 0,
		decodePass = 1,
		finalPass = 2;
		
	private static readonly int chromaFreqPID = Shader.PropertyToID("_ChromaFreq");
	private static readonly int frameTimePID = Shader.PropertyToID("_FrameTime");
	private static readonly int outputResPID = Shader.PropertyToID("_OutputRes");

	private float frameTime = 0f;


	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if (mat == null)
			mat = new Material(Shader.Find("Hidden/AnalogVideo/Main"));

		mat.SetFloat(frameTimePID, frameTime);

		Vector2Int res = new Vector2Int(320, 240);

		mat.SetFloat(chromaFreqPID, Mathf.Floor(chromaFreq * res.x / 320f) + (chromaFreq - Mathf.Floor(chromaFreq)) );
		mat.SetVector(outputResPID, new Vector4(res.x, res.y, 0f, 0f));
		
		int scanlines = Mathf.FloorToInt(res.y);
        RenderTexture temp = RenderTexture.GetTemporary(horizontalResolution, scanlines, 0, RenderTextureFormat.ARGBFloat);

		temp.wrapMode = TextureWrapMode.Repeat;

        Graphics.Blit(src, temp, mat, encodePass);
        Graphics.Blit(temp, src, mat, decodePass);
		Graphics.Blit(src, dest, mat, finalPass);

        RenderTexture.ReleaseTemporary(temp);

		frameTime = (frameTime += Time.deltaTime) % (2f / 30f);
	}
}