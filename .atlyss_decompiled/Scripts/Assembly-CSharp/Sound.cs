using System;
using UnityEngine;

[Serializable]
public class Sound
{
	public string name;

	public AudioClip clip;

	[Range(0f, 1f)]
	public float volume = 1f;

	[Range(0f, 3f)]
	public float pitch = 1f;

	[Range(0f, 256f)]
	public int priority;

	[Range(0f, 1f)]
	public float spatialBlend;

	public float dopplerLevel;

	public AudioRolloffMode rollOff;

	public float minDistance;

	public float maxDistance = 500f;

	public bool loop;

	[HideInInspector]
	public AudioSource source;
}
