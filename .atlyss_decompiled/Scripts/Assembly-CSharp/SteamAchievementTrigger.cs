using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SteamAchievementTrigger : MonoBehaviour
{
	[Header("Achievement Trigger")]
	[SerializeField]
	private string _triggerTag = "stat_00";

	[Space]
	[SerializeField]
	private PatternInstanceManager _patternInstance;

	[SerializeField]
	private ZoneDifficulty _setDifficulty;

	private void OnValidate()
	{
		base.gameObject.name = "_achievementTrigger(" + _triggerTag + ")";
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.gameObject.GetComponentInParent<Player>() && SteamManager.Initialized)
		{
			Player componentInParent = other.gameObject.GetComponentInParent<Player>();
			if ((!componentInParent._isHostPlayer || !(componentInParent.gameObject.scene != base.gameObject.scene)) && !componentInParent._bufferingStatus && componentInParent.isLocalPlayer && (!_patternInstance || _patternInstance._setDungeonDifficulty == _setDifficulty))
			{
				SteamAchievementManager.current.Init_EarnAchievement(_triggerTag);
			}
		}
	}
}
