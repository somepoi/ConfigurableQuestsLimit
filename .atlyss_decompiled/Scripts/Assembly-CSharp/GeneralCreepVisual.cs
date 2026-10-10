using System;
using UnityEngine;

[Serializable]
public class GeneralCreepVisual
{
	public float _creepVisualScale = 1f;

	public float _deathAnimDespawnTime = 3f;

	[Space]
	public float _shadowProjectionScale = 1f;

	public float _modifierEmblemTexturePos = 5f;

	[Header("UI")]
	public bool _showHealthBar = true;

	public float _healthBarYPosition = 2.5f;

	public Vector3 _healthBarSize = Vector3.one;

	[Space]
	public float _targeterOffset = 2.5f;

	public float _lockonArrowOffset = 8.5f;

	public float _aggroiconYPosition = 6.5f;

	[Header("Blood")]
	public float _bloodEffectSize = 1f;

	public float _gibEffectSize = 1f;

	public bool _canGib = true;

	[Space]
	public Color _creepBloodColor = Color.red;

	public Color _creepBloodSubColor = new Color(1f, 0f, 1f, 0.45f);

	public Color _creepGibColor = Color.red;

	public Color _creepGibSubColor = new Color(1f, 0f, 1f, 0.45f);
}
