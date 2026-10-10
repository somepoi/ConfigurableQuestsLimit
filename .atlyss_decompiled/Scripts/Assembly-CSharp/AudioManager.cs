using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	public bool isGlobalAudioManager;

	public Sound[] sounds;

	private void Awake()
	{
		if (isGlobalAudioManager)
		{
			if (!(instance == null))
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
		}
		Sound[] array = sounds;
		foreach (Sound sound in array)
		{
			sound.source = base.gameObject.AddComponent<AudioSource>();
			sound.source.playOnAwake = false;
			sound.source.clip = sound.clip;
			sound.source.volume = sound.volume;
			sound.source.pitch = sound.pitch;
			sound.source.loop = sound.loop;
			sound.source.priority = sound.priority;
			sound.source.spatialBlend = sound.spatialBlend;
			sound.source.minDistance = sound.minDistance;
			sound.source.maxDistance = sound.maxDistance;
			sound.source.rolloffMode = sound.rollOff;
			sound.source.dopplerLevel = 0f;
		}
	}

	public void Play(string name)
	{
		Sound sound = Array.Find(sounds, (Sound sound2) => sound2.name == name);
		sound.source.Play();
		if (sound == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
		}
	}

	public void Stop(string name)
	{
		Sound sound = Array.Find(sounds, (Sound sound2) => sound2.name == name);
		sound.source.Stop();
		if (sound == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
		}
	}

	public void StopAllLoops()
	{
		Sound[] array = sounds;
		foreach (Sound sound in array)
		{
			if (sound.loop)
			{
				sound.source.Stop();
			}
		}
	}
}
