using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound_MapAmbience : MonoBehaviour
{
	public MapInstance _setMapInstance;

	public WorldTime _ambienceType = WorldTime.NULL;

	[Space]
	public float _setAmbienceVolume = 0.65f;

	public AudioSource _aSrc;

	public virtual void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position + Vector3.up * 4.5f, "_ambienceSfxIco", allowScaling: false);
	}

	private void OnValidate()
	{
		_aSrc = GetComponent<AudioSource>();
		if (_aSrc.clip != null)
		{
			base.name = $"_entity_mapAmbience ({_aSrc.clip.name} / {_ambienceType})";
		}
	}

	private void Awake()
	{
		_aSrc.volume = 0f;
	}

	private void Update()
	{
		if ((bool)Player._mainPlayer && !(Player._mainPlayer.Network_playerMapInstance != _setMapInstance) && (bool)_setMapInstance)
		{
			if (Player._mainPlayer._bufferingStatus)
			{
				_aSrc.volume = Mathf.Lerp(_aSrc.volume, 0f, 0.07f * Time.deltaTime);
			}
			else if (_ambienceType == WorldTime.NULL)
			{
				_aSrc.volume = Mathf.Lerp(_aSrc.volume, _setAmbienceVolume, 0.07f * Time.deltaTime);
			}
			else if (_setMapInstance._instanceWorldTime != _ambienceType)
			{
				_aSrc.volume = Mathf.Lerp(_aSrc.volume, 0f, 0.07f * Time.deltaTime);
			}
			else
			{
				_aSrc.volume = Mathf.Lerp(_aSrc.volume, _setAmbienceVolume, 0.07f * Time.deltaTime);
			}
		}
	}
}
