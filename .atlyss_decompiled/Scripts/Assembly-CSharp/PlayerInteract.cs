using System.Collections;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerInteract : NetworkBehaviour
{
	private Player _player;

	private PlayerMove _pMove;

	private PlayerCombat _pCombat;

	private PlayerCasting _pCasting;

	private PlayerInventory _pInventory;

	private PlayerVisual _pVisual;

	private PlayerSound _pSound;

	private PlayerTargeting _pTargeting;

	private PlayerQuesting _pQuest;

	public float _currentCommandBuffer;

	private bool _bufferCommandRequest;

	[Header("Interact Reticle")]
	[SerializeField]
	private Transform _interactReticleObject;

	[SerializeField]
	private GameObject _interactReticlePrefab;

	private Transform _interactableObj;

	private bool UnableToInteract
	{
		get
		{
			if (_player._currentPlayerAction != PlayerAction.IDLE || _player._currentPlayerCondition != PlayerCondition.ACTIVE)
			{
				return true;
			}
			if (_player._bufferingStatus || _player._inChat)
			{
				return true;
			}
			if (DialogManager._current._isDialogEnabled)
			{
				return true;
			}
			if (TabMenu._current._isOpen)
			{
				return true;
			}
			if (_pCombat._isChargingWeapon || _pTargeting._isTargetingEntity)
			{
				return true;
			}
			if (_pCombat._isBlocking || _pCombat._localBlockingInput)
			{
				return true;
			}
			if ((bool)_pCasting._currentCastSkill)
			{
				return true;
			}
			if (_pCasting._isCasting)
			{
				return true;
			}
			if (_player._pProfession._isProfessionAction)
			{
				return true;
			}
			if (_bufferCommandRequest)
			{
				return true;
			}
			if (GamepadInput.GetButton(GamepadButton.LeftShoulder))
			{
				return true;
			}
			if (GamepadInput.GetButton(GamepadButton.RightShoulder))
			{
				return true;
			}
			if (InputControlManager.current.IsDashInput)
			{
				return true;
			}
			return false;
		}
	}

	private void Awake()
	{
		_player = GetComponent<Player>();
		_pInventory = GetComponent<PlayerInventory>();
		_pMove = GetComponent<PlayerMove>();
		_pCombat = GetComponent<PlayerCombat>();
		_pCasting = GetComponent<PlayerCasting>();
		_pVisual = GetComponent<PlayerVisual>();
		_pSound = GetComponent<PlayerSound>();
		_pTargeting = GetComponent<PlayerTargeting>();
		_pQuest = GetComponent<PlayerQuesting>();
	}

	private void Update()
	{
		if (!base.isLocalPlayer)
		{
			return;
		}
		Handle_CommandBufferParams();
		if (_player._currentPlayerCondition != PlayerCondition.ACTIVE || _player._bufferingStatus)
		{
			_currentCommandBuffer = 4.5f;
			return;
		}
		if (_player._pProfession._isProfessionAction)
		{
			_currentCommandBuffer = 1.25f;
		}
		Handle_InteractControl();
		Handle_InteractReticleDisplay();
	}

	[ClientCallback]
	public bool Check_HasSoulPearl()
	{
		if (!NetworkClient.active)
		{
			return default;
		}
		for (int i = 0; i < Player._mainPlayer._pInventory._heldItems.Count; i++)
		{
			if (Player._mainPlayer._pInventory._heldItems[i]._itemName == "Soul Pearl")
			{
				return true;
			}
		}
		return false;
	}

	[ClientCallback]
	private void Handle_CommandBufferParams()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		_bufferCommandRequest = _currentCommandBuffer > 0f;
		if (!_player._bufferingStatus)
		{
			if (_currentCommandBuffer > 0f)
			{
				_currentCommandBuffer -= Time.deltaTime;
			}
			else
			{
				_currentCommandBuffer = 0f;
			}
			if (!_bufferCommandRequest && (bool)DialogManager._current._cachedNpc && !DialogManager._current._cachedNpc.Network_netSyncPlayerInteraction)
			{
				Cmd_SetInteraction(DialogManager._current._cachedNpc.netIdentity);
				_currentCommandBuffer = 1f;
			}
		}
	}

	[ClientCallback]
	private void Reset_Reticle()
	{
		if (NetworkClient.active)
		{
			if (!_interactReticleObject)
			{
				GameObject gameObject = Object.Instantiate(_interactReticlePrefab, base.transform.position, Quaternion.identity);
				_interactReticleObject = gameObject.transform;
			}
			if (_interactReticleObject.gameObject.activeInHierarchy)
			{
				_interactReticleObject.SetParent(base.transform);
				_interactReticleObject.localPosition = Vector3.zero;
				_interactReticleObject.gameObject.SetActive(value: false);
			}
		}
	}

	[ClientCallback]
	private void Handle_InteractReticleDisplay()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		if (!_interactReticleObject)
		{
			Reset_Reticle();
		}
		else if (_player._bufferingStatus || _player._currentPlayerCondition != PlayerCondition.ACTIVE || _player._currentGameCondition != GameCondition.IN_GAME)
		{
			Reset_Reticle();
		}
		else if ((UnableToInteract || !_interactableObj) && (bool)_interactReticleObject)
		{
			Reset_Reticle();
		}
		else
		{
			if (!_interactableObj)
			{
				return;
			}
			if (_interactableObj.TryGetComponent<ItemObject>(out var component) && (component._isPickedUp || (component._currencyDropAmount > 0 && (bool)component._local_taggedID)))
			{
				_interactReticleObject.gameObject.SetActive(value: false);
				return;
			}
			if (_interactableObj.TryGetComponent<ResourceEntity>(out var _) && _pMove._currentMovementAction != MovementAction.IDLE)
			{
				Disable_InteractPrompt();
				_currentCommandBuffer = 0.85f;
				return;
			}
			if (_interactReticleObject.transform.parent != null)
			{
				_pSound._aSrcInteractBlip.Play();
				_interactReticleObject.localScale = Vector3.one * 9.5f;
				_interactReticleObject.SetParent(null);
			}
			_interactReticleObject.gameObject.SetActive(value: true);
			_interactReticleObject.position = _interactableObj.transform.position + new Vector3(0f, 0.45f, 0f);
		}
	}

	private void Disable_InteractPrompt()
	{
		InGameUI._current.ContextPrompt(_isOpen: false, string.Empty);
		InGameUI._current.DisablePortalCaption();
		_interactableObj = null;
	}

	[ClientCallback]
	private void Handle_InteractControl()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		if (_pCasting._isCasting || _player._currentPlayerCondition != PlayerCondition.ACTIVE)
		{
			_currentCommandBuffer = 0.36f;
		}
		if (_player._pProfession._isProfessionAction && _player._pProfession._professionControlBuffer <= 0f)
		{
			if (_player._pProfession._scriptTool._scriptableProfession._professionName == "Fishing")
			{
				InGameUI._current.ContextPrompt(_isOpen: true, "REEL");
			}
			else
			{
				InGameUI._current.ContextPrompt(_isOpen: true, "CANCEL");
			}
		}
		else if (UnableToInteract)
		{
			if ((bool)_interactReticleObject)
			{
				_interactReticleObject.SetParent(base.transform);
			}
			InGameUI._current.ContextPrompt(_isOpen: false, string.Empty);
			InGameUI._current.DisablePortalCaption();
		}
		else
		{
			if (!_interactReticleObject)
			{
				return;
			}
			_interactReticleObject.localScale = Vector3.Lerp(_interactReticleObject.localScale, Vector3.one * 2.5f, 25.5f * Time.deltaTime);
			Collider[] array = new Collider[2];
			base.gameObject.scene.GetPhysicsScene().OverlapSphere(base.transform.position, 2.35f, array, CastLayers.InteractableMask, QueryTriggerInteraction.Collide);
			for (int i = 0; i < array.Length; i++)
			{
				Player component;
				ItemDropEntity_Chest component2;
				ItemObject component3;
				ResourceEntity component4;
				Portal component5;
				RecallPortal component6;
				NetTrigger component7;
				DialogTrigger component8;
				if (array[i] == null)
				{
					Disable_InteractPrompt();
				}
				else if (array[i].gameObject.transform.root.TryGetComponent<Player>(out component))
				{
					if ((!Player._mainPlayer._isHostPlayer || !(component.gameObject.scene != Player._mainPlayer.gameObject.scene)) && !(Player._mainPlayer.Network_playerMapInstance != component.Network_playerMapInstance) && Player._mainPlayer._playerZoneType != ZoneType.Arena && !(component == Player._mainPlayer) && component._pPvp._currentVersusFlag != VersusFlag.PVP && !component._bufferingStatus)
					{
						InteractQueue_RevivePlayer(component);
						break;
					}
				}
				else if (array[i].gameObject.TryGetComponent<ItemDropEntity_Chest>(out component2))
				{
					if (!component2._isChestOpen && !component2._isChestHidden)
					{
						InteractQueue_ItemChestEntity(component2);
						break;
					}
				}
				else if (array[i].gameObject.TryGetComponent<ItemObject>(out component3))
				{
					if ((bool)component3 && (!component3._local_taggedID || !(component3._local_taggedID != base.netIdentity)))
					{
						InteractQueue_ItemObject(component3);
						break;
					}
				}
				else if (array[i].gameObject.TryGetComponent<ResourceEntity>(out component4))
				{
					if (component4._isActive && _pMove._currentMovementAction == MovementAction.IDLE && _pInventory.Check_ItemQuantity(component4._scriptableProfession._professionTool) > 0)
					{
						InteractQueue_ResourceEntity(component4);
						break;
					}
				}
				else if (array[i].gameObject.TryGetComponent<Portal>(out component5))
				{
					if (_pMove._currentMovementAction == MovementAction.JUMP)
					{
						Disable_InteractPrompt();
					}
					else if (_pMove._currentMovementAction == MovementAction.DASH || _pMove._currentMovementAction == MovementAction.JUMPDASH)
					{
						Disable_InteractPrompt();
					}
					else if (_pMove._dashActionBuffer)
					{
						Disable_InteractPrompt();
					}
					else if (component5._isPortalOpen && !component5._netDisablePortal)
					{
						InteractQueue_Portal(component5);
						break;
					}
				}
				else if (array[i].gameObject.TryGetComponent<RecallPortal>(out component6))
				{
					if (_pMove._currentMovementAction == MovementAction.JUMP)
					{
						Disable_InteractPrompt();
					}
					else if (_pMove._currentMovementAction == MovementAction.DASH || _pMove._currentMovementAction == MovementAction.JUMPDASH)
					{
						Disable_InteractPrompt();
					}
					else if (_pMove._dashActionBuffer)
					{
						Disable_InteractPrompt();
					}
					else if (component6._isPortalOpen)
					{
						InteractQueue_RecallPortal(component6);
						break;
					}
				}
				else if (array[i].gameObject.TryGetComponent<NetTrigger>(out component7))
				{
					if (component7._requireInteractionKey && !component7._wasInteractedWith)
					{
						InteractQueue_NetTrigger(component7);
						break;
					}
				}
				else if (array[i].gameObject.TryGetComponent<DialogTrigger>(out component8))
				{
					if (_pMove._currentMovementAction == MovementAction.JUMP)
					{
						Disable_InteractPrompt();
					}
					else if (!(DialogManager._current._dialogBuffer > 0f) && (bool)component8._scriptDialogData)
					{
						InteractQueue_DialogTrigger(component8);
						break;
					}
				}
				else
				{
					if (array[i].gameObject.TryGetComponent<QuestTrigger>(out var component9))
					{
						if (_pMove._currentMovementAction == MovementAction.JUMP)
						{
							continue;
						}
						for (int j = 0; j < _pQuest._questProgressData.Count; j++)
						{
							if (!(_pQuest._questProgressData[j]._questTag != component9._scriptQuest._questName) && !_pQuest._questProgressData[j]._questComplete)
							{
								InteractQueue_QuestTrigger(component9);
							}
						}
						break;
					}
					if (array[i].gameObject.TryGetComponent<PushBlock>(out var component10))
					{
						if (_pMove._currentMovementAction == MovementAction.JUMP)
						{
							Disable_InteractPrompt();
						}
						else if (_pMove._currentMovementAction == MovementAction.LEDGEGRAB)
						{
							Disable_InteractPrompt();
						}
						else if (_pMove._currentMovementAction == MovementAction.CLIMB)
						{
							Disable_InteractPrompt();
						}
						else if (_pMove._airTime > 0.5f)
						{
							Disable_InteractPrompt();
						}
						else if (component10._isMoving)
						{
							Disable_InteractPrompt();
						}
						else if ((bool)component10.Network_parentPlayer)
						{
							Disable_InteractPrompt();
						}
						else if (component10._pushBuffer > 0f)
						{
							Disable_InteractPrompt();
						}
						else
						{
							InteractQueue_Pushblock(component10);
						}
					}
				}
			}
			if (_bufferCommandRequest)
			{
				InGameUI._current.ContextPrompt(_isOpen: false, string.Empty);
			}
		}
	}

	[ClientCallback]
	private void InteractQueue_RevivePlayer(Player _foundPlayer)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		_interactableObj = _foundPlayer.transform;
		InGameUI._current.ContextPrompt(_isOpen: true, "REVIVE");
		if (InputControlManager.current.IsInteractionInput && !(_currentCommandBuffer > 0f))
		{
			if (Check_HasSoulPearl())
			{
				ItemData itemData = Player._mainPlayer._pInventory.Fetch_HeldItem(GameManager._current._statLogics._soulPearl);
				Player._mainPlayer._statusEntity.Cmd_RevivePlayer(_foundPlayer._statusEntity);
				Player._mainPlayer._pInventory.Remove_Item(itemData, 1);
			}
			else
			{
				Player._mainPlayer._pCasting.Cmd_SetReviveEntity(_foundPlayer._statusEntity);
				Player._mainPlayer._pCasting.Client_SelectReviveSkill();
			}
			_interactableObj = null;
			_currentCommandBuffer = 2.45f;
		}
	}

	[ClientCallback]
	private void InteractQueue_ResourceEntity(ResourceEntity _foundResourceEntity)
	{
		if (NetworkClient.active)
		{
			_interactableObj = _foundResourceEntity.transform;
			InGameUI._current.ContextPrompt(_isOpen: true, _foundResourceEntity._scriptableProfession._interactionTag ?? "");
			if (InputControlManager.current.IsInteractionInput && _foundResourceEntity._scriptableProfession._professionTool.CanUseConsumable(_player._statusEntity, _isButtonPrompt: true))
			{
				_foundResourceEntity._scriptableProfession._professionTool.Use_Consumable(_player._statusEntity);
				_interactableObj = null;
				_currentCommandBuffer = 0.85f;
			}
		}
	}

	[ClientCallback]
	private void InteractQueue_DialogTrigger(DialogTrigger _foundDialogTrigger)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		_interactableObj = _foundDialogTrigger.transform;
		InGameUI._current.ContextPrompt(_isOpen: true, "INTERACT");
		if (InputControlManager.current.IsInteractionInput)
		{
			_foundDialogTrigger.BeginDialog(_foundDialogTrigger._scriptDialogData);
			if ((bool)_foundDialogTrigger.GetComponentInParent<NetNPC>())
			{
				_foundDialogTrigger.GetComponentInParent<NetNPC>()._localPlayerInteraction = Player._mainPlayer;
				Cmd_SetInteraction(_foundDialogTrigger.GetComponentInParent<NetworkIdentity>());
			}
			_interactableObj = null;
			_currentCommandBuffer = 0.85f;
		}
	}

	[ClientCallback]
	private void InteractQueue_QuestTrigger(QuestTrigger _foundQuestTrigger)
	{
		if (NetworkClient.active)
		{
			_interactableObj = _foundQuestTrigger.transform;
			InGameUI._current.ContextPrompt(_isOpen: true, "INTERACT");
			if (InputControlManager.current.IsInteractionInput)
			{
				Cmd_SetInteraction(_foundQuestTrigger.netIdentity);
				_interactableObj = null;
				_currentCommandBuffer = 0.85f;
			}
		}
	}

	[ClientCallback]
	private void InteractQueue_Pushblock(PushBlock _foundPushblock)
	{
		if (NetworkClient.active)
		{
			_interactableObj = _foundPushblock.transform;
			InGameUI._current.ContextPrompt(_isOpen: true, "HOLD");
			if (InputControlManager.current.IsInteractionInput)
			{
				_currentCommandBuffer = 1.5f;
				Cmd_SetPushblockParent(_foundPushblock);
				_pVisual.Send_CrossFadeAnim("pushblock_hold", 0.1f, 7, LatencyCheck.IGNORE_LATENCY);
			}
		}
	}

	[Command]
	private void Cmd_SetPushblockParent(PushBlock _foundPushblock)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_foundPushblock);
		SendCommandInternal("System.Void PlayerInteract::Cmd_SetPushblockParent(PushBlock)", -1812077629, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_SetInteraction(NetworkIdentity _netID)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkIdentity(_netID);
		SendCommandInternal("System.Void PlayerInteract::Cmd_SetInteraction(Mirror.NetworkIdentity)", 454620980, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_RemoveInteraction(NetworkIdentity _netID)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkIdentity(_netID);
		SendCommandInternal("System.Void PlayerInteract::Cmd_RemoveInteraction(Mirror.NetworkIdentity)", 1924129170, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	private void InteractQueue_NetTrigger(NetTrigger _foundTrigger)
	{
		if (NetworkClient.active)
		{
			_interactableObj = _foundTrigger.transform;
			InGameUI._current.ContextPrompt(_isOpen: true, "INTERACT");
			if (InputControlManager.current.IsInteractionInput)
			{
				_pVisual.Send_CrossFadeAnim("interact_button", 0.1f, 7, LatencyCheck.IGNORE_LATENCY);
				StartCoroutine(Queue_TriggerInteractionCommand(_foundTrigger));
				_interactableObj = null;
				_currentCommandBuffer = 0.85f;
			}
		}
	}

	private IEnumerator Queue_TriggerInteractionCommand(NetTrigger _foundTrigger)
	{
		yield return new WaitForSeconds(0.1f);
		Cmd_InteractWithTrigger(_foundTrigger.netIdentity);
	}

	[Command]
	private void Cmd_InteractWithTrigger(NetworkIdentity _triggerID)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkIdentity(_triggerID);
		SendCommandInternal("System.Void PlayerInteract::Cmd_InteractWithTrigger(Mirror.NetworkIdentity)", 1185694368, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	private void InteractQueue_Portal(Portal _foundPortal)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		InGameUI._current.PortalCaptionPrompt(_foundPortal._scenePortal._portalCaptionTitle ?? "");
		InGameUI._current.ContextPrompt(_isOpen: true, "TELEPORT");
		if (InputControlManager.current.IsInteractionInput)
		{
			Cmd_InteractWithPortal(_foundPortal, ZoneDifficulty.NORMAL);
			InGameUI._current.DisablePortalCaption();
			_pSound._aSrcGeneral.PlayOneShot(_pSound._portalInteract, 0.85f);
			_pVisual.Send_CrossFadeAnim("Idle", 0f, 0, LatencyCheck.IGNORE_LATENCY);
			_pMove._freezeMovementBuffer = 1.25f;
			if (_foundPortal._scenePortal._subScene != string.Empty)
			{
				_currentCommandBuffer = 2.35f;
			}
			else
			{
				_currentCommandBuffer = 0.65f;
			}
			_interactableObj = null;
		}
	}

	[ClientCallback]
	private void InteractQueue_RecallPortal(RecallPortal _foundPortal)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		InGameUI._current.PortalCaptionPrompt(_foundPortal._scenePortal._portalCaptionTitle ?? "");
		InGameUI._current.ContextPrompt(_isOpen: true, "TELEPORT");
		if (InputControlManager.current.IsInteractionInput)
		{
			_foundPortal.Init_PickupItem(base.netIdentity);
			InGameUI._current.DisablePortalCaption();
			_pSound._aSrcGeneral.PlayOneShot(_pSound._portalInteract, 0.85f);
			_pVisual.Send_CrossFadeAnim("Idle", 0f, 0, LatencyCheck.IGNORE_LATENCY);
			if (_foundPortal._scenePortal._subScene != string.Empty)
			{
				_currentCommandBuffer = 2.35f;
			}
			else
			{
				_currentCommandBuffer = 0.65f;
			}
			_interactableObj = null;
		}
	}

	[Command]
	public void Cmd_InteractWithPortal(Portal _portal, ZoneDifficulty _setDifficulty)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_portal);
		GeneratedNetworkCode._Write_ZoneDifficulty(writer, _setDifficulty);
		SendCommandInternal("System.Void PlayerInteract::Cmd_InteractWithPortal(Portal,ZoneDifficulty)", -641452126, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	private void InteractQueue_ItemChestEntity(ItemDropEntity_Chest _foundChestEntity)
	{
		if (NetworkClient.active)
		{
			_interactableObj = _foundChestEntity.transform;
			InGameUI._current.ContextPrompt(_isOpen: true, "OPEN");
			if (InputControlManager.current.IsInteractionInput)
			{
				_pVisual.Send_CrossFadeAnim("interact_button", 0.1f, 7, LatencyCheck.IGNORE_LATENCY);
				StartCoroutine(Queue_TriggerInteractionCommand(_foundChestEntity));
				_interactableObj = null;
				_currentCommandBuffer = 0.85f;
			}
		}
	}

	private IEnumerator Queue_TriggerInteractionCommand(ItemDropEntity_Chest _foundChestEntity)
	{
		yield return new WaitForSeconds(0.1f);
		Cmd_OpenChest(_foundChestEntity);
	}

	[Command]
	private void Cmd_OpenChest(ItemDropEntity_Chest _foundChestEntity)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_foundChestEntity);
		SendCommandInternal("System.Void PlayerInteract::Cmd_OpenChest(ItemDropEntity_Chest)", -910568719, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void Rpc_OpenChest(ItemDropEntity_Chest _foundChestEntity)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_foundChestEntity);
		SendRPCInternal("System.Void PlayerInteract::Rpc_OpenChest(ItemDropEntity_Chest)", 167026588, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	private void InteractQueue_ItemObject(ItemObject _foundItemObj)
	{
		if (!NetworkClient.active || !_foundItemObj._canPickUp || _foundItemObj._localPickup || _foundItemObj._isPickedUp)
		{
			return;
		}
		Net_ItemObject net_ItemObject = null;
		if ((bool)_foundItemObj.GetComponent<Net_ItemObject>())
		{
			net_ItemObject = _foundItemObj.GetComponent<Net_ItemObject>();
			if (!net_ItemObject._canPickUp || net_ItemObject._isPickedUp)
			{
				return;
			}
		}
		_interactableObj = _foundItemObj.transform;
		if (_foundItemObj._currencyDropAmount > 0 && (bool)_foundItemObj._local_taggedID)
		{
			return;
		}
		if (!_foundItemObj._isPickedUp)
		{
			InGameUI._current.ContextPrompt(_isOpen: true, "PICK UP");
		}
		else
		{
			InGameUI._current.ContextPrompt(_isOpen: false, string.Empty);
		}
		if (!InputControlManager.current.IsInteractionInput)
		{
			return;
		}
		if ((bool)_foundItemObj._foundItem)
		{
			if (_pInventory.Check_InventoryFull(_foundItemObj._foundItem, _foundItemObj._local_itemData._quantity))
			{
				return;
			}
			if ((bool)_foundItemObj._foundItem._scriptableQuest && !_pQuest.Check_HasQuest(_foundItemObj._foundItem._scriptableQuest))
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Not on Quest");
				return;
			}
		}
		if ((bool)net_ItemObject)
		{
			Cmd_InteractNetItemObject(net_ItemObject, base.netIdentity);
			_foundItemObj._localPickup = true;
		}
		else
		{
			_foundItemObj.Init_PickupItem(base.netIdentity);
			_foundItemObj._localPickup = true;
		}
		Init_PickupTutorial();
		_pSound._aSrcItemPickup.Play();
		_interactableObj = null;
		_currentCommandBuffer = 0.1f;
		static void Init_PickupTutorial()
		{
			string tutorialTag = "tut_pickup";
			string message = "[" + InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._itemMenuHotkey.ToString()) + "] Inventory Menu Hotkey\nYou can hold the interact key to auto pick up items that are nearby.";
			if (InputControlManager.current._setGamepadType != GamepadType.Keyboard)
			{
				tutorialTag = "tut_pickup_gpad";
				message = "You can hold [X] to auto pick up items that are nearby.";
			}
			TutorialManager._current.Init_TutorialMessage(message, tutorialTag);
		}
	}

	[Command]
	public void Cmd_InteractNetItemObject(Net_ItemObject _netItemObj, NetworkIdentity _netID)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_netItemObj);
		writer.WriteNetworkIdentity(_netID);
		SendCommandInternal("System.Void PlayerInteract::Cmd_InteractNetItemObject(Net_ItemObject,Mirror.NetworkIdentity)", 265545635, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Cmd_SetPushblockParent__PushBlock(PushBlock _foundPushblock)
	{
		if ((bool)_foundPushblock)
		{
			_foundPushblock.Network_pushBuffer = _foundPushblock._timeBetweenPushes;
			_foundPushblock.Network_parentPlayer = _player;
			_foundPushblock.netIdentity.AssignClientAuthority(base.connectionToClient);
		}
	}

	protected static void InvokeUserCode_Cmd_SetPushblockParent__PushBlock(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_SetPushblockParent called on client.");
		}
		else
		{
			((PlayerInteract)obj).UserCode_Cmd_SetPushblockParent__PushBlock(reader.ReadNetworkBehaviour<PushBlock>());
		}
	}

	protected void UserCode_Cmd_SetInteraction__NetworkIdentity(NetworkIdentity _netID)
	{
		if ((bool)_netID.gameObject.GetComponent<NetNPC>())
		{
			_netID.gameObject.GetComponent<NetNPC>().Network_netSyncPlayerInteraction = GetComponent<Player>();
		}
		if ((bool)_netID.gameObject.GetComponent<QuestTrigger>())
		{
			_netID.gameObject.GetComponent<QuestTrigger>().Init_PickupItem(base.netIdentity);
		}
	}

	protected static void InvokeUserCode_Cmd_SetInteraction__NetworkIdentity(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_SetInteraction called on client.");
		}
		else
		{
			((PlayerInteract)obj).UserCode_Cmd_SetInteraction__NetworkIdentity(reader.ReadNetworkIdentity());
		}
	}

	protected void UserCode_Cmd_RemoveInteraction__NetworkIdentity(NetworkIdentity _netID)
	{
		if ((bool)_netID.gameObject.GetComponent<NetNPC>())
		{
			NetNPC component = _netID.gameObject.GetComponent<NetNPC>();
			if (component.Network_netSyncPlayerInteraction == GetComponent<Player>())
			{
				component.Network_netSyncPlayerInteraction = null;
			}
		}
	}

	protected static void InvokeUserCode_Cmd_RemoveInteraction__NetworkIdentity(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_RemoveInteraction called on client.");
		}
		else
		{
			((PlayerInteract)obj).UserCode_Cmd_RemoveInteraction__NetworkIdentity(reader.ReadNetworkIdentity());
		}
	}

	protected void UserCode_Cmd_InteractWithTrigger__NetworkIdentity(NetworkIdentity _triggerID)
	{
		NetTrigger component = _triggerID.GetComponent<NetTrigger>();
		ChatBehaviour component2 = GetComponent<ChatBehaviour>();
		if (component._wasInteractedWith)
		{
			return;
		}
		if (component._requireDungeonKey)
		{
			if (_player.Network_playerMapInstance._patternInstance._dungeonKeysAquired <= 0)
			{
				component2.Target_RecieveMessage("The door is locked. It requires a Dungeon Key.");
				return;
			}
			PatternInstanceManager patternInstance = _player.Network_playerMapInstance._patternInstance;
			patternInstance.Network_dungeonKeysAquired = patternInstance._dungeonKeysAquired - 1;
		}
		component._lastNetID = GetComponent<NetworkIdentity>();
		component.Init_EnableTrigger();
		_interactableObj = null;
		_currentCommandBuffer = 0.85f;
	}

	protected static void InvokeUserCode_Cmd_InteractWithTrigger__NetworkIdentity(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_InteractWithTrigger called on client.");
		}
		else
		{
			((PlayerInteract)obj).UserCode_Cmd_InteractWithTrigger__NetworkIdentity(reader.ReadNetworkIdentity());
		}
	}

	protected void UserCode_Cmd_InteractWithPortal__Portal__ZoneDifficulty(Portal _portal, ZoneDifficulty _setDifficulty)
	{
		if ((bool)_portal && (bool)base.netIdentity)
		{
			if ((bool)_pCombat)
			{
				_pCombat.Network_currentSheathCondition = WeaponSheatheCondition.PUT_AWAY;
			}
			_portal.Server_InteractPortal(base.netIdentity, _setDifficulty);
			_currentCommandBuffer = 2.35f;
		}
	}

	protected static void InvokeUserCode_Cmd_InteractWithPortal__Portal__ZoneDifficulty(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_InteractWithPortal called on client.");
		}
		else
		{
			((PlayerInteract)obj).UserCode_Cmd_InteractWithPortal__Portal__ZoneDifficulty(reader.ReadNetworkBehaviour<Portal>(), GeneratedNetworkCode._Read_ZoneDifficulty(reader));
		}
	}

	protected void UserCode_Cmd_OpenChest__ItemDropEntity_Chest(ItemDropEntity_Chest _foundChestEntity)
	{
		if (!_foundChestEntity._isChestOpen)
		{
			_foundChestEntity.Network_isChestOpen = true;
			Rpc_OpenChest(_foundChestEntity);
		}
	}

	protected static void InvokeUserCode_Cmd_OpenChest__ItemDropEntity_Chest(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_OpenChest called on client.");
		}
		else
		{
			((PlayerInteract)obj).UserCode_Cmd_OpenChest__ItemDropEntity_Chest(reader.ReadNetworkBehaviour<ItemDropEntity_Chest>());
		}
	}

	protected void UserCode_Rpc_OpenChest__ItemDropEntity_Chest(ItemDropEntity_Chest _foundChestEntity)
	{
		if ((bool)_foundChestEntity && (!Player._mainPlayer._isHostPlayer || !(Player._mainPlayer.gameObject.scene != _foundChestEntity.gameObject.scene)) && Player._mainPlayer._playerZoneType == _player._playerZoneType && !(Vector3.Distance(Player._mainPlayer.transform.position, _foundChestEntity.transform.position) > 385f))
		{
			_foundChestEntity.Interactable_DropItems();
		}
	}

	protected static void InvokeUserCode_Rpc_OpenChest__ItemDropEntity_Chest(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_OpenChest called on server.");
		}
		else
		{
			((PlayerInteract)obj).UserCode_Rpc_OpenChest__ItemDropEntity_Chest(reader.ReadNetworkBehaviour<ItemDropEntity_Chest>());
		}
	}

	protected void UserCode_Cmd_InteractNetItemObject__Net_ItemObject__NetworkIdentity(Net_ItemObject _netItemObj, NetworkIdentity _netID)
	{
		_netItemObj.Network_taggedID = _netID;
		_netItemObj.Network_canPickUp = false;
		_netItemObj.Network_isPickedUp = true;
		_netItemObj.GetComponent<ItemObject>().UnspawnObject();
		_netItemObj.Rpc_NetItemObjectPickup(_netID);
	}

	protected static void InvokeUserCode_Cmd_InteractNetItemObject__Net_ItemObject__NetworkIdentity(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_InteractNetItemObject called on client.");
		}
		else
		{
			((PlayerInteract)obj).UserCode_Cmd_InteractNetItemObject__Net_ItemObject__NetworkIdentity(reader.ReadNetworkBehaviour<Net_ItemObject>(), reader.ReadNetworkIdentity());
		}
	}

	static PlayerInteract()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInteract), "System.Void PlayerInteract::Cmd_SetPushblockParent(PushBlock)", InvokeUserCode_Cmd_SetPushblockParent__PushBlock, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInteract), "System.Void PlayerInteract::Cmd_SetInteraction(Mirror.NetworkIdentity)", InvokeUserCode_Cmd_SetInteraction__NetworkIdentity, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInteract), "System.Void PlayerInteract::Cmd_RemoveInteraction(Mirror.NetworkIdentity)", InvokeUserCode_Cmd_RemoveInteraction__NetworkIdentity, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInteract), "System.Void PlayerInteract::Cmd_InteractWithTrigger(Mirror.NetworkIdentity)", InvokeUserCode_Cmd_InteractWithTrigger__NetworkIdentity, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInteract), "System.Void PlayerInteract::Cmd_InteractWithPortal(Portal,ZoneDifficulty)", InvokeUserCode_Cmd_InteractWithPortal__Portal__ZoneDifficulty, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInteract), "System.Void PlayerInteract::Cmd_OpenChest(ItemDropEntity_Chest)", InvokeUserCode_Cmd_OpenChest__ItemDropEntity_Chest, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInteract), "System.Void PlayerInteract::Cmd_InteractNetItemObject(Net_ItemObject,Mirror.NetworkIdentity)", InvokeUserCode_Cmd_InteractNetItemObject__Net_ItemObject__NetworkIdentity, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInteract), "System.Void PlayerInteract::Rpc_OpenChest(ItemDropEntity_Chest)", InvokeUserCode_Rpc_OpenChest__ItemDropEntity_Chest);
	}
}
