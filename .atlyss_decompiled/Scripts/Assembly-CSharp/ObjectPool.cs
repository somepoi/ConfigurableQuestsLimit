using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPool
{
	public string _poolTag;

	public GameObject _spawnedPrefab;

	public GameObject _objectContainer;

	public List<GameObject> _objectPool;

	public int _objectCount;

	public float _timer;

	public float _inactivityTimer;

	public bool _flaggedInactivity;
}
