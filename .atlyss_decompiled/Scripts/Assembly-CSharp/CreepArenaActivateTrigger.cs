using UnityEngine;

public class CreepArenaActivateTrigger : MonoBehaviour
{
	[SerializeField]
	private CreepArena _creepArena;

	private void Awake()
	{
		if (!_creepArena.isServer)
		{
			base.enabled = false;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		Handle_ServerTriggerStay(other);
	}

	private void Handle_ServerTriggerStay(Collider other)
	{
		if (!(_creepArena._currentResetBuffer > 0f) && !_creepArena._arenaEnabled && _creepArena._arenaReady && !(other.gameObject.scene != base.gameObject.scene) && (bool)other.gameObject.GetComponentInParent<Player>())
		{
			Player componentInParent = other.gameObject.GetComponentInParent<Player>();
			if (componentInParent._currentPlayerCondition == PlayerCondition.ACTIVE && !componentInParent._bufferingStatus && !(componentInParent._spawnBuffer > 0f))
			{
				_creepArena.Server_InitArenaSession();
			}
		}
	}
}
