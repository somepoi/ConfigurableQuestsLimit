using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarManager : MonoBehaviour
{
	public static ActionBarManager _current;

	private Player _player;

	private PlayerCasting _pCast;

	private PlayerCombat _pCombat;

	private PlayerInventory _pInventory;

	private PlayerMove _pMove;

	private StatusEntity _statusEntity;

	[Header("Actionbar Manager")]
	public ActionSlot[] _actionSlots = new ActionSlot[6];

	public RectTransform[] _actionSlotTransforms = new RectTransform[6];

	[SerializeField]
	private CanvasGroup[] _actionSlotGlowGroups = new CanvasGroup[6];

	[SerializeField]
	private RotateObject[] _actionSlotGlowRotators = new RotateObject[6];

	[Space]
	[SerializeField]
	private GameObject _castBarObject;

	[SerializeField]
	private Image _castBarImage;

	[SerializeField]
	private Text _castBarTag;

	[SerializeField]
	private Image _leftShoulderIco;

	[SerializeField]
	private Image _rightShoulderIco;

	public Sprite _emptySkillIcon;

	public Sprite _gamepadInputIco_west;

	public Sprite _gamepadInputIco_north;

	public Sprite _gamepadInputIco_east;

	public Color _defaultIconColor = Color.white;

	public Color _inactiveIconColor = Color.grey;

	private float castBarMaxFill;

	[Header("Recall Action Slot")]
	[SerializeField]
	private MenuElement _recallIcoElement;

	[SerializeField]
	private Image _gamepadRecallHotkeyIco;

	[SerializeField]
	private Text _recallHotkeyText;

	[Header("Tooltip Config")]
	[SerializeField]
	private Vector3 _skillToolTipAnchor = new Vector3(0f, 160f, 0f);

	public int _currentHighlightedSlotIndex;

	public bool _isHighlightingSlot;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _aSrc;

	[SerializeField]
	private AudioSource _aSrc_gamepadShoulderInit;

	[SerializeField]
	private AudioClip _actionSlotPress;

	[SerializeField]
	private AudioClip _actionSlotCancel;

	private void Awake()
	{
		_current = this;
	}

	private void Update()
	{
		if ((bool)Player._mainPlayer)
		{
			_player = Player._mainPlayer;
			_pCast = _player._pCasting;
			_pCombat = _player._pCombat;
			_pMove = _player._pMove;
			_statusEntity = _player._statusEntity;
			_pInventory = _player._pInventory;
			Handle_CastBar();
			Handle_Actionkeys();
			Handle_ActionSlotCooldownDisplay();
		}
	}

	public void Glow_ActionBarIcon(int _index)
	{
		_actionSlotTransforms[_index].localScale = new Vector2(0.35f, 0.35f);
		_actionSlotGlowRotators[_index].Alter_RotationValues(Vector3.forward * 35f);
		_actionSlotGlowGroups[_index].alpha = 1f;
	}

	private void Handle_Actionkeys()
	{
		_leftShoulderIco.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard && !TabMenu._current._isOpen;
		_rightShoulderIco.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard && !TabMenu._current._isOpen;
		_gamepadRecallHotkeyIco.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		_recallHotkeyText.enabled = InputControlManager.current._setGamepadType == GamepadType.Keyboard;
		_recallHotkeyText.text = InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._recall.ToString()) ?? "";
		_gamepadRecallHotkeyIco.color = Color.white;
		bool flag = GamepadInput.GetButton(GamepadButton.LeftShoulder) || GamepadInput.GetButton(GamepadButton.RightShoulder);
		_recallIcoElement.isEnabled = _player._mapName != "Sanctum" && !flag && !_pCast._currentCastSkill && _player._currentPlayerCondition == PlayerCondition.ACTIVE && !TabMenu._current._isOpen;
		bool flag2 = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		bool flag3 = !TabMenu._current._isOpen && !SettingsManager._current._isOpen && !DialogManager._current._isDialogEnabled;
		if ((GamepadInput.GetButtonDown(GamepadButton.LeftShoulder) || GamepadInput.GetButtonDown(GamepadButton.RightShoulder)) && (flag3 & flag2) && Player._mainPlayer._currentGameCondition == GameCondition.IN_GAME)
		{
			_aSrc_gamepadShoulderInit.Play();
		}
		if (!flag2)
		{
			for (int i = 0; i < _actionSlots.Length; i++)
			{
				_actionSlots[i]._gamepadDisplayIcon.enabled = false;
			}
			_actionSlotTransforms[1].anchoredPosition = new Vector2(105f, -20f);
			_actionSlotTransforms[4].anchoredPosition = new Vector2(315f, -20f);
			if (Input.GetKeyDown(InputControlManager.current.actionBar_0))
			{
				OnActionkeyPress(0);
			}
			else if (Input.GetKeyDown(InputControlManager.current.actionBar_1))
			{
				OnActionkeyPress(1);
			}
			else if (Input.GetKeyDown(InputControlManager.current.actionBar_2))
			{
				OnActionkeyPress(2);
			}
			else if (Input.GetKeyDown(InputControlManager.current.actionBar_3))
			{
				OnActionkeyPress(3);
			}
			else if (Input.GetKeyDown(InputControlManager.current.actionBar_4))
			{
				OnActionkeyPress(4);
			}
			else if (Input.GetKeyDown(InputControlManager.current.actionBar_5))
			{
				OnActionkeyPress(5);
			}
		}
		else
		{
			_actionSlots[0]._gamepadDisplayIcon.sprite = _gamepadInputIco_west;
			_actionSlots[1]._gamepadDisplayIcon.sprite = _gamepadInputIco_north;
			_actionSlots[2]._gamepadDisplayIcon.sprite = _gamepadInputIco_east;
			_actionSlots[3]._gamepadDisplayIcon.sprite = _gamepadInputIco_west;
			_actionSlots[4]._gamepadDisplayIcon.sprite = _gamepadInputIco_north;
			_actionSlots[5]._gamepadDisplayIcon.sprite = _gamepadInputIco_east;
			_actionSlotTransforms[1].anchoredPosition = new Vector2(105f, -20f);
			_actionSlotTransforms[4].anchoredPosition = new Vector2(315f, -20f);
			_leftShoulderIco.color = Color.grey;
			_rightShoulderIco.color = Color.grey;
			if (flag3)
			{
				if (GamepadInput.GetButton(GamepadButton.LeftShoulder))
				{
					_actionSlotTransforms[1].anchoredPosition = new Vector2(105f, 0f);
					_actionSlotTransforms[4].anchoredPosition = new Vector2(315f, -20f);
					_leftShoulderIco.color = Color.white;
					_gamepadRecallHotkeyIco.color = Color.grey;
					_actionSlots[0]._gamepadDisplayIcon.enabled = true;
					_actionSlots[1]._gamepadDisplayIcon.enabled = true;
					_actionSlots[2]._gamepadDisplayIcon.enabled = true;
					_actionSlots[3]._gamepadDisplayIcon.enabled = false;
					_actionSlots[4]._gamepadDisplayIcon.enabled = false;
					_actionSlots[5]._gamepadDisplayIcon.enabled = false;
					if (GamepadInput.GetButtonDown(GamepadButton.Button_West))
					{
						OnActionkeyPress(0);
					}
					else if (GamepadInput.GetButtonDown(GamepadButton.Button_North))
					{
						OnActionkeyPress(1);
					}
					else if (GamepadInput.GetButtonDown(GamepadButton.Button_East))
					{
						OnActionkeyPress(2);
					}
				}
				else if (GamepadInput.GetButton(GamepadButton.RightShoulder))
				{
					_actionSlotTransforms[1].anchoredPosition = new Vector2(105f, -20f);
					_actionSlotTransforms[4].anchoredPosition = new Vector2(315f, -0f);
					_rightShoulderIco.color = Color.white;
					_gamepadRecallHotkeyIco.color = Color.grey;
					_actionSlots[0]._gamepadDisplayIcon.enabled = false;
					_actionSlots[1]._gamepadDisplayIcon.enabled = false;
					_actionSlots[2]._gamepadDisplayIcon.enabled = false;
					_actionSlots[3]._gamepadDisplayIcon.enabled = true;
					_actionSlots[4]._gamepadDisplayIcon.enabled = true;
					_actionSlots[5]._gamepadDisplayIcon.enabled = true;
					if (GamepadInput.GetButtonDown(GamepadButton.Button_West))
					{
						OnActionkeyPress(3);
					}
					else if (GamepadInput.GetButtonDown(GamepadButton.Button_North))
					{
						OnActionkeyPress(4);
					}
					else if (GamepadInput.GetButtonDown(GamepadButton.Button_East))
					{
						OnActionkeyPress(5);
					}
				}
				else
				{
					for (int j = 0; j < _actionSlots.Length; j++)
					{
						_actionSlots[j]._gamepadDisplayIcon.enabled = false;
					}
				}
			}
		}
		for (int k = 0; k < _actionSlots.Length; k++)
		{
			Iterate_ActionSlot(_actionSlots[k], k);
		}
		void Iterate_ActionSlot(ActionSlot _actionSlot, int num)
		{
			if (!_player._bufferingStatus)
			{
				_actionSlotGlowGroups[num].alpha = Mathf.Lerp(_actionSlotGlowGroups[num].alpha, 0f, 3.5f * Time.deltaTime);
				if ((!_pCombat._isUsingAltWeapon && !_pCast._skillLoadoutSlots[num]._scriptSkill) || (_pCombat._isUsingAltWeapon && !_pCast._skillLoadoutSlots[num]._altScriptSkill))
				{
					_actionSlots[num]._scriptSkill = null;
					_actionSlots[num]._wepSwapIconElement.isEnabled = false;
					Inactive_ActionSlot(num);
				}
				else
				{
					_actionSlot._skillIcon.color = _defaultIconColor;
					_actionSlot._ammoCounter.color = _actionSlot._defaultAmmoColor;
					_actionSlot._manaCounter.color = _actionSlot._defaultManaColor;
					_actionSlot._healthCounter.color = _actionSlot._defaultHealthColor;
					_actionSlot._staminaCounter.color = _actionSlot.defaultStaminaColor;
					_actionSlot._isInactive = false;
					if (!_pCombat._isUsingAltWeapon && (bool)_pCast._skillLoadoutSlots[num]._scriptSkill)
					{
						_actionSlot._scriptSkill = _pCast._skillLoadoutSlots[num]._scriptSkill;
					}
					else if (_pCombat._isUsingAltWeapon && (bool)_pCast._skillLoadoutSlots[num]._altScriptSkill)
					{
						_actionSlot._scriptSkill = _pCast._skillLoadoutSlots[num]._altScriptSkill;
					}
					if (_pMove._movementLockType != MovementLockType.NONE)
					{
						Inactive_ActionSlot(num);
					}
					if (_pMove._currentMovementAction == MovementAction.CLIMB)
					{
						Inactive_ActionSlot(num);
					}
					else if (_pMove._currentMovementAction == MovementAction.LEDGEGRAB)
					{
						Inactive_ActionSlot(num);
					}
					if (_pCombat._isChargingWeapon)
					{
						Inactive_ActionSlot(num);
					}
					if (_pCombat._currentCommandTimer > 0f)
					{
						Inactive_ActionSlot(num);
					}
					if (_player._currentPlayerAction == PlayerAction.CASTING)
					{
						Inactive_ActionSlot(num);
					}
					if (_pCast._underGlobalCooldown)
					{
						Inactive_ActionSlot(num);
					}
					if (_pCast._isQueuedSkill)
					{
						Inactive_ActionSlot(num);
					}
					if (_player._chatBehaviour._focusedInChat)
					{
						Inactive_ActionSlot(num);
					}
					if (TabMenu._current._isOpen && TabMenu._current._selectedMenuCell != TabMenu._current._skillsCell)
					{
						Inactive_ActionSlot(num);
					}
					ScriptableSkill scriptSkill = _actionSlot._scriptSkill;
					if (_pCast._skillLoadoutSlots[num]._skillStructIndex > -1 && (bool)scriptSkill && !(_actionSlots[num]._scriptSkill == null))
					{
						if (scriptSkill._skillControlType == SkillControlType.Passive || (scriptSkill._skillUtilityType != _actionSlot._slotUtilityType && _actionSlot._slotUtilityType != CombatColliderType.NULL))
						{
							Init_ClearSlot(num);
						}
						else
						{
							_actionSlots[num]._skillQueueDisplay.gameObject.SetActive(_pCast._currentCastSkill == scriptSkill);
							if (_player._currentPlayerCondition != PlayerCondition.ACTIVE)
							{
								Inactive_ActionSlot(num);
							}
							if (scriptSkill._weaponTypeRequirements != null && scriptSkill._weaponTypeRequirements.Length != 0)
							{
								bool flag4 = false;
								for (int l = 0; l < scriptSkill._weaponTypeRequirements.Length; l++)
								{
									ScriptableWeaponType wepType = scriptSkill._weaponTypeRequirements[l]._wepType;
									if (wepType == _pCombat._currentScriptableWeaponType)
									{
										flag4 = true;
										_actionSlots[num]._wepSwapIconElement.isEnabled = false;
										break;
									}
									if (_pCombat._lockWeapon)
									{
										flag4 = false;
									}
									else if (!_pCombat._isUsingAltWeapon && (bool)_pCombat._foundAltWeaponEquip._scriptableEquip)
									{
										if (((ScriptableWeapon)_pCombat._foundAltWeaponEquip._scriptableEquip).weaponType == wepType)
										{
											flag4 = true;
											_actionSlots[num]._wepSwapIconElement.isEnabled = true;
											break;
										}
									}
									else if (_pCombat._isUsingAltWeapon && (bool)_pCombat._foundWeaponEquip._scriptableEquip && ((ScriptableWeapon)_pCombat._foundWeaponEquip._scriptableEquip).weaponType == wepType)
									{
										flag4 = true;
										_actionSlots[num]._wepSwapIconElement.isEnabled = true;
										break;
									}
								}
								if (_pCombat._isDisarmed)
								{
									flag4 = false;
								}
								if (!flag4)
								{
									Inactive_ActionSlot(num);
									_actionSlots[num]._wepSwapIconElement.isEnabled = false;
								}
							}
							else
							{
								_actionSlots[num]._wepSwapIconElement.isEnabled = false;
							}
							if (scriptSkill._requireShield)
							{
								if (!_pCombat._equippedShield)
								{
									Inactive_ActionSlot(num);
									_actionSlots[num]._wepSwapIconElement.isEnabled = false;
								}
								else if (_pCombat._currentScriptableWeaponType._weaponHandedness == WeaponHandedness.Two_Handed)
								{
									if (!_pCombat._isUsingAltWeapon && (bool)_pCombat._foundAltWeaponEquip._scriptableEquip)
									{
										if (((ScriptableWeapon)_pCombat._foundAltWeaponEquip._scriptableEquip).weaponType._weaponHandedness == WeaponHandedness.One_Handed)
										{
											_actionSlots[num]._wepSwapIconElement.isEnabled = true;
										}
										else
										{
											Inactive_ActionSlot(num);
											_actionSlots[num]._wepSwapIconElement.isEnabled = false;
										}
									}
									else if (_pCombat._isUsingAltWeapon && (bool)_pCombat._foundWeaponEquip._scriptableEquip)
									{
										if (((ScriptableWeapon)_pCombat._foundWeaponEquip._scriptableEquip).weaponType._weaponHandedness == WeaponHandedness.One_Handed)
										{
											_actionSlots[num]._wepSwapIconElement.isEnabled = true;
										}
										else
										{
											Inactive_ActionSlot(num);
											_actionSlots[num]._wepSwapIconElement.isEnabled = false;
										}
									}
								}
							}
							if (_pCast._lockControlBuffer > 0f)
							{
								Inactive_ActionSlot(num);
							}
							if (_pCast._ignoreCost)
							{
								_actionSlot._manaCounter.text = "-";
								_actionSlot._healthCounter.text = "-";
								_actionSlot._staminaCounter.text = "-";
								_actionSlot._manaCounter.color = Color.green;
								_actionSlot._healthCounter.color = Color.green;
								_actionSlot._staminaCounter.color = Color.green;
							}
							else
							{
								if (scriptSkill._skillRankParams._manaCost > 0 && _statusEntity._currentMana < scriptSkill._skillRankParams._manaCost)
								{
									Inactive_ActionSlot(num);
									_actionSlot._manaCounter.color = Color.red;
								}
								if (scriptSkill._skillRankParams._healthCost > 0 && _statusEntity._currentHealth < scriptSkill._skillRankParams._healthCost)
								{
									Inactive_ActionSlot(num);
									_actionSlot._healthCounter.color = Color.red;
								}
								if (scriptSkill._skillRankParams._staminaCost > 0 && _statusEntity._currentStamina < scriptSkill._skillRankParams._staminaCost)
								{
									Inactive_ActionSlot(num);
									_actionSlot._staminaCounter.color = Color.red;
								}
								_actionSlot._manaCounterObject.SetActive(scriptSkill._skillRankParams._manaCost > 0);
								_actionSlot._manaCounter.text = $"{scriptSkill._skillRankParams._manaCost}";
								_actionSlot._healthCounterObject.SetActive(scriptSkill._skillRankParams._healthCost > 0);
								_actionSlot._healthCounter.text = $"{scriptSkill._skillRankParams._healthCost}";
								_actionSlot._staminaCounterObject.SetActive(scriptSkill._skillRankParams._staminaCost > 0);
								_actionSlot._staminaCounter.text = $"{scriptSkill._skillRankParams._staminaCost}";
							}
							if (scriptSkill._skillRankParams._checkFullHealth && _statusEntity._currentHealth >= _player._pStats._statStruct._maxHealth)
							{
								Inactive_ActionSlot(num);
							}
							if (scriptSkill._skillRankParams._checkFullMana && _statusEntity._currentMana >= _player._pStats._statStruct._maxMana)
							{
								Inactive_ActionSlot(num);
							}
							if (scriptSkill._skillRankParams._checkFullStamina && _statusEntity._currentStamina >= _player._pStats._statStruct._maxStamina)
							{
								Inactive_ActionSlot(num);
							}
							bool flag5 = (bool)scriptSkill._skillRankParams._baseRequiredItem && scriptSkill._skillRankParams._basedItemCost > 0;
							_actionSlot._ammoCounterObject.SetActive(flag5);
							if (flag5)
							{
								int num2 = 0;
								for (int m = 0; m < _pInventory._heldItems.Count; m++)
								{
									if (_pInventory._heldItems[m]._itemName == scriptSkill._skillRankParams._baseRequiredItem._itemName)
									{
										num2 += _pInventory._heldItems[m]._quantity;
									}
								}
								if (num2 < 999)
								{
									_actionSlots[num]._ammoCounter.text = $"{num2}";
								}
								else
								{
									_actionSlot._ammoCounter.text = "*";
								}
								if (num2 < scriptSkill._skillRankParams._basedItemCost)
								{
									Inactive_ActionSlot(num);
									_actionSlot._ammoCounter.color = Color.red;
								}
							}
						}
					}
				}
			}
		}
	}

	private void Inactive_ActionSlot(int i)
	{
		_actionSlots[i]._skillIcon.color = _inactiveIconColor;
		_actionSlots[i]._isInactive = true;
	}

	private void OnActionkeyPress(int index)
	{
		if (_player._currentPlayerCondition == PlayerCondition.ACTIVE && (!_player._inUI || Cursor.lockState != CursorLockMode.None) && !_player._bufferingStatus && !_player._inChat)
		{
			if (!_actionSlots[index]._isInactive)
			{
				_aSrc.Stop();
				_aSrc.PlayOneShot(_actionSlotPress, 0.45f);
				_actionSlotTransforms[index].localScale = new Vector2(0.75f, 0.75f);
			}
			else
			{
				_aSrc.Stop();
				_aSrc.PlayOneShot(_actionSlotPress, 0.45f);
				_actionSlotTransforms[index].localScale = new Vector2(0.95f, 0.95f);
			}
		}
	}

	public void OnClick_ActionSlot(int _index)
	{
		if (!Player._mainPlayer || Player._mainPlayer._bufferingStatus)
		{
			return;
		}
		ScriptableSkill scriptableSkill = (_pCombat._isUsingAltWeapon ? _pCast._skillLoadoutSlots[_index]._altScriptSkill : _pCast._skillLoadoutSlots[_index]._scriptSkill);
		if (!scriptableSkill)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("No Skill Assigned");
		}
		else if (Input.GetMouseButton(0))
		{
			for (int i = 0; i < TabMenu._current._skillsCell._listDataEntries.Count; i++)
			{
				SkillListDataEntry component = TabMenu._current._skillsCell._listDataEntries[i].GetComponent<SkillListDataEntry>();
				if (component._scriptSkill == scriptableSkill)
				{
					component.OnClick_SkillIcon();
				}
			}
		}
		else if (Input.GetMouseButton(1) && TabMenu._current._selectedMenuCell == TabMenu._current._skillsCell)
		{
			Init_ClearSlot(_index);
		}
	}

	public void Init_ClearSlot(int _index)
	{
		if (_player._playerZoneType != ZoneType.Safe)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Cannot Clear Skill Here");
			return;
		}
		_pCast._setSkillPresetIndex[_index] = string.Empty;
		Glow_ActionBarIcon(_index);
		Disable_SkillToolTip();
	}

	private void Handle_ActionSlotCooldownDisplay()
	{
		if (Player._mainPlayer._bufferingStatus)
		{
			return;
		}
		for (int i = 0; i < _actionSlots.Length; i++)
		{
			_actionSlots[i]._skillCoolDown.enabled = false;
			for (int j = 0; j < _pCast._skillCoolDownSlots.Count; j++)
			{
				if (!(_actionSlots[i]._scriptSkill != _pCast._skillCoolDownSlots[j]._scriptSkill))
				{
					_actionSlots[i]._skillCoolDown.fillAmount = _pCast._skillCoolDownSlots[j]._currentCooldown / _pCast._skillCoolDownSlots[j]._setCooldown;
					_actionSlots[i]._skillCoolDown.enabled = true;
					if (SettingsManager._current._displaySlotCooldownCounters.isOn)
					{
						_actionSlots[i]._cooldownCounter.text = _pCast._skillCoolDownSlots[j]._currentCooldown.ToString("F1");
					}
					Inactive_ActionSlot(i);
				}
			}
		}
	}

	public void Init_RowScaleEffect()
	{
		StopCoroutine("RowScaleEffect");
		StartCoroutine(RowScaleEffect());
	}

	private IEnumerator RowScaleEffect()
	{
		float _scaleMod = 2f;
		for (int i = 0; i < _actionSlotTransforms.Length; i++)
		{
			_actionSlotTransforms[i].localScale *= _scaleMod;
			_scaleMod -= 0.06f;
			yield return new WaitForSeconds(0.016f);
		}
	}

	private void Handle_CastBar()
	{
		_castBarObject.SetActive(_pCast._currentCastSkill);
		if ((bool)_pCast._currentCastSkill)
		{
			_castBarTag.text = _pCast._currentCastSkill._skillName;
			castBarMaxFill = _pCast._currentCastSkill._skillRankParams._baseCastTime + _pCast._castTimerMod;
			if (_pCast._currentCastSkill._skillRankParams._baseCastTime + _pCast._castTimerMod <= 0.12f)
			{
				_castBarObject.SetActive(value: false);
				return;
			}
		}
		if (_castBarImage.fillAmount >= 0.85f)
		{
			_castBarImage.color = Color.Lerp(_castBarImage.color, Color.yellow, 9.6f * Time.deltaTime);
		}
		else
		{
			_castBarImage.color = Color.green;
		}
		_castBarImage.fillAmount = _pCast._castTimer / castBarMaxFill;
	}

	public void Init_SkillToolTip(int _index)
	{
		_isHighlightingSlot = true;
		_currentHighlightedSlotIndex = _index;
		_actionSlotTransforms[_currentHighlightedSlotIndex].localScale = new Vector2(1.45f, 1.45f);
		if (!_player)
		{
			ToolTipManager._current._skillToolTip.Disable_ToolTip();
			return;
		}
		if (_player._bufferingStatus)
		{
			ToolTipManager._current._skillToolTip.Disable_ToolTip();
			return;
		}
		if (_pCast._skillLoadoutSlots[_index] == null)
		{
			ToolTipManager._current._skillToolTip.Disable_ToolTip();
			return;
		}
		if (string.IsNullOrEmpty(_pCast._setSkillPresetIndex[_index]))
		{
			ToolTipManager._current._skillToolTip.Disable_ToolTip();
			return;
		}
		if (_pCast._playerSkillStructs.Count < _pCast._skillLoadoutSlots[_index]._skillStructIndex)
		{
			ToolTipManager._current._skillToolTip.Disable_ToolTip();
			return;
		}
		if (!_actionSlots[_index]._scriptSkill)
		{
			ToolTipManager._current._skillToolTip.Disable_ToolTip();
			return;
		}
		ScriptableSkill scriptableSkill = (_pCombat._isUsingAltWeapon ? _pCast._skillLoadoutSlots[_index]._altScriptSkill : _pCast._skillLoadoutSlots[_index]._scriptSkill);
		if ((bool)scriptableSkill)
		{
			SkillStruct skillStruct = _pCast._playerSkillStructs[_pCast._skillLoadoutSlots[_index]._skillStructIndex];
			ToolTipManager._current.Apply_SkillDataToolTip(scriptableSkill, skillStruct);
			ToolTipManager._current._skillToolTip.Set_TooltipAnchorPos(_skillToolTipAnchor);
			ToolTipManager._current._skillToolTip.Enable_ToolTip();
		}
	}

	public void Disable_SkillToolTip()
	{
		ToolTipManager._current._skillToolTip.Disable_ToolTip();
		_isHighlightingSlot = false;
	}
}
