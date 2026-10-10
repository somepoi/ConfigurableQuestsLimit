using Mirror;
using UnityEngine;

public class SkillBehavior_Recall : SkillObjectBehavior
{
	[Header("Recall Skill")]
	[Scene]
	public string _sceneToTransfer = string.Empty;

	public string _spawnTag = "";

	public override void End_SkillBehavior()
	{
	}

	public override void Init_SkillBehavior()
	{
		if (!_skillObject._parentPlayer || _skillObject._parentPlayer._currentPlayerCondition != PlayerCondition.ACTIVE || !_skillObject._parentPlayer.Network_playerMapInstance)
		{
			return;
		}
		Player parentPlayer = _skillObject._parentPlayer;
		if (!parentPlayer)
		{
			return;
		}
		if (parentPlayer.Network_playerMapInstance._zoneType != ZoneType.Safe && parentPlayer.Network_playerMapInstance._zoneType != ZoneType.Dungeon)
		{
			parentPlayer._recalledMapInstanceName = parentPlayer.Network_playerMapInstance._mapName;
			parentPlayer._recalledMapInstance = parentPlayer.Network_playerMapInstance.gameObject.scene.path;
			if (parentPlayer.isServer)
			{
				parentPlayer.Network_recallSpawnPoint = parentPlayer._lastSpawnPoint;
			}
		}
		parentPlayer.Server_SceneTransport(_sceneToTransfer, _spawnTag, ZoneDifficulty.NORMAL);
	}

	public override void Update_SkillBehavior()
	{
	}
}
