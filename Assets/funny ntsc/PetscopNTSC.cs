using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[ImageEffectAllowedInSceneView]
public class PetscopNTSC : MonoBehaviour
{
	[SerializeField] Material mat;
	bool inv;

	void Awake()
	{
		Cursor.visible = false;
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, mat);
	}

	void Update()
	{
		mat.SetFloat("DistInvert", (Time.time * 30) % 2 >= 1 ? 1f : -1f);
	}
}
