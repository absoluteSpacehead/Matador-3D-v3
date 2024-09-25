using UnityEngine;

[ExecuteInEditMode]
public class AspectUtility : MonoBehaviour
{

	[SerializeField] Camera cam;
	[SerializeField] float wantedAspectRatio;
	Vector2 screenres;

	void Awake()
	{
		screenres = new Vector2(Screen.height, Screen.width);
		SetCamera();
	}

	void Update()
	{
		Vector2 currentscreenres = new Vector2(Screen.height, Screen.width);
		if (screenres != currentscreenres)
		{
			screenres = currentscreenres;
			SetCamera();
		}
	}

	void SetCamera()
	{
		float currentAspectRatio = (float)Screen.width / Screen.height;
		if ((int)(currentAspectRatio * 100) / 100.0f == (int)(wantedAspectRatio * 100) / 100.0f)
		{
			cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
			return;
		}
		if (currentAspectRatio > wantedAspectRatio)
		{
			float inset = 1.0f - wantedAspectRatio / currentAspectRatio;
			cam.rect = new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f);
		}
		else
		{
			float inset = 1.0f - currentAspectRatio / wantedAspectRatio;
			cam.rect = new Rect(0.0f, inset / 2, 1.0f, 1.0f - inset);
		}
	}
}