using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[ImageEffectAllowedInSceneView]
public class CRT : MonoBehaviour
{
	#pragma warning disable
	[SerializeField] Vector2 resolution = new Vector2(640, 480);
	[SerializeField] Material material;
	#pragma warning restore

    private const int
		maskPass = 0,
		curvaturePass = 1;

	private static readonly int crtResolutionPID = Shader.PropertyToID("_CRTResolution");


	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
        material.SetVector(crtResolutionPID, new Vector4(resolution.x, resolution.y, 0f, 0f));

        RenderTexture temp = RenderTexture.GetTemporary(src.descriptor);

		Graphics.Blit(src, temp, material, maskPass);
        Graphics.Blit(temp, dest, material, curvaturePass);

        RenderTexture.ReleaseTemporary(temp);
	}
}