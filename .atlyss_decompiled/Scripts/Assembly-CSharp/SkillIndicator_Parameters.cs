using System;
using UnityEngine;

[Serializable]
public class SkillIndicator_Parameters
{
	[Header("Display")]
	public Texture _indicatorTexture;

	public Mesh _indicatorMesh;

	public Color _confirmColor = Color.cyan;

	public Color _denyColor = Color.red;

	[Header("Distance Calculation")]
	public bool _measureDistanceFromPlayer = true;

	public float _distanceToDeny = 20.5f;

	[Header("Logic")]
	public LayerMask layerMask;

	public Vector3 _clampAngleAxis = Vector3.zero;

	public bool _snapAngle = true;
}
