using UnityEngine;

public class Scr_Pixelate : MonoBehaviour
{
	[SerializeField]
	private Material m_mat;

	private void Start()
	{
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, m_mat);
	}
}
