using Mirror;
using UnityEngine;

public class QuestPrefabGroup : NetworkBehaviour
{
	[Header("Quest Prefab Group")]
	[SerializeField]
	private MapInstance _mapInstance;

	[SerializeField]
	private ScriptableQuest _scriptableQuest;

	public QuestTriggerCondition _questGroupType = QuestTriggerCondition.ENABLE_DURING_QUEST;

	public NetTrigger[] _netTriggers;

	public CreepSpawner[] _creepSpawners;

	private void Awake()
	{
		if ((bool)GetComponent<MapInstance>())
		{
			_mapInstance = GetComponent<MapInstance>();
		}
		Disable_QuestGroup();
	}

	private void Update()
	{
		if (!_mapInstance)
		{
			Debug.LogError("Map instance not set on this quest prefab group object. Assign an instance to it.");
		}
		else
		{
			Iterate_PeerlistInMapInstance();
		}
	}

	private void Iterate_PeerlistInMapInstance()
	{
		if (!_mapInstance.isServer)
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
			switch (_questGroupType)
			{
			case QuestTriggerCondition.ENABLE_DURING_QUEST:
				if (pQuest._scriptableQuests.Contains(_scriptableQuest))
				{
					Enable_QuestGroup();
				}
				break;
			case QuestTriggerCondition.ENABLE_DURING_AND_AFTER_QUEST:
				if (pQuest._scriptableQuests.Contains(_scriptableQuest))
				{
					Enable_QuestGroup();
				}
				break;
			}
			if (i >= _mapInstance._peersInInstance.Count && _questGroupType != QuestTriggerCondition.ENABLE_BEFORE_QUEST && !pQuest._scriptableQuests.Contains(_scriptableQuest))
			{
				Disable_QuestGroup();
			}
		}
	}

	private void Disable_QuestGroup()
	{
		for (int i = 0; i < _netTriggers.Length; i++)
		{
			_netTriggers[i].Network_deactivateTrigger = true;
		}
		for (int j = 0; j < _creepSpawners.Length; j++)
		{
			_creepSpawners[j].Network_disableSpawner = true;
		}
	}

	private void Enable_QuestGroup()
	{
		for (int i = 0; i < _netTriggers.Length; i++)
		{
			_netTriggers[i].Network_deactivateTrigger = false;
		}
		for (int j = 0; j < _creepSpawners.Length; j++)
		{
			_creepSpawners[j].Network_disableSpawner = false;
		}
	}

	public override bool Weaved()
	{
		return true;
	}
}
