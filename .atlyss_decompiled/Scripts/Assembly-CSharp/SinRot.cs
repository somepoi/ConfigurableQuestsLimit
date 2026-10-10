using UnityEngine;

public class SinRot : MonoBehaviour
{
	public float radius = 10f;

	public float speed = 1f;

	public bool offsetIsCenter = true;

	public Vector3 offset;

	public RectTransform pos;

	private void Start()
	{
		if (offsetIsCenter)
		{
			offset = pos.anchoredPosition;
		}
	}

	private void Update()
	{
		pos.anchoredPosition = new Vector3(radius * Mathf.Cos(Time.time * speed) + offset.x, radius * Mathf.Sin(Time.time * speed) + offset.y, offset.z);
	}
}
