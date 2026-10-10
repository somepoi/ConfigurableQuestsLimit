using System;
using UnityEngine;

[Serializable]
public class GeneralCreepAudio
{
	[Range(-3f, 3f)]
	public float _dynamicAudioPitch = 1f;

	[Space]
	public AudioClip[] _attackSfx = new AudioClip[1];

	public AudioClip _painSfx;

	public AudioClip _deathSfx;
}
