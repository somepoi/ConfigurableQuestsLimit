using UnityEngine;

[CreateAssetMenu(fileName = "classTome_", menuName = "ScriptableObjects/Item/Consumables/Class Tome", order = 0)]
public class ScriptableClassTome : ScriptableConsumable
{
	[Header("Class Tome Consumable")]
	public ScriptablePlayerBaseClass _scriptableClass;

	public int _classTierIndex;

	public override bool CanUseConsumable(StatusEntity _statusEntity, bool _isButtonPrompt)
	{
		PlayerStats pStats = Player._mainPlayer._pStats;
		PlayerInventory pInventory = _statusEntity._isPlayer._pInventory;
		if (((bool)_scriptableQuest & _isButtonPrompt) && pInventory.Check_InventoryFull(_scriptableQuest._questItemRewards[0]._scriptItem, 2))
		{
			return false;
		}
		if (!((_classTierIndex <= 0) ? pStats.Check_RequestClass(_scriptableClass._className, _isButtonPrompt) : (pStats.Check_RequestTier(_scriptableClass._className, _classTierIndex, _isButtonPrompt) && (bool)pStats._class && pStats._class == _scriptableClass)))
		{
			return false;
		}
		return true;
	}

	public override void Use_Consumable(StatusEntity _statusEntity)
	{
		if (_statusEntity._isPlayer._playerZoneType != ZoneType.Safe)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Can only use in Safe Zone");
			return;
		}
		PlayerStats pStats = _statusEntity._isPlayer._pStats;
		PlayerInventory pInventory = _statusEntity._isPlayer._pInventory;
		if (!pStats._class || _classTierIndex > 0)
		{
			if ((bool)pStats._class && pStats._class != _scriptableClass)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Class Required - " + _scriptableClass._className);
			}
			else if (pStats._syncClassTier > 0 && _classTierIndex > 0)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Already assigned to a Subclass");
			}
			else if (!_scriptableQuest || !pInventory.Check_InventoryFull(_scriptableQuest._questItemRewards[0]._scriptItem, 1))
			{
				Apply_ClassTomeConsumable(pStats);
				QuestTrigger_OnUse();
			}
		}
	}

	private void Apply_ClassTomeConsumable(PlayerStats _pStats)
	{
		if (_pStats._player._playerZoneType == ZoneType.Safe && _pStats.isLocalPlayer)
		{
			if (_classTierIndex <= 0)
			{
				_pStats.Cmd_RequestClass(_scriptableClass._className);
			}
			else
			{
				_pStats.Cmd_RequestClassTier(_classTierIndex);
			}
		}
	}
}
