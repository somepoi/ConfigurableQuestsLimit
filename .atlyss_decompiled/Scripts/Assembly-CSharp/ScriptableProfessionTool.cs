using UnityEngine;

[CreateAssetMenu(fileName = "TOOL_", menuName = "ScriptableObjects/Item/Consumables/Tool", order = 0)]
public class ScriptableProfessionTool : ScriptableConsumable
{
	[Header("Tool Consumable")]
	public ScriptableProfession _scriptableProfession;

	public int _requiredProfessionLevel;

	[Range(0f, 1f)]
	public float _toolEffiency = 0.35f;

	public string _playerAnimTag = "";

	[Header("Audio")]
	public AudioClip _toolAudio_end;

	public override bool CanUseConsumable(StatusEntity _statusEntity, bool _isButtonPrompt)
	{
		Player isPlayer = _statusEntity._isPlayer;
		PlayerMove pMove = isPlayer._pMove;
		PlayerProfession pProfession = isPlayer._pProfession;
		PlayerWater componentInChildren = isPlayer.GetComponentInChildren<PlayerWater>();
		if (pMove._currentMovementAction != MovementAction.IDLE || isPlayer._currentPlayerAction != PlayerAction.IDLE || pProfession._professionControlBuffer > 0f)
		{
			Init_NotReadyPrompt();
			return false;
		}
		if (pProfession._initProfessionAction)
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Busy");
			}
			return false;
		}
		if (!pProfession._setResourceEntity || ((bool)pProfession._setResourceEntity && pProfession._setResourceEntity._scriptableProfession != _scriptableProfession))
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Requires " + _scriptableProfession._professionName + " Node");
			}
			return false;
		}
		if (componentInChildren._isInWater)
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Submerged");
			}
			return false;
		}
		if ((bool)pProfession._setResourceEntity)
		{
			int index = 0;
			for (int i = 0; i < GameManager._current._statLogics._scriptableProfessions.Length; i++)
			{
				if (_scriptableProfession == GameManager._current._statLogics._scriptableProfessions[i])
				{
					index = i;
					break;
				}
			}
			if (pProfession._setResourceEntity._resourceEntityLevel > isPlayer._pStats._playerProfessions[index]._professionLvl)
			{
				if (_isButtonPrompt)
				{
					ErrorPromptTextManager.current.Init_ErrorPrompt($"Requires Lv-{pProfession._setResourceEntity._resourceEntityLevel} {_scriptableProfession._professionName}");
				}
				return false;
			}
			if (!pProfession._setResourceEntity._isActive)
			{
				if (_isButtonPrompt)
				{
					ErrorPromptTextManager.current.Init_ErrorPrompt("Empty Node");
				}
				return false;
			}
		}
		if (pProfession._hasBite)
		{
			Init_NotReadyPrompt();
			return false;
		}
		return true;
		void Init_NotReadyPrompt()
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Not Ready");
			}
		}
	}

	public override void Use_Consumable(StatusEntity _statusEntity)
	{
		if (_statusEntity.isLocalPlayer)
		{
			PlayerProfession pProfession = _statusEntity._isPlayer._pProfession;
			if ((bool)pProfession._setResourceEntity && !(pProfession._setResourceEntity._scriptableProfession != _scriptableProfession))
			{
				pProfession._initProfessionAction = false;
				pProfession._scriptTool = this;
				pProfession._isProfessionAction = true;
			}
		}
	}

	public override void OnValidate()
	{
		base.OnValidate();
		if (_requiredProfessionLevel < 0)
		{
			_requiredProfessionLevel = 0;
		}
		_itemSortTag = SortTag.TOOL;
	}
}
