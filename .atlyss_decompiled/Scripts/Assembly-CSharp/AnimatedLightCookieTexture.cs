using System.Collections;
using UnityEngine;

public class AnimatedLightCookieTexture : MonoBehaviour
{
	[SerializeField]
	private Light _lightSource;

	[SerializeField]
	private Texture[] _lightCookieTextures = new Texture[1];

	[SerializeField]
	private float _framerate = 0.1f;

	private int _currentTexIndex;

	private void Start()
	{
		StartCoroutine(AnimateTextures());
	}

	private IEnumerator AnimateTextures()
	{
		while (true)
		{
			_lightSource.cookie = _lightCookieTextures[_currentTexIndex];
			yield return new WaitForSeconds(_framerate);
			_currentTexIndex++;
			if (_currentTexIndex >= _lightCookieTextures.Length)
			{
				_currentTexIndex = 0;
			}
		}
	}
}
