using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerQuesting : NetworkBehaviour
{
	[CompilerGenerated]
	private sealed class _003CInit_QuestShareRoll_003Ed__38 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public PlayerQuesting _003C_003E4__this;

		public ScriptableQuest _scriptQuest;

		private int _003Ci_003E5__2;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CInit_QuestShareRoll_003Ed__38(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			PlayerQuesting playerQuesting = _003C_003E4__this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				_003C_003E1__state = -1;
				goto IL_0131;
			}
			_003C_003E1__state = -1;
			_003Ci_003E5__2 = 0;
			goto IL_0141;
			IL_0131:
			_003Ci_003E5__2++;
			goto IL_0141;
			IL_0141:
			if (_003Ci_003E5__2 < playerQuesting._player.Network_partyObject._syncPartyPeers.Count)
			{
				if ((bool)playerQuesting._player.Network_partyObject._syncPartyPeers[_003Ci_003E5__2] && !(playerQuesting._player.Network_partyObject._syncPartyPeers[_003Ci_003E5__2] == playerQuesting._player))
				{
					playerQuesting._player._chatBehaviour.Target_RecieveMessage("Sharing " + _scriptQuest._questName + " to " + playerQuesting._player.Network_partyObject._syncPartyPeers[_003Ci_003E5__2]._nickname + "...");
					playerQuesting._player.Network_partyObject._syncPartyPeers[_003Ci_003E5__2]._pQuest.Target_RecieveQuestShare(_scriptQuest._questName);
					_003C_003E2__current = new WaitForSeconds(0.2f);
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_0131;
			}
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	private Player _player;

	private PlayerInventory _pInventory;

	private PlayerStats _pStats;

	private PlayerSound _pSound;

	private ChatBehaviour _cB;

	[Header("Player Questing")]
	public int _questLogLimit = 5;

	private float _shareQuestBuffer;

	[Space]
	public List<QuestProgressStruct> _questProgressData = new List<QuestProgressStruct>();

	public List<ScriptableQuest> _scriptableQuests = new List<ScriptableQuest>();

	public Dictionary<string, ScriptableQuest> _finishedQuests = new Dictionary<string, ScriptableQuest>(0);

	public readonly SyncList<string> _syncQuestTags = new SyncList<string>();

	private bool _isAutoCompletingQuest;

	private void Awake()
	{
		_player = GetComponent<Player>();
		_pStats = GetComponent<PlayerStats>();
		_pInventory = GetComponent<PlayerInventory>();
		_pSound = GetComponent<PlayerSound>();
		_cB = GetComponent<ChatBehaviour>();
	}

	private void Start()
	{
		_syncQuestTags.Callback += OnQuestTagsUpdated;
		for (int i = 0; i < _syncQuestTags.Count; i++)
		{
			OnQuestTagsUpdated(SyncList<string>.Operation.OP_ADD, i, "", _syncQuestTags[i]);
		}
	}

	private void OnQuestTagsUpdated(SyncList<string>.Operation op, int _i, string _oldString, string _newString)
	{
		switch (op)
		{
		}
	}

	private void Update()
	{
		if (_shareQuestBuffer > 0f)
		{
			_shareQuestBuffer -= Time.deltaTime;
		}
		else
		{
			_shareQuestBuffer = 0f;
		}
	}

	public override void OnStartAuthority()
	{
		base.OnStartAuthority();
		StartCoroutine(Init_QuestProgressLoad());
	}

	private IEnumerator Init_QuestProgressLoad()
	{
		yield return new WaitForSeconds(3.5f);
		Load_QuestProgressProfile();
	}

	public bool Check_HasQuest(ScriptableQuest _scriptQuest)
	{
		for (int i = 0; i < _scriptableQuests.Count; i++)
		{
			if (_scriptQuest == _scriptableQuests[i])
			{
				return true;
			}
		}
		return false;
	}

	public bool Check_HasQuestTag(string _questTag)
	{
		for (int i = 0; i < _syncQuestTags.Count; i++)
		{
			if (_questTag == _syncQuestTags[i])
			{
				return true;
			}
		}
		return false;
	}

	public bool Check_IfCannotAcceptQuest(ScriptableQuest _scriptableQuest)
	{
		if (_scriptableQuest._questLevel > Player._mainPlayer._pStats._currentLevel)
		{
			return true;
		}
		if (_scriptableQuest._requireNoviceClass && Player._mainPlayer._pStats._class != null)
		{
			return true;
		}
		if ((bool)_scriptableQuest._baseClassRequirement && Player._mainPlayer._pStats._class != _scriptableQuest._baseClassRequirement && Player._mainPlayer._pStats._class != null)
		{
			return true;
		}
		if ((bool)_scriptableQuest._raceRequirement && Player._mainPlayer._pVisual._playerRaceModel._scriptablePlayerRace != _scriptableQuest._raceRequirement)
		{
			return true;
		}
		for (int i = 0; i < _scriptableQuest._preQuestRequirements.Length; i++)
		{
			if (!Player._mainPlayer._pQuest._finishedQuests.ContainsKey(_scriptableQuest._preQuestRequirements[i]._questName))
			{
				return true;
			}
		}
		if ((bool)_scriptableQuest._skillToHide)
		{
			for (int j = 0; j < Player._mainPlayer._pCasting._skillLibrary.Count; j++)
			{
				if (_scriptableQuest._skillToHide == Player._mainPlayer._pCasting._skillLibrary[j]._scriptSkill)
				{
					return true;
				}
			}
		}
		return false;
	}

	[ClientCallback]
	private void Load_QuestProgressProfile()
	{
		if (!NetworkClient.active || !base.isLocalPlayer)
		{
			return;
		}
		ProfileDataManager current = ProfileDataManager._current;
		QuestProgressProfile questProgressProfile = ((current._characterFile._questProgressProfile != null) ? current._characterFile._questProgressProfile : new QuestProgressProfile
		{
			_questProgressStructs = new QuestProgressStruct[0],
			_finishedQuests = new string[0]
		});
		for (int i = 0; i < questProgressProfile._finishedQuests.Length; i++)
		{
			ScriptableQuest scriptableQuest = GameManager._current.Locate_Quest(questProgressProfile._finishedQuests[i]);
			if ((bool)scriptableQuest && !_finishedQuests.ContainsKey(questProgressProfile._finishedQuests[i]))
			{
				_finishedQuests.Add(questProgressProfile._finishedQuests[i], scriptableQuest);
			}
		}
		if (questProgressProfile._questProgressStructs != null)
		{
			for (int j = 0; j < questProgressProfile._questProgressStructs.Length; j++)
			{
				ScriptableQuest scriptableQuest2 = GameManager._current.Locate_Quest(questProgressProfile._questProgressStructs[j]._questTag);
				if ((bool)scriptableQuest2 && questProgressProfile._questProgressStructs[j]._creepKillProgressValues.Length == scriptableQuest2._questObjective._questCreepRequirements.Length && questProgressProfile._questProgressStructs[j]._itemProgressValues.Length == scriptableQuest2._questObjective._questItemRequirements.Length && questProgressProfile._questProgressStructs[j]._triggerProgressValues.Length == scriptableQuest2._questObjective._questTriggerRequirements.Length)
				{
					if (base.isServer)
					{
						_syncQuestTags.Add(scriptableQuest2._questName);
					}
					_questProgressData.Add(questProgressProfile._questProgressStructs[j]);
					_scriptableQuests.Add(scriptableQuest2);
					QuestTrackerManager._current.Refresh_ProgressElements();
				}
			}
		}
		Init_QueryQuestProgressDataToServer();
	}

	[ClientCallback]
	private void Init_QueryQuestProgressDataToServer()
	{
		if (NetworkClient.active && base.isClientOnly)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < _questProgressData.Count; i++)
			{
				list.Add(_questProgressData[i]._questTag);
			}
			Cmd_QueryServerQuestData(list.ToArray(), _finishedQuests.Keys.ToArray());
		}
	}

	[Command]
	private void Cmd_QueryServerQuestData(string[] _acquiredQuestTags, string[] _completedQuests)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_System_002EString_005B_005D(writer, _acquiredQuestTags);
		GeneratedNetworkCode._Write_System_002EString_005B_005D(writer, _completedQuests);
		SendCommandInternal("System.Void PlayerQuesting::Cmd_QueryServerQuestData(System.String[],System.String[])", -984297906, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Accept_Quest(ScriptableQuest _newQuest)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		if (_questProgressData.Count >= _questLogLimit)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Quest log full");
			return;
		}
		if (Check_HasQuest(_newQuest))
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Already on this quest");
			return;
		}
		if ((bool)_newQuest._questObjectiveItem._scriptItem && _pInventory.Check_InventoryFull(_newQuest._questObjectiveItem._scriptItem, _newQuest._questObjectiveItem._itemQuantity))
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Inventory full to accept quest");
			return;
		}
		int[] itemProgressValues = new int[_newQuest._questObjective._questItemRequirements.Length];
		int[] creepKillProgressValues = new int[_newQuest._questObjective._questCreepRequirements.Length];
		int[] triggerProgressValues = new int[_newQuest._questObjective._questTriggerRequirements.Length];
		QuestProgressStruct item = new QuestProgressStruct
		{
			_questTag = _newQuest._questName,
			_itemProgressValues = itemProgressValues,
			_creepKillProgressValues = creepKillProgressValues,
			_triggerProgressValues = triggerProgressValues,
			_questComplete = false
		};
		_questProgressData.Add(item);
		_scriptableQuests.Add(_newQuest);
		if ((bool)_newQuest._questObjectiveItem._scriptItem)
		{
			ItemData itemData = new ItemData
			{
				_itemName = _newQuest._questObjectiveItem._scriptItem._itemName,
				_quantity = _newQuest._questObjectiveItem._itemQuantity,
				_maxQuantity = _newQuest._questObjectiveItem._scriptItem._maxStackAmount,
				_modifierID = 0,
				_slotNumber = 0,
				_isEquipped = false,
				_isAltWeapon = false,
				_damageTypeOverride = DamageType.Strength,
				_useDamageTypeOverride = false
			};
			_pInventory.Add_Item(itemData, _autoStack: true);
			_cB.New_ChatMessage("Retrieved Quest Objective Item: " + _newQuest._questObjectiveItem._scriptItem._itemName + ".");
		}
		_pSound._aSrcPlayerUI.PlayOneShot(_pSound._questAcceptSfx, 1f);
		_cB.Init_GameLogicMessage("Accepted Quest: " + _newQuest._questName + ".");
		if (DialogManager._current._isDialogEnabled && (bool)DialogManager._current._scriptableDialog && DialogManager._current._scriptableDialog._questAcceptResponses != null && DialogManager._current._scriptableDialog._questAcceptResponses.Length != 0 && UnityEngine.Random.Range(0, 2) == 1)
		{
			DialogManager._current.Start_QuickSentence(DialogManager._current._scriptableDialog._questAcceptResponses[UnityEngine.Random.Range(0, DialogManager._current._scriptableDialog._questAcceptResponses.Length)]);
		}
		QuestTrackerManager._current.Refresh_ProgressElements();
		ProfileDataManager._current.Save_ProfileData();
		Init_QueryQuestProgressDataToServer();
		if (_newQuest._questObjective._questItemRequirements != null && _newQuest._questObjective._questItemRequirements.Length != 0)
		{
			for (int i = 0; i < _newQuest._questObjective._questItemRequirements.Length; i++)
			{
				Apply_QuestItemProgress(_newQuest._questObjective._questItemRequirements[i]._questItem);
			}
		}
	}

	[ClientCallback]
	public void Apply_QuestProgressNote(string _string, int _index)
	{
		if (NetworkClient.active && (bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && !DialogManager._current._isDialogEnabled && !_questProgressData[_index]._questComplete)
		{
			QuestTrackerManager._current.Init_QuestNote(_string);
		}
	}

	[ClientCallback]
	public void Apply_QuestItemProgress(ScriptableItem _scriptItem)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		for (int i = 0; i < _questProgressData.Count; i++)
		{
			ScriptableQuest scriptableQuest = GameManager._current.Locate_Quest(_questProgressData[i]._questTag);
			if ((bool)scriptableQuest)
			{
				Iterate_ScriptableQuests(scriptableQuest, i);
			}
		}
		void Iterate_ScriptableQuests(ScriptableQuest _scriptQuest, int _index)
		{
			int[] array = new int[_scriptQuest._questObjective._questItemRequirements.Length];
			bool flag = true;
			for (int j = 0; j < _scriptQuest._questObjective._questItemRequirements.Length; j++)
			{
				QuestItemRequirement questItemRequirement = _scriptQuest._questObjective._questItemRequirements[j];
				flag = ((_questProgressData[_index]._itemProgressValues[j] < questItemRequirement._itemsNeeded) ? true : false);
				for (int k = 0; k < _pInventory._heldItems.Count; k++)
				{
					if (_pInventory._heldItems[k]._itemName == questItemRequirement._questItem._itemName)
					{
						array[j] += _pInventory._heldItems[k]._quantity;
						if (array[j] > questItemRequirement._itemsNeeded)
						{
							array[j] = questItemRequirement._itemsNeeded;
						}
					}
				}
				if ((bool)_scriptItem && _scriptItem._itemName != questItemRequirement._questItem._itemName)
				{
					flag = false;
				}
				if (_player._bufferingStatus)
				{
					flag = false;
				}
				if (flag)
				{
					Apply_QuestProgressNote($"{questItemRequirement._questItem._itemName}: ({array[j]} / {questItemRequirement._itemsNeeded})", _index);
				}
			}
			_questProgressData[_index]._itemProgressValues = array;
			Check_QuestCompletion(_index);
		}
	}

	[TargetRpc]
	public void Target_Query_CreepKillProgress(string _creepTag)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_creepTag);
		SendTargetRPCInternal(null, "System.Void PlayerQuesting::Target_Query_CreepKillProgress(System.String)", -1701191915, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void Target_QueryQuestTriggerProgress(string _triggerTag)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_triggerTag);
		SendTargetRPCInternal(null, "System.Void PlayerQuesting::Target_QueryQuestTriggerProgress(System.String)", -487611937, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Apply_QuestTriggerProgress(string _triggerTag)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		bool _sendNote = true;
		for (int i = 0; i < _questProgressData.Count; i++)
		{
			ScriptableQuest scriptableQuest = GameManager._current.Locate_Quest(_questProgressData[i]._questTag);
			if ((bool)scriptableQuest)
			{
				Iterate_ScriptableQuest(scriptableQuest, i);
			}
		}
		void Iterate_ScriptableQuest(ScriptableQuest _scriptQuest, int _index)
		{
			for (int j = 0; j < _scriptQuest._questObjective._questTriggerRequirements.Length; j++)
			{
				if (!(_scriptQuest._questObjective._questTriggerRequirements[j]._questTriggerTag != _triggerTag))
				{
					QuestTriggerRequirement questTriggerRequirement = _scriptQuest._questObjective._questTriggerRequirements[j];
					int[] triggerProgressValues = _questProgressData[_index]._triggerProgressValues;
					int num = triggerProgressValues[j] + 1;
					if (num > questTriggerRequirement._triggerEmitsNeeded)
					{
						num = questTriggerRequirement._triggerEmitsNeeded;
						_sendNote = false;
					}
					if (_player._bufferingStatus)
					{
						_sendNote = false;
					}
					triggerProgressValues[j] = num;
					if (_sendNote)
					{
						Apply_QuestProgressNote($"{questTriggerRequirement._prefix} {questTriggerRequirement._suffix}: ({triggerProgressValues[j]} / {questTriggerRequirement._triggerEmitsNeeded})", _index);
					}
					_questProgressData[_index]._triggerProgressValues = triggerProgressValues;
					Check_QuestCompletion(_index);
				}
			}
		}
	}

	[ClientCallback]
	private void Check_QuestCompletion(int _index)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		ScriptableQuest scriptableQuest = GameManager._current.Locate_Quest(_questProgressData[_index]._questTag);
		if ((bool)scriptableQuest)
		{
			int[] array = new int[scriptableQuest._questObjective._questItemRequirements.Length];
			int[] array2 = new int[scriptableQuest._questObjective._questCreepRequirements.Length];
			int[] array3 = new int[scriptableQuest._questObjective._questTriggerRequirements.Length];
			for (int i = 0; i < scriptableQuest._questObjective._questItemRequirements.Length; i++)
			{
				QuestItemRequirement questItemRequirement = scriptableQuest._questObjective._questItemRequirements[i];
				array[i] = questItemRequirement._itemsNeeded;
			}
			for (int j = 0; j < scriptableQuest._questObjective._questCreepRequirements.Length; j++)
			{
				QuestCreepRequirement questCreepRequirement = scriptableQuest._questObjective._questCreepRequirements[j];
				array2[j] = questCreepRequirement._creepsKilled;
			}
			for (int k = 0; k < scriptableQuest._questObjective._questTriggerRequirements.Length; k++)
			{
				QuestTriggerRequirement questTriggerRequirement = scriptableQuest._questObjective._questTriggerRequirements[k];
				array3[k] = questTriggerRequirement._triggerEmitsNeeded;
			}
			bool flag = Enumerable.SequenceEqual(array, _questProgressData[_index]._itemProgressValues) && Enumerable.SequenceEqual(array2, _questProgressData[_index]._creepKillProgressValues) && Enumerable.SequenceEqual(array3, _questProgressData[_index]._triggerProgressValues);
			_questProgressData[_index]._questComplete = flag;
			if (flag && scriptableQuest._autoFinishQuest && !_isAutoCompletingQuest)
			{
				StartCoroutine(Buffer_AutoComplete(_index));
			}
			QuestTrackerManager._current.Update_ProgressElements();
		}
	}

	private IEnumerator Buffer_AutoComplete(int _index)
	{
		_isAutoCompletingQuest = true;
		yield return new WaitForSeconds(0.2f);
		Client_CompleteQuest(_index);
		_isAutoCompletingQuest = false;
	}

	[ClientCallback]
	public void Client_CompleteQuest(int _index)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		if (_questProgressData.Count <= 0 || !_questProgressData[_index]._questComplete)
		{
			return;
		}
		ScriptableQuest _scriptQuest = GameManager._current.Locate_Quest(_questProgressData[_index]._questTag);
		if (!_scriptQuest)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < _scriptQuest._questItemRewards.Length; i++)
		{
			if (_pInventory.Check_InventoryFull(_scriptQuest._questItemRewards[i]._scriptItem, _scriptQuest._questItemRewards[i]._itemQuantity))
			{
				return;
			}
			bool flag = false;
			for (int j = 0; j < _pInventory._heldItems.Count; j++)
			{
				if (_pInventory._heldItems[j]._itemName == _scriptQuest._questItemRewards[i]._scriptItem._itemName && _scriptQuest._questItemRewards[i]._itemQuantity + _pInventory._heldItems[j]._quantity < _pInventory._heldItems[j]._maxQuantity)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				switch (_scriptQuest._questItemRewards[i]._scriptItem._itemType)
				{
				case ItemType.GEAR:
					num++;
					break;
				case ItemType.CONSUMABLE:
					num2++;
					break;
				case ItemType.TRADE:
					num3++;
					break;
				}
			}
		}
		if (num + _pInventory._usedEquipmentSlots > _pInventory._maxInventoryStruct._maxEquipmentSlots)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Equipment Inventory Full");
			return;
		}
		if (num2 + _pInventory._usedConsumableItemSlots > _pInventory._maxInventoryStruct._maxConsumableItemSlots)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Consumable Inventory Full");
			return;
		}
		if (num3 + _pInventory._usedTradeItemSlots > _pInventory._maxInventoryStruct._maxTradeItemSlots)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Trade Item Inventory Full");
			return;
		}
		Give_QuestItems();
		Give_QuestRewards();
		Finish_Quest();
		void Finish_Quest()
		{
			if (0 == 0 && DialogManager._current._isDialogEnabled)
			{
				DialogManager._current.Start_QuickSentence(DialogManager._current._scriptableDialog._questCompleteResponses[UnityEngine.Random.Range(0, DialogManager._current._scriptableDialog._questCompleteResponses.Length)]);
			}
			if (!base.isServer && !_finishedQuests.ContainsKey(_scriptQuest._questName))
			{
				_finishedQuests.Add(_scriptQuest._questName, _scriptQuest);
			}
			if (!_finishedQuests.ContainsKey(_scriptQuest._questName))
			{
				_finishedQuests.Add(_scriptQuest._questName, _scriptQuest);
			}
			_questProgressData.RemoveAt(_index);
			_scriptableQuests.RemoveAt(_index);
			if ((bool)SteamAchievementManager.current && _scriptQuest._questName == "A Warm Welcome")
			{
				SteamAchievementManager.current.Init_EarnAchievement("ATLYSS_ACHIEVEMENT_00");
			}
			_cB.Init_GameLogicMessage(_scriptQuest._questName + " Completed!");
			_pSound._aSrcPlayerUI.PlayOneShot(_pSound._questTurnInSfx, 1f);
			ProfileDataManager._current.Save_ProfileData();
			QuestTrackerManager._current.Refresh_ProgressElements();
			Init_QueryQuestProgressDataToServer();
		}
		void Give_QuestItems()
		{
			for (int k = 0; k < _scriptQuest._questObjective._questItemRequirements.Length; k++)
			{
				QuestItemRequirement qIR = _scriptQuest._questObjective._questItemRequirements[k];
				Iterate_QuestItemRequirement(qIR);
			}
		}
		void Give_QuestRewards()
		{
			for (int k = 0; k < _scriptQuest._questItemRewards.Length; k++)
			{
				int modifierID = 0;
				if ((bool)_scriptQuest._questItemRewards[k]._scriptableStatModifier)
				{
					modifierID = _scriptQuest._questItemRewards[k]._scriptableStatModifier._modifierID;
				}
				ItemData itemData = new ItemData
				{
					_itemName = _scriptQuest._questItemRewards[k]._scriptItem._itemName,
					_quantity = _scriptQuest._questItemRewards[k]._itemQuantity,
					_maxQuantity = _scriptQuest._questItemRewards[k]._scriptItem._maxStackAmount,
					_modifierID = modifierID,
					_slotNumber = 0,
					_isEquipped = false,
					_isAltWeapon = false,
					_damageTypeOverride = DamageType.Strength,
					_useDamageTypeOverride = false
				};
				_pInventory.Add_Item(itemData, _autoStack: true);
			}
			Cmd_InitServersideQuestRewards(_scriptQuest._questName);
		}
		void Iterate_QuestItemRequirement(QuestItemRequirement _qIR)
		{
			int num4 = 0;
			for (int k = 0; k < _pInventory._heldItems.Count; k++)
			{
				if (!(_pInventory._heldItems[k]._itemName != _qIR._questItem._itemName))
				{
					if (num4 < _qIR._itemsNeeded)
					{
						int quantity = _pInventory._heldItems[k]._quantity;
						if (quantity >= _qIR._itemsNeeded)
						{
							quantity -= _qIR._itemsNeeded;
							num4 = _qIR._itemsNeeded;
						}
						else
						{
							num4 += quantity;
							quantity -= quantity;
						}
						if (quantity <= 0)
						{
							if (_pInventory._heldItems[k]._isEquipped)
							{
								_player._pEquipment.Init_UnEquipItem(_pInventory._heldItems[k]);
								_pInventory.Remove_Item(_pInventory._heldItems[k], 0);
							}
							else
							{
								_pInventory.Remove_Item(_pInventory._heldItems[k], 0);
							}
						}
						else
						{
							_pInventory._heldItems[k]._quantity = quantity;
						}
					}
					_cB.Init_GameLogicMessage($"Turned in x{num4} {_qIR._questItem._itemName}.");
				}
			}
		}
	}

	[Command]
	private void Cmd_InitServersideQuestRewards(string _questTag)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_questTag);
		SendCommandInternal("System.Void PlayerQuesting::Cmd_InitServersideQuestRewards(System.String)", -1907687189, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Abandon_Quest(int _index)
	{
		if (NetworkClient.active && _questProgressData.Count > 0)
		{
			_cB.Init_GameLogicMessage("Abandoned Quest: " + _questProgressData[_index]._questTag + ".");
			_questProgressData.RemoveAt(_index);
			_scriptableQuests.RemoveAt(_index);
			ProfileDataManager._current.Save_ProfileData();
			QuestTrackerManager._current.Refresh_ProgressElements();
			Init_QueryQuestProgressDataToServer();
			StartCoroutine(Init_ItemIteration());
		}
	}

	private IEnumerator Init_ItemIteration()
	{
		yield return new WaitForSeconds(0.08f);
		_pInventory.Apply_ItemIteration();
	}

	[ClientCallback]
	public void Init_ShareQuest(ScriptableQuest _scriptQuest)
	{
		if (NetworkClient.active && !(_shareQuestBuffer > 0f))
		{
			if (!_player.Network_partyObject)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Not in a party");
				return;
			}
			if (_player != _player.Network_partyObject.Network_partyLeaderPlayer)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Not the party leader");
				return;
			}
			Cmd_ShareQuest(_scriptQuest._questName);
			_shareQuestBuffer = 1.5f;
		}
	}

	[Command]
	private void Cmd_ShareQuest(string _questTag)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_questTag);
		SendCommandInternal("System.Void PlayerQuesting::Cmd_ShareQuest(System.String)", -1685021420, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[IteratorStateMachine(typeof(_003CInit_QuestShareRoll_003Ed__38))]
	[ServerCallback]
	private IEnumerator Init_QuestShareRoll(ScriptableQuest _scriptQuest)
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		return new _003CInit_QuestShareRoll_003Ed__38(0)
		{
			_003C_003E4__this = this,
			_scriptQuest = _scriptQuest
		};
	}

	[TargetRpc]
	private void Target_RecieveQuestShare(string _questTag)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_questTag);
		SendTargetRPCInternal(null, "System.Void PlayerQuesting::Target_RecieveQuestShare(System.String)", -980125934, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	public PlayerQuesting()
	{
		InitSyncObject(_syncQuestTags);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Cmd_QueryServerQuestData__String_005B_005D__String_005B_005D(string[] _acquiredQuestTags, string[] _completedQuests)
	{
		_scriptableQuests.Clear();
		_finishedQuests.Clear();
		_syncQuestTags.Clear();
		for (int i = 0; i < _acquiredQuestTags.Length; i++)
		{
			ScriptableQuest scriptableQuest = GameManager._current.Locate_Quest(_acquiredQuestTags[i]);
			if ((bool)scriptableQuest)
			{
				_syncQuestTags.Add(_acquiredQuestTags[i]);
				_scriptableQuests.Add(scriptableQuest);
			}
		}
		for (int j = 0; j < _completedQuests.Length; j++)
		{
			ScriptableQuest scriptableQuest2 = GameManager._current.Locate_Quest(_completedQuests[j]);
			if ((bool)scriptableQuest2)
			{
				_finishedQuests.Add(_completedQuests[j], scriptableQuest2);
			}
		}
	}

	protected static void InvokeUserCode_Cmd_QueryServerQuestData__String_005B_005D__String_005B_005D(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command Cmd_QueryServerQuestData called on client.");
		}
		else
		{
			((PlayerQuesting)obj).UserCode_Cmd_QueryServerQuestData__String_005B_005D__String_005B_005D(GeneratedNetworkCode._Read_System_002EString_005B_005D(reader), GeneratedNetworkCode._Read_System_002EString_005B_005D(reader));
		}
	}

	protected void UserCode_Target_Query_CreepKillProgress__String(string _creepTag)
	{
		ScriptableCreep _scriptCreep = GameManager._current.Locate_Creep(_creepTag);
		if (!_scriptCreep)
		{
			return;
		}
		int[] _newCreepKillArray = new int[0];
		bool _sendNote = true;
		for (int i = 0; i < _questProgressData.Count; i++)
		{
			ScriptableQuest scriptableQuest = GameManager._current.Locate_Quest(_questProgressData[i]._questTag);
			if ((bool)scriptableQuest)
			{
				Iterate_ScriptableQuest(scriptableQuest, i);
			}
		}
		void Iterate_ScriptableQuest(ScriptableQuest _scriptQuest, int _index)
		{
			_newCreepKillArray = new int[_scriptQuest._questObjective._questCreepRequirements.Length];
			for (int j = 0; j < _questProgressData[_index]._creepKillProgressValues.Length; j++)
			{
				_newCreepKillArray[j] = _questProgressData[_index]._creepKillProgressValues[j];
			}
			for (int k = 0; k < _scriptQuest._questObjective._questCreepRequirements.Length; k++)
			{
				if (!(_scriptQuest._questObjective._questCreepRequirements[k]._questCreep != _scriptCreep))
				{
					QuestCreepRequirement questCreepRequirement = _scriptQuest._questObjective._questCreepRequirements[k];
					if (_questProgressData[_index]._creepKillProgressValues[k] >= questCreepRequirement._creepsKilled)
					{
						_sendNote = false;
					}
					int num = _questProgressData[_index]._creepKillProgressValues[k] + 1;
					if (num > questCreepRequirement._creepsKilled)
					{
						num = questCreepRequirement._creepsKilled;
					}
					_newCreepKillArray[k] = num;
					if (_player._bufferingStatus)
					{
						_sendNote = false;
					}
					if (_sendNote)
					{
						Apply_QuestProgressNote($"{questCreepRequirement._questCreep._creepName} slain: ({num} / {questCreepRequirement._creepsKilled})", _index);
					}
				}
			}
			_questProgressData[_index]._creepKillProgressValues = _newCreepKillArray;
			Check_QuestCompletion(_index);
		}
	}

	protected static void InvokeUserCode_Target_Query_CreepKillProgress__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("TargetRPC Target_Query_CreepKillProgress called on server.");
		}
		else
		{
			((PlayerQuesting)obj).UserCode_Target_Query_CreepKillProgress__String(reader.ReadString());
		}
	}

	protected void UserCode_Target_QueryQuestTriggerProgress__String(string _triggerTag)
	{
		Apply_QuestTriggerProgress(_triggerTag);
	}

	protected static void InvokeUserCode_Target_QueryQuestTriggerProgress__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("TargetRPC Target_QueryQuestTriggerProgress called on server.");
		}
		else
		{
			((PlayerQuesting)obj).UserCode_Target_QueryQuestTriggerProgress__String(reader.ReadString());
		}
	}

	protected void UserCode_Cmd_InitServersideQuestRewards__String(string _questTag)
	{
		ScriptableQuest scriptableQuest = GameManager._current.Locate_Quest(_questTag);
		if ((bool)scriptableQuest)
		{
			int expGain = (int)((float)(int)GameManager._current._statLogics._experienceCurve.Evaluate(scriptableQuest._questLevel) * scriptableQuest._questExperiencePercentage);
			_pStats.GainExp(expGain, _pStats._currentLevel);
			_pInventory.Add_Currency(scriptableQuest._questCurrencyReward);
		}
	}

	protected static void InvokeUserCode_Cmd_InitServersideQuestRewards__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command Cmd_InitServersideQuestRewards called on client.");
		}
		else
		{
			((PlayerQuesting)obj).UserCode_Cmd_InitServersideQuestRewards__String(reader.ReadString());
		}
	}

	protected void UserCode_Cmd_ShareQuest__String(string _questTag)
	{
		if ((bool)_player.Network_partyObject && (bool)_player.Network_partyObject.Network_partyLeaderPlayer && !(_shareQuestBuffer > 0f))
		{
			ScriptableQuest scriptableQuest = GameManager._current.Locate_Quest(_questTag);
			if ((bool)scriptableQuest)
			{
				StartCoroutine(Init_QuestShareRoll(scriptableQuest));
				_shareQuestBuffer = 2.5f;
			}
		}
	}

	protected static void InvokeUserCode_Cmd_ShareQuest__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command Cmd_ShareQuest called on client.");
		}
		else
		{
			((PlayerQuesting)obj).UserCode_Cmd_ShareQuest__String(reader.ReadString());
		}
	}

	protected void UserCode_Target_RecieveQuestShare__String(string _questTag)
	{
		if (!_player.Network_partyObject || !_player.Network_partyObject.Network_partyLeaderPlayer)
		{
			return;
		}
		ScriptableQuest scriptableQuest = GameManager._current.Locate_Quest(_questTag);
		if ((bool)scriptableQuest)
		{
			if (scriptableQuest._questType == QuestType.SINGLE && _finishedQuests.ContainsKey(scriptableQuest._questName))
			{
				_player._chatBehaviour.New_ChatMessage("<color=red>" + _player.Network_partyObject.Network_partyLeaderPlayer._nickname + " attempted to share " + _questTag + ", but you have already completed it.</color>");
			}
			else if (Check_HasQuest(scriptableQuest))
			{
				_player._chatBehaviour.New_ChatMessage("<color=red>" + _player.Network_partyObject.Network_partyLeaderPlayer._nickname + " attempted to share " + _questTag + ", but you are on this quest.</color>");
			}
			else if (Check_IfCannotAcceptQuest(scriptableQuest))
			{
				_player._chatBehaviour.New_ChatMessage("<color=red>" + _player.Network_partyObject.Network_partyLeaderPlayer._nickname + " attempted to share " + _questTag + ", but you do not meet the requirements.</color>");
			}
			else
			{
				_player._chatBehaviour.New_ChatMessage("<color=cyan>" + _player.Network_partyObject.Network_partyLeaderPlayer._nickname + " shared " + _questTag + " to you.</color>");
				Accept_Quest(scriptableQuest);
			}
		}
	}

	protected static void InvokeUserCode_Target_RecieveQuestShare__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("TargetRPC Target_RecieveQuestShare called on server.");
		}
		else
		{
			((PlayerQuesting)obj).UserCode_Target_RecieveQuestShare__String(reader.ReadString());
		}
	}

	static PlayerQuesting()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerQuesting), "System.Void PlayerQuesting::Cmd_QueryServerQuestData(System.String[],System.String[])", InvokeUserCode_Cmd_QueryServerQuestData__String_005B_005D__String_005B_005D, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerQuesting), "System.Void PlayerQuesting::Cmd_InitServersideQuestRewards(System.String)", InvokeUserCode_Cmd_InitServersideQuestRewards__String, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerQuesting), "System.Void PlayerQuesting::Cmd_ShareQuest(System.String)", InvokeUserCode_Cmd_ShareQuest__String, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerQuesting), "System.Void PlayerQuesting::Target_Query_CreepKillProgress(System.String)", InvokeUserCode_Target_Query_CreepKillProgress__String);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerQuesting), "System.Void PlayerQuesting::Target_QueryQuestTriggerProgress(System.String)", InvokeUserCode_Target_QueryQuestTriggerProgress__String);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerQuesting), "System.Void PlayerQuesting::Target_RecieveQuestShare(System.String)", InvokeUserCode_Target_RecieveQuestShare__String);
	}
}
