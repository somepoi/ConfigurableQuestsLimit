using System;
using UnityEngine;

[Serializable]
public class CreepArenaSlot
{
	[HideInInspector]
	public string _arenaSlotTag = string.Empty;

	public ZoneDifficulty _zoneDifficulty;

	public WorldTime _arenaTime = WorldTime.NULL;

	public CreepArenaWave[] _creepArenaWaves;
}
