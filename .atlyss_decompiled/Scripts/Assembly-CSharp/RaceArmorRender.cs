using System;
using UnityEngine;

[Serializable]
public class RaceArmorRender
{
	public string _raceNameTag = string.Empty;

	public ScriptablePlayerRace _scriptableRace;

	public Mesh _raceArmorMesh;

	[Header("Chestpiece Rendering")]
	public Mesh _boobOverrideMesh;

	[Header("Non-Skinned Transforms")]
	public Vector3 _meshPosition = new Vector3(0f, 0.005f, 0f);

	public Vector3 _meshRotation = Vector3.zero;

	public Vector3 _meshScale = new Vector3(0.008f, 0.008f, 0.008f);
}
