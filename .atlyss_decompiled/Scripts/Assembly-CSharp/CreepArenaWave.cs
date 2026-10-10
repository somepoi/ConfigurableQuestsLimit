using System;
using UnityEngine;

[Serializable]
public class CreepArenaWave
{
	[HideInInspector]
	public string _arenaWaveTag = string.Empty;

	public ScriptableCreep[] _creepPool;
}
