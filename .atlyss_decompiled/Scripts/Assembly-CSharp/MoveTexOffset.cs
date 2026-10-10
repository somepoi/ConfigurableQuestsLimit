using UnityEngine;

public class MoveTexOffset : MonoBehaviour
{
	private Renderer renderer;

	public int materialIndex;

	public Vector2 uvAnimationRate = new Vector2(1f, 0f);

	public string textureName = "_MainTex";

	private Vector2 uvOffset = Vector2.zero;

	private void LateUpdate()
	{
		if ((bool)Player._mainPlayer && (!Player._mainPlayer._isHostPlayer || !(Player._mainPlayer.gameObject.scene != base.gameObject.scene)))
		{
			renderer = GetComponent<Renderer>();
			uvOffset += uvAnimationRate * Time.deltaTime;
			if (renderer.enabled)
			{
				renderer.materials[materialIndex].SetTextureOffset(textureName, uvOffset);
			}
		}
	}
}
