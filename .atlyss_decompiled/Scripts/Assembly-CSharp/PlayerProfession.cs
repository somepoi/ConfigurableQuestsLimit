using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mirror;
using UnityEngine;

public class PlayerProfession : NetworkBehaviour
{
	[CompilerGenerated]
	private sealed class _003CProfession_Action_003Ed__14 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public PlayerProfession _003C_003E4__this;

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
		public _003CProfession_Action_003Ed__14(int _003C_003E1__state)
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
			PlayerProfession playerProfession = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				if (playerProfession._initProfessionAction)
				{
					return false;
				}
				if (!playerProfession._scriptTool)
				{
					return false;
				}
				if (!playerProfession._setResourceEntity)
				{
					playerProfession._isProfessionAction = false;
					return false;
				}
				playerProfession._setResourceEntity._resourceAnimator.CrossFade("Empty", 0f, 1);
				playerProfession._setResourceEntity._resourceAnimator.CrossFade("Interation_init", 0f, 1);
				if ((bool)playerProfession._scriptTool)
				{
					playerProfession._player._pVisual.Send_CrossFadeAnim(playerProfession._scriptTool._playerAnimTag ?? "", 0.1f, 11, LatencyCheck.IGNORE_LATENCY);
				}
				playerProfession._professionControlBuffer = 0.6f;
				playerProfession._initProfessionAction = true;
				_003C_003E2__current = new WaitForSeconds(3.2f);
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				playerProfession._rollingForResource = true;
				goto IL_01cd;
			case 2:
			{
				_003C_003E1__state = -1;
				float num2 = UnityEngine.Random.Range(0f, 1f);
				if (!playerProfession._scriptTool)
				{
					playerProfession._isProfessionAction = false;
					return false;
				}
				if (playerProfession._scriptTool._toolEffiency >= num2)
				{
					if (playerProfession._scriptTool._scriptableProfession._professionName == "Fishing")
					{
						playerProfession._player._pVisual.Send_CrossFadeAnim("Fish_Bite", 0.1f, 11, LatencyCheck.IGNORE_LATENCY);
						playerProfession._setResourceEntity._resourceAnimator.CrossFade("Interaction_loop", 0f, 1);
						playerProfession._hasBite = true;
					}
					playerProfession._rollingForResource = false;
				}
				goto IL_01cd;
			}
			case 3:
				_003C_003E1__state = -1;
				ErrorPromptTextManager.current.Init_ErrorPrompt("Got Away");
				playerProfession._player._pVisual.Send_CrossFadeAnim("Empty", 0.1f, 11, LatencyCheck.IGNORE_LATENCY);
				playerProfession._setResourceEntity._resourceAnimator.CrossFade("Empty", 0f, 1);
				playerProfession._hasBite = false;
				playerProfession._isProfessionAction = false;
				break;
			case 4:
				{
					_003C_003E1__state = -1;
					if ((bool)playerProfession._setResourceEntity && playerProfession._setResourceEntity._isActive)
					{
						if ((bool)playerProfession._scriptTool)
						{
							playerProfession._scriptTool.Use_Consumable(playerProfession._player._statusEntity);
							playerProfession._initProfessionAction = false;
						}
					}
					else
					{
						playerProfession._isProfessionAction = false;
					}
					break;
				}
				IL_01cd:
				if (playerProfession._rollingForResource)
				{
					_003C_003E2__current = new WaitForSeconds(UnityEngine.Random.Range(2f, 3.5f));
					_003C_003E1__state = 2;
					return true;
				}
				if (playerProfession._scriptTool._scriptableProfession._professionName == "Fishing")
				{
					_003C_003E2__current = new WaitForSeconds(UnityEngine.Random.Range(1f, 1.32785f));
					_003C_003E1__state = 3;
					return true;
				}
				playerProfession.Init_FetchProfessionResource();
				_003C_003E2__current = new WaitForSeconds(1.32f);
				_003C_003E1__state = 4;
				return true;
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

	[Header("Player Profession")]
	public ResourceEntity _setResourceEntity;

	public ScriptableProfessionTool _scriptTool;

	public float _professionControlBuffer;

	[Header("Profession Behavior")]
	public bool _isProfessionAction;

	public bool _initProfessionAction;

	public bool _hasBite;

	private bool _rollingForResource;

	private void Awake()
	{
		_player = GetComponent<Player>();
	}

	private void Update()
	{
		Handle_ProfessionConditions();
		Handle_ProfessionAction();
	}

	[ClientCallback]
	private void Handle_ProfessionConditions()
	{
		if (NetworkClient.active)
		{
			if (_player._bufferingStatus || _player._currentPlayerCondition != PlayerCondition.ACTIVE)
			{
				_setResourceEntity = null;
			}
			if ((bool)_setResourceEntity && !_setResourceEntity._isActive)
			{
				_setResourceEntity = null;
			}
			if (_professionControlBuffer > 0f)
			{
				_professionControlBuffer -= Time.deltaTime;
			}
			else
			{
				_professionControlBuffer = 0f;
			}
		}
	}

	[ClientCallback]
	private void Handle_ProfessionAction()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		if (!_isProfessionAction)
		{
			_scriptTool = null;
			_rollingForResource = false;
			_initProfessionAction = false;
			_hasBite = false;
			return;
		}
		if (TabMenu._current._isOpen)
		{
			TabMenu._current.Close_TabMenu();
		}
		if (!_setResourceEntity)
		{
			_isProfessionAction = false;
			return;
		}
		_player._pMove._lockControlBuffer = 0.2f;
		_player._pCombat._lockControlBuffer = 0.2f;
		_player._pCasting._lockControlBuffer = 0.2f;
		Vector3 forward = _setResourceEntity._resourceVisualContainer.transform.position - base.transform.position;
		forward.y = 0f;
		Quaternion b = Quaternion.LookRotation(forward);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, 6.45f * Time.deltaTime);
		if (_isProfessionAction && !_initProfessionAction)
		{
			StartCoroutine(Profession_Action());
		}
		if (IsCancelInput() && _professionControlBuffer <= 0f && !_player._inChat)
		{
			Cancel_ProfessionAction();
		}
		if (_initProfessionAction && CheckToCancel())
		{
			Cancel_ProfessionAction();
			StopAllCoroutines();
		}
		bool CheckToCancel()
		{
			if (!_scriptTool)
			{
				return true;
			}
			if (!_setResourceEntity._isActive)
			{
				return true;
			}
			if (_player._isHurt || _player._currentHurtBuffer > 0f)
			{
				return true;
			}
			if (_player._statusEntity._generalHitBuffer > 0f)
			{
				return true;
			}
			if (_player._statusEntity._isDamageBuffer)
			{
				return true;
			}
			return false;
		}
		bool IsCancelInput()
		{
			if (_player._chatBehaviour._focusedInChat)
			{
				return false;
			}
			if (InputControlManager.current.IsInteractionInputDown)
			{
				return true;
			}
			if (Input.GetKeyDown(InputControlManager.current._attack))
			{
				return true;
			}
			if (Input.GetKeyDown(InputControlManager.current._block))
			{
				return true;
			}
			if (Input.GetKeyDown(InputControlManager.current._jump))
			{
				return true;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_South))
			{
				return true;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.LeftTrigger))
			{
				return true;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.RightTrigger))
			{
				return true;
			}
			return false;
		}
	}

	public void Cancel_ProfessionAction()
	{
		StopAllCoroutines();
		_setResourceEntity._resourceAnimator.CrossFade("Empty", 0f, 1);
		_player._pVisual.Send_CrossFadeAnim("Empty", 0.1f, 11, LatencyCheck.CHECK_LATENCY);
		if (_isProfessionAction)
		{
			if (_scriptTool._scriptableProfession._professionName == "Fishing" && _hasBite)
			{
				Init_FetchProfessionResource();
			}
			_player._pSound._aSrcGeneral.PlayOneShot(_scriptTool._toolAudio_end, 1f);
		}
		_isProfessionAction = false;
	}

	private void Init_FetchProfessionResource()
	{
		ScriptableItem scriptableItem = _setResourceEntity._resourceEntityLoot.Roll_ForItem(_guaranteeItem: true);
		if (!scriptableItem)
		{
			if (UnityEngine.Random.Range(0f, 1f) <= 0.6f)
			{
				scriptableItem = _setResourceEntity._resourceEntityLoot._itemDrops[UnityEngine.Random.Range(0, _setResourceEntity._resourceEntityLoot._itemDrops.Length)]._item;
			}
			return;
		}
		if (_player._pInventory.Check_InventoryFull(scriptableItem, 1))
		{
			_isProfessionAction = false;
			Cancel_ProfessionAction();
			return;
		}
		ItemData itemData = new ItemData
		{
			_itemName = scriptableItem._itemName,
			_maxQuantity = scriptableItem._maxStackAmount,
			_quantity = 1,
			_isEquipped = false,
			_damageTypeOverride = DamageType.Strength,
			_useDamageTypeOverride = false,
			_modifierID = 0,
			_isAltWeapon = false,
			_slotNumber = 0
		};
		_player._chatBehaviour.Client_RecieveTriggerMessage("Got " + scriptableItem._itemName + "!");
		_player._pInventory.Add_Item(itemData, _autoStack: true);
		_player._pVisual.Cmd_ShowItemEmote(itemData._itemName);
		_player._pStats.Cmd_GainProfessionExp(_setResourceEntity, scriptableItem._professionExperience);
	}

	[IteratorStateMachine(typeof(_003CProfession_Action_003Ed__14))]
	[ClientCallback]
	private IEnumerator Profession_Action()
	{
		if (!NetworkClient.active)
		{
			return null;
		}
		return new _003CProfession_Action_003Ed__14(0)
		{
			_003C_003E4__this = this
		};
	}

	public override bool Weaved()
	{
		return true;
	}
}
