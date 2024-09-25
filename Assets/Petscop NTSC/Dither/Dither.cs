using UnityEngine;

[ExecuteInEditMode]
public class Dither : MonoBehaviour
{
	public Material mat;

    void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit (source, destination, mat);
	}
}
