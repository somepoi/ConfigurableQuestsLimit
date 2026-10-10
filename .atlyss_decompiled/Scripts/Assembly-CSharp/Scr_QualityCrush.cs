using UnityEngine;

public class Scr_QualityCrush : MonoBehaviour
{
	public Shader imageEffect;

	public bool autoResolution;

	public Vector2 screenResolution = new Vector2(320f, 240f);

	public float colorResolution = 5f;

	public bool _7GenRes;

	private Material m_mat;

	private void Start()
	{
		if ((bool)SettingsManager._current)
		{
			SettingsManager._current._foundQualityCrushes.Add(this);
		}
		if (Shader.Find("Image Effects/Quality Crush") != null)
		{
			imageEffect = Shader.Find("Image Effects/Quality Crush");
		}
		else
		{
			Debug.LogWarning("A shader is missing: Quality Crush could not be found in your shaders list. Are you sure it's still there?");
		}
		m_mat = new Material(imageEffect);
		_7GenRes = true;
	}

	private void Update()
	{
		if (autoResolution)
		{
			m_mat.SetInt("_ResolutionX", Screen.width);
			m_mat.SetInt("_ResolutionY", Screen.height);
		}
		else
		{
			m_mat.SetInt("_ResolutionX", (int)screenResolution.x);
			m_mat.SetInt("_ResolutionY", (int)screenResolution.y);
		}
		if (_7GenRes)
		{
			m_mat.SetInt("_ResolutionX", 960);
			m_mat.SetInt("_ResolutionY", 480);
			colorResolution = 40f;
		}
		m_mat.SetFloat("_Colors", (colorResolution <= 0f) ? 1f : colorResolution);
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, m_mat);
	}
}
