using System;
using UnityEngine;

[Serializable]
public class DelaySpawn
{
	[Header("Delay Spawn")]
	public float _timeDelay = 1f;

	public GameObject _objectToSpawn;

	public bool _useObjectPool = true;

	public bool _copyParentRotation = true;

	public Vector3 _rotationAdjust = Vector3.zero;
}
