using System;
using Mirror;
using UnityEngine;

[Serializable]
public class ScenePortalData
{
	public PortalType _portalType;

	[Scene]
	public string _subScene = string.Empty;

	public string _portalCaptionTitle = "";

	public string _spawnPointTag = "startPoint";

	[Header("Dungeon Portal")]
	public DialogTrigger _portalDialogTrigger;

	public int _easyLevelRequirement = 1;

	public int _maxEasyLevel = 1;

	[Space]
	public int _normalLevelRequirement = 1;

	public int _maxNormalLevel = 1;

	[Space]
	public int _hardLevelRequirement = 1;

	public int _maxHardLevel = 1;
}
