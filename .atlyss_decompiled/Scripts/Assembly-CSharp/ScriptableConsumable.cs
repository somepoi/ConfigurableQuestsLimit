using UnityEngine;

public abstract class ScriptableConsumable : ScriptableItem
{
	[Header("Scriptable Consumable")]
	public int _levelRequirement;

	public ScriptablePlayerBaseClass _baseClassRequirement;

	public float _consumableCooldown = 1.5f;

	[Header("Flags")]
	public bool _useOnlyInTown;

	public bool _unperishable;

	[Header("Consumable Object Spawn")]
	public GameObject _consumableObject;

	public abstract bool CanUseConsumable(StatusEntity _statusEntity, bool _isButtonPrompt);

	public abstract void Use_Consumable(StatusEntity _statusEntity);

	public void QuestTrigger_OnUse()
	{
		if ((bool)Player._mainPlayer && Player._mainPlayer.isLocalPlayer && !string.IsNullOrEmpty(_questTriggerTag) && (bool)_scriptableQuest)
		{
			PlayerQuesting pQuest = Player._mainPlayer._pQuest;
			if (pQuest._scriptableQuests.Contains(_scriptableQuest))
			{
				pQuest.Apply_QuestTriggerProgress(_questTriggerTag);
			}
		}
	}

	public virtual void OnValidate()
	{
		_itemType = ItemType.CONSUMABLE;
		if (_levelRequirement < 0)
		{
			_levelRequirement = 0;
		}
		if (_consumableCooldown < 0f)
		{
			_consumableCooldown = 0f;
		}
	}
}
