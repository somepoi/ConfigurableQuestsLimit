using UnityEngine;

public class LightSpriteDistance : MonoBehaviour
{
	[SerializeField]
	private Transform _flareSpriteTransform;

	[SerializeField]
	private SpriteRenderer _flareSpriteRenderer;

	private Color _originColor = Color.white;

	private Transform _m_Camera;

	[SerializeField]
	private float _minScale = 6.5f;

	[SerializeField]
	private float _maxScale = 25.5f;

	[SerializeField]
	private float _solidDistance = 15f;

	[SerializeField]
	private float _solidDistModifier = 2.5f;

	private void Awake()
	{
		_originColor = _flareSpriteRenderer.color;
	}

	private void Update()
	{
		if (!CameraFunction._current)
		{
			return;
		}
		if (!_m_Camera)
		{
			_m_Camera = CameraFunction._current._mainCamera.transform;
			return;
		}
		float value = Vector3.Distance(_m_Camera.position, base.transform.position);
		value = Mathf.Clamp(value, _minScale, _maxScale);
		float num = Vector3.Distance(_m_Camera.position, base.transform.position);
		if (num < _solidDistance)
		{
			num = _solidDistance;
		}
		_flareSpriteTransform.localScale = Vector3.one * value;
		_flareSpriteRenderer.color = new Color(_originColor.r, _originColor.g, _originColor.b, _solidDistance / num * _solidDistModifier);
	}
}
