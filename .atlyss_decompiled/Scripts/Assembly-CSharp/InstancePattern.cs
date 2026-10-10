using System;
using Mirror;
using UnityEngine;

[Serializable]
public class InstancePattern
{
	[HideInInspector]
	public string _patternTag = string.Empty;

	public GameObject _patternContainerObject;

	[Space]
	public SpawnPoint _setSpawnPoint;

	public SpawnPoint _setRespawnPoint;

	[Space]
	public NetworkIdentity[] _netObjects;

	public Teleporter[] _teleporters;

	public PatternInstanceObject[] _localObjects;
}
