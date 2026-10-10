using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class QuestTrigger : NetworkBehaviour, IInteractable
{
	public MapInstance _mapInstance;

	[Header("Quest Trigger")]
	public ScriptableQuest _scriptQuest;

	[SerializeField]
	private string _questTriggerTag = string.Empty;

	[SerializeField]
	private Collider _triggerCollider;

	[SerializeField]
	private bool _interactTrigger;

	public bool _completedRequirements = true;

	[SerializeField]
	private bool _questTriggerOnlyOnce;

	[Header("Invoke Net Trigger")]
	[SerializeField]
	private NetTrigger _netTriggerToInvoke;

	[SerializeField]
	private QuestTriggerCondition _netTriggerType = QuestTriggerCondition.ENABLE_DURING_QUEST;

	[Header("Message")]
	public TriggerMessage _triggerMessage;

	[Header("Instance Pattern Quest")]
	public PatternInstanceManager _patternInstanceManager;

	public ZoneDifficulty _difficultyRequirement = ZoneDifficulty.NORMAL;

	public bool _arenaSweepQuest;

	[Header("Display")]
	[SerializeField]
	private GameObject _visualContainer;

	[SerializeField]
	private Animator _questTriggerAnimator;

	[SerializeField]
	private ParticleSystem[] _questTriggerParticles;

	[Space]
	[SerializeField]
	private AudioSource _aSrcOnInteract;

	[SerializeField]
	private bool _disableVisualIfNotOnQuest;

	protected override void OnValidate()
	{
		base.OnValidate();
		if ((bool)_scriptQuest)
		{
			base.name = "_QuestTrigger(" + _scriptQuest._questName + ", " + _questTriggerTag + ")";
		}
		else
		{
			base.name = "_QuestTrigger(Empty, " + _questTriggerTag + ")";
		}
	}

	private void Update()
	{
		if ((bool)Player._mainPlayer && (bool)_visualContainer && _disableVisualIfNotOnQuest && Player._mainPlayer._bufferingStatus)
		{
			bool active = Player._mainPlayer._pQuest._scriptableQuests.Contains(_scriptQuest);
			_visualContainer.SetActive(active);
		}
		Handle_InteractTrigger();
		Server_NetTriggerRelation();
		if (_interactTrigger)
		{
			base.gameObject.layer = LayerMask.NameToLayer("Interactable");
		}
		else
		{
			base.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		}
		if ((bool)_patternInstanceManager && _arenaSweepQuest)
		{
			_completedRequirements = _patternInstanceManager._allArenasBeaten;
		}
		if (base.isServer && _arenaSweepQuest)
		{
			_questTriggerOnlyOnce = true;
		}
	}

	[ClientCallback]
	private void Handle_InteractTrigger()
	{
		if (NetworkClient.active && (bool)Player._mainPlayer && _interactTrigger)
		{
			PlayerQuesting pQuest = Player._mainPlayer._pQuest;
			_triggerCollider.enabled = pQuest._scriptableQuests.Contains(_scriptQuest);
		}
	}

	[ServerCallback]
	private void Server_NetTriggerRelation()
	{
		if (!NetworkServer.active || !_netTriggerToInvoke || !_mapInstance)
		{
			return;
		}
		for (int i = 0; i < _mapInstance._peersInInstance.Count; i++)
		{
			if (_mapInstance._peersInInstance[i] == null)
			{
				continue;
			}
			PlayerQuesting pQuest = _mapInstance._peersInInstance[i]._pQuest;
			if (!pQuest)
			{
				continue;
			}
			switch (_netTriggerType)
			{
			case QuestTriggerCondition.ENABLE_BEFORE_QUEST:
				if (!pQuest._scriptableQuests.Contains(_scriptQuest) && !pQuest._finishedQuests.ContainsKey(_scriptQuest._questName))
				{
					_netTriggerToInvoke.Network_deactivateTrigger = false;
				}
				return;
			case QuestTriggerCondition.ENABLE_DURING_QUEST:
				if (pQuest._scriptableQuests.Contains(_scriptQuest))
				{
					_netTriggerToInvoke.Network_deactivateTrigger = false;
					return;
				}
				break;
			case QuestTriggerCondition.ENABLE_AFTER_COMPLETED_QUEST:
				if (pQuest._finishedQuests.ContainsKey(_scriptQuest._questName))
				{
					_netTriggerToInvoke.Network_deactivateTrigger = false;
					return;
				}
				break;
			case QuestTriggerCondition.ENABLE_DURING_AND_AFTER_QUEST:
				if (pQuest._scriptableQuests.Contains(_scriptQuest) || pQuest._finishedQuests.ContainsKey(_scriptQuest._questName))
				{
					_netTriggerToInvoke.Network_deactivateTrigger = false;
				}
				return;
			}
		}
		_netTriggerToInvoke.Network_deactivateTrigger = true;
	}

	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (!NetworkServer.active || base.gameObject.scene != other.gameObject.scene || !other.GetComponentInParent<Player>() || _interactTrigger)
		{
			return;
		}
		Player componentInParent = other.GetComponentInParent<Player>();
		PlayerQuesting pQuest = componentInParent._pQuest;
		if (componentInParent._bufferingStatus || (!pQuest._scriptableQuests.Contains(_scriptQuest) && _netTriggerType == QuestTriggerCondition.ENABLE_DURING_QUEST) || ((bool)_patternInstanceManager && _patternInstanceManager._setDungeonDifficulty != _difficultyRequirement))
		{
			return;
		}
		Init_SendTriggerMessage(_completedRequirements);
		if (!_completedRequirements)
		{
			return;
		}
		pQuest.Target_QueryQuestTriggerProgress(_questTriggerTag);
		if ((bool)_netTriggerToInvoke)
		{
			_netTriggerToInvoke.Init_EnableTrigger();
		}
		if ((bool)componentInParent.Network_partyObject)
		{
			PartyObjectBehavior network_partyObject = componentInParent.Network_partyObject;
			for (int i = 0; i < network_partyObject._syncPartyPeers.Count; i++)
			{
				network_partyObject._syncPartyPeers[i]._pQuest.Target_QueryQuestTriggerProgress(_questTriggerTag);
			}
		}
		if (_questTriggerOnlyOnce)
		{
			NetworkServer.Destroy(base.gameObject);
		}
	}

	[ServerCallback]
	public void Init_PickupItem(NetworkIdentity _netID)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		PlayerQuesting component = _netID.GetComponent<PlayerQuesting>();
		if (component._scriptableQuests.Contains(_scriptQuest) && (!_patternInstanceManager || _patternInstanceManager._setDungeonDifficulty == _difficultyRequirement))
		{
			component.Target_QueryQuestTriggerProgress(_questTriggerTag);
			Rpc_InitEffect();
			if ((bool)_netTriggerToInvoke)
			{
				_netTriggerToInvoke.Init_EnableTrigger();
			}
		}
	}

	[ClientRpc]
	private void Rpc_InitEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void QuestTrigger::Rpc_InitEffect()", 1092219734, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	private void Init_SendTriggerMessage(bool _isCompletedRequirements)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		string text = _triggerMessage._singleMessage;
		if (_triggerMessage._triggerMessageArray.Length != 0)
		{
			text = _triggerMessage._triggerMessageArray[Random.Range(0, _triggerMessage._triggerMessageArray.Length)];
		}
		if (!_isCompletedRequirements)
		{
			text = _triggerMessage._incompleteMessage;
		}
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		ChatBehaviour[] array = Object.FindObjectsOfType<ChatBehaviour>();
		foreach (ChatBehaviour chatBehaviour in array)
		{
			if (!(chatBehaviour == null) && !(chatBehaviour.gameObject.scene != base.gameObject.scene))
			{
				if (_triggerMessage._sentMsg)
				{
					break;
				}
				chatBehaviour.Target_RecieveTriggerMessage(text);
			}
		}
		_triggerMessage._sentMsg = true;
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Rpc_InitEffect()
	{
		if ((bool)Player._mainPlayer && Player._mainPlayer._isHostPlayer && base.gameObject.scene != Player._mainPlayer.gameObject.scene)
		{
			return;
		}
		if (_questTriggerParticles != null && _questTriggerParticles.Length != 0)
		{
			for (int i = 0; i < _questTriggerParticles.Length; i++)
			{
				_questTriggerParticles[i].Play();
			}
		}
		if ((bool)_aSrcOnInteract && _aSrcOnInteract.enabled)
		{
			_aSrcOnInteract.Play();
		}
	}

	protected static void InvokeUserCode_Rpc_InitEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_InitEffect called on server.");
		}
		else
		{
			((QuestTrigger)obj).UserCode_Rpc_InitEffect();
		}
	}

	static QuestTrigger()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(QuestTrigger), "System.Void QuestTrigger::Rpc_InitEffect()", InvokeUserCode_Rpc_InitEffect);
	}
}
