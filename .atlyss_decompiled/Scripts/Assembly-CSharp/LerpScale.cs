using UnityEngine;

public class LerpScale : MonoBehaviour
{
	[SerializeField]
	private float lerpSpeed = 12.5f;

	public Vector3 setScale = Vector3.one;

	public Vector3 originScale = Vector3.one;

	private void OnEnable()
	{
		Apply_OriginScale();
	}

	public void Apply_OriginScale()
	{
		base.transform.localScale = originScale;
	}

	private void Update()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, setScale, lerpSpeed * Time.deltaTime);
	}
}
