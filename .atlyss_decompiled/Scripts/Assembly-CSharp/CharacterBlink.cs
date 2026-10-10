using UnityEngine;

public class CharacterBlink : MonoBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer _eyeRender;

	private float _blinkBuffer;

	private void Update()
	{
		BlinkEffect();
	}

	private void BlinkEffect()
	{
		if (!_eyeRender)
		{
			return;
		}
		_eyeRender.SetBlendShapeWeight(0, Mathf.Lerp(_eyeRender.GetBlendShapeWeight(0), 0f, 16.5f * Time.deltaTime));
		if (_blinkBuffer > 0f)
		{
			_blinkBuffer -= Time.deltaTime;
			return;
		}
		if (Random.Range(0, 100) < 10)
		{
			_eyeRender.SetBlendShapeWeight(0, 100f);
		}
		_blinkBuffer = Random.Range(0.25f, 1.25f);
	}
}
