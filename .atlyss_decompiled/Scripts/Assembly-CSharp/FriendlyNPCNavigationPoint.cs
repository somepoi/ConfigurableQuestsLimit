using System;
using UnityEngine;

[Serializable]
public class FriendlyNPCNavigationPoint
{
	public Transform _waypoint;

	public float _distanceToStop = 3.5f;

	public string _animationOnStop = string.Empty;

	[Range(0f, 1f)]
	public float _chanceToAnimationOnStop;

	public float _bufferTime = 7.5f;
}
