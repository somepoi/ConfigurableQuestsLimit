using UnityEngine;

public class FriendlyNPC_Hitbox : MonoBehaviour
{
	private NetNPC _npc;

	[SerializeField]
	private float _disableHitboxBuffer;

	[SerializeField]
	private string _animToEvoke = string.Empty;

	[SerializeField]
	private string _achievementTag = string.Empty;

	private void Awake()
	{
		_npc = GetComponentInParent<NetNPC>();
	}

	private void Update()
	{
		if (_npc.isServer)
		{
			if (_disableHitboxBuffer > 0f)
			{
				_disableHitboxBuffer -= Time.deltaTime;
			}
			else
			{
				_disableHitboxBuffer = 0f;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<CombatCollider>() || _disableHitboxBuffer > 0f || !_npc)
		{
			return;
		}
		CombatCollider component = other.GetComponent<CombatCollider>();
		if ((bool)component._parentObject.GetComponentInParent<Player>() && component._colliderType != CombatColliderType.SUPPORT)
		{
			if (!string.IsNullOrEmpty(_achievementTag) && component._parentObject.GetComponentInParent<Player>() == Player._mainPlayer)
			{
				SteamAchievementManager.current.Init_EarnAchievement(_achievementTag);
			}
			if (_npc.isServer)
			{
				_npc.Rpc_CrossFade_Anim(_animToEvoke, 0f, 0);
			}
		}
	}
}
