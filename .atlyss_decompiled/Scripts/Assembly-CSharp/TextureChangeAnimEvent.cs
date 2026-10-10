using UnityEngine;

public class TextureChangeAnimEvent : MonoBehaviour
{
	[Header("Texture Change Event")]
	[SerializeField]
	private TextureChangeAnimStruct[] _textureStructs;

	[SerializeField]
	private Renderer _render;

	[SerializeField]
	private int _matIndex;

	private float _textureBufferTime;

	private bool _isTextureChanged;

	[Header("Particle Effects")]
	[SerializeField]
	private ParticleSystem[] _particleSystems;

	[Header("Audio")]
	[SerializeField]
	private AudioSource[] _aSrcs;

	[Space]
	[SerializeField]
	private AudioSource _aSrc;

	[SerializeField]
	private AudioClip[] _sfxClips;

	private void Update()
	{
		if (_textureBufferTime > 0f)
		{
			_textureBufferTime -= Time.deltaTime;
		}
		if (_textureBufferTime <= 0f && _isTextureChanged)
		{
			Material[] materials = _render.materials;
			materials[_matIndex].mainTexture = _textureStructs[0]._texture;
			_render.materials = materials;
			_isTextureChanged = false;
		}
	}

	public void ChangeTexture(int _value)
	{
		Material[] materials = _render.materials;
		materials[_matIndex].mainTexture = _textureStructs[_value]._texture;
		_render.materials = materials;
		_textureBufferTime = _textureStructs[_value]._bufferTime;
		_isTextureChanged = true;
	}

	public void PlaySound(int _index)
	{
		if (_sfxClips != null && _sfxClips.Length != 0 && (!Player._mainPlayer || !Player._mainPlayer._isHostPlayer || !(Player._mainPlayer.gameObject.scene != base.gameObject.scene)))
		{
			_aSrc.PlayOneShot(_sfxClips[_index], 0.45f);
		}
	}

	public void Play_AudioSourceIndex(int _i)
	{
		if (!Player._mainPlayer || !Player._mainPlayer._isHostPlayer || !(Player._mainPlayer.gameObject.scene != base.gameObject.scene))
		{
			_aSrcs[_i].Play();
		}
	}

	public void PlayParticleSystem(int _index)
	{
		if ((!Player._mainPlayer || !Player._mainPlayer._isHostPlayer || !(Player._mainPlayer.gameObject.scene != base.gameObject.scene)) && !_particleSystems[_index].isEmitting)
		{
			_particleSystems[_index].Play();
		}
	}
}
