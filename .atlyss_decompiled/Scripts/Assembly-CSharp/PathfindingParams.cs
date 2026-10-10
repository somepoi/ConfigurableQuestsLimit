using System;
using UnityEngine;

[Serializable]
public class PathfindingParams
{
	public bool _makeKinematicOnDeath;

	public bool _alwaysKinematic;

	public bool _ignoreGravity;

	public float _creepPushColliderRadius = 6f;

	[Header("Movement Speed")]
	public float _movementSpeed = 35.5f;

	public float _stoppingDistance = 10.65f;

	[Header("Wanderer Params")]
	public bool _wanderingPathfinder = true;

	[Header("Height / Ledge Jump Check")]
	public bool _disableLedgeJump;

	public bool _disableHeightJump;
}
