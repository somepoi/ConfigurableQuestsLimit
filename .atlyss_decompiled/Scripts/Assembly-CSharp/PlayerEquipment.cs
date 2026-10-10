using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerEquipment : NetworkBehaviour
{
	[CompilerGenerated]
	private sealed class _003CInit_Equip_003Ed__17 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public ScriptableEquipment _scriptEquip;

		public PlayerEquipment _003C_003E4__this;

		public ItemData _itemData;

		public string _key;

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
		public _003CInit_Equip_003Ed__17(int _003C_003E1__state)
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
			PlayerEquipment playerEquipment = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				if (!_scriptEquip.CanEquipItem(playerEquipment._player._pStats, _isClickPrompt: false))
				{
					_itemData._isEquipped = false;
					return false;
				}
				if (playerEquipment._equips[_key]._heldItem != null)
				{
					playerEquipment._equips[_key]._heldItem._isEquipped = false;
					playerEquipment._equips[_key]._heldItem._isAltWeapon = false;
					if (!playerEquipment._player._bufferingStatus && (bool)playerEquipment._equips[_key]._scriptableEquip)
					{
						playerEquipment._cBehaviour.Init_GameLogicMessage("Un-equipped " + playerEquipment._equips[_key]._scriptableEquip._itemName + ".");
					}
				}
				playerEquipment._equips[_key]._scriptableEquip = null;
				playerEquipment._equips[_key]._heldItem = null;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				playerEquipment._equips[_key]._scriptableEquip = _scriptEquip;
				playerEquipment._equips[_key]._heldItem = _itemData;
				if (!playerEquipment._player._bufferingStatus)
				{
					playerEquipment._cBehaviour.Init_GameLogicMessage("Equipped " + _itemData._itemName + ".");
					ProfileDataManager._current.Save_ProfileData();
				}
				TabMenu._current._itemCell.Refresh_EquipCells();
				playerEquipment.Init_SendEquipStruct(EquipSfx.EQUIP);
				return false;
			}
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

	private PlayerVisual _pVisual;

	private PlayerSound _pSound;

	private PlayerCombat _pCombat;

	private ChatBehaviour _cBehaviour;

	[Header("Player Equipment")]
	[SyncVar]
	public EquipSyncStruct _equipSyncStruct;

	[SyncVar]
	public EquipSyncStruct _vanitySyncStruct;

	[SyncVar]
	public bool _setToDisplay = true;

	public Dictionary<string, EquipData> _equips = new Dictionary<string, EquipData>();

	private float _equipSoundInitBuffer;

	public EquipSyncStruct Network_equipSyncStruct
	{
		get
		{
			return _equipSyncStruct;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _equipSyncStruct, 1uL, null);
		}
	}

	public EquipSyncStruct Network_vanitySyncStruct
	{
		get
		{
			return _vanitySyncStruct;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _vanitySyncStruct, 2uL, null);
		}
	}

	public bool Network_setToDisplay
	{
		get
		{
			return _setToDisplay;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _setToDisplay, 4uL, null);
		}
	}

	[ClientRpc]
	private void Rpc_EquipSfx(EquipSfx _setSfx)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_EquipSfx(writer, _setSfx);
		SendRPCInternal("System.Void PlayerEquipment::Rpc_EquipSfx(EquipSfx)", -1144814780, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Awake()
	{
		_player = GetComponent<Player>();
		_pInventory = _player._pInventory;
		_pStats = _player._pStats;
		_pVisual = _player._pVisual;
		_pSound = _player._pSound;
		_pCombat = _player._pCombat;
		_cBehaviour = _player._chatBehaviour;
		_equips.Add("Helm", new EquipData
		{
			_heldItem = new ItemData()
		});
		_equips.Add("Chest", new EquipData
		{
			_heldItem = new ItemData()
		});
		_equips.Add("Leggings", new EquipData
		{
			_heldItem = new ItemData()
		});
		_equips.Add("Cape", new EquipData
		{
			_heldItem = new ItemData()
		});
		_equips.Add("Ring", new EquipData
		{
			_heldItem = new ItemData()
		});
		_equips.Add("Weapon", new EquipData
		{
			_heldItem = new ItemData()
		});
		_equips.Add("Alt Weapon", new EquipData
		{
			_heldItem = new ItemData()
		});
		_equips.Add("Shield", new EquipData
		{
			_heldItem = new ItemData()
		});
	}

	private void Update()
	{
		if (base.isClientOnly && (bool)Player._mainPlayer && Player._mainPlayer.Network_playerMapInstance == _player.Network_playerMapInstance)
		{
			Set_EquipParams_BySyncStructs();
		}
		if (base.isServer && (bool)_pVisual._raceModelED)
		{
			Network_setToDisplay = _pVisual._raceModelED._displayBuffer > 0f;
		}
		if (_equipSoundInitBuffer < 0.25f)
		{
			_equipSoundInitBuffer += Time.deltaTime;
		}
		else if (_equipSoundInitBuffer > 0.25f)
		{
			_equipSoundInitBuffer = 0.25f;
		}
	}

	private void Set_EquipParams_BySyncStructs()
	{
		if (!base.isLocalPlayer || _player._bufferingStatus)
		{
			Apply_Helm();
			Apply_Chest();
			Apply_Leggings();
			Apply_Cape();
			Apply_Ring();
			Apply_Weapon();
			Apply_AltWeapon();
			Apply_Shield();
		}
		void Apply_AltWeapon()
		{
			if (!string.IsNullOrEmpty(_equipSyncStruct._altWeaponTag))
			{
				_equips["Alt Weapon"]._heldItem._modifierID = _equipSyncStruct._altWeaponModifier;
				_equips["Alt Weapon"]._scriptableEquip = (ScriptableWeapon)GameManager._current.Locate_Item(_equipSyncStruct._altWeaponTag);
				_equips["Alt Weapon"]._heldItem._useDamageTypeOverride = _equipSyncStruct._useDamageOverride_altWep;
				_equips["Alt Weapon"]._heldItem._damageTypeOverride = _equipSyncStruct._damageOverride_altWep;
			}
			else
			{
				_equips["Alt Weapon"]._scriptableEquip = null;
			}
		}
		void Apply_Cape()
		{
			if (!string.IsNullOrEmpty(_equipSyncStruct._capeTag))
			{
				ScriptableCape scriptableEquip = (ScriptableCape)GameManager._current.Locate_Item(_equipSyncStruct._capeTag);
				_equips["Cape"]._heldItem._modifierID = _equipSyncStruct._capeModifier;
				_equips["Cape"]._scriptableEquip = scriptableEquip;
			}
			else
			{
				_equips["Cape"]._scriptableEquip = null;
			}
			if (!string.IsNullOrEmpty(_vanitySyncStruct._capeTag))
			{
				_equips["Cape"]._vanityEquip = (ScriptableCape)GameManager._current.Locate_Item(_vanitySyncStruct._capeTag);
			}
			else
			{
				_equips["Cape"]._vanityEquip = null;
			}
		}
		void Apply_Chest()
		{
			if (!string.IsNullOrEmpty(_equipSyncStruct._chestTag))
			{
				ScriptableChestpiece scriptableEquip = (ScriptableChestpiece)GameManager._current.Locate_Item(_equipSyncStruct._chestTag);
				_equips["Chest"]._heldItem._modifierID = _equipSyncStruct._chestModifier;
				_equips["Chest"]._scriptableEquip = scriptableEquip;
			}
			else
			{
				_equips["Chest"]._scriptableEquip = null;
			}
			if (!string.IsNullOrEmpty(_vanitySyncStruct._chestTag))
			{
				_equips["Chest"]._vanityEquip = (ScriptableChestpiece)GameManager._current.Locate_Item(_vanitySyncStruct._chestTag);
			}
			else
			{
				_equips["Chest"]._vanityEquip = null;
			}
		}
		void Apply_Helm()
		{
			if (!string.IsNullOrEmpty(_equipSyncStruct._helmTag))
			{
				ScriptableHelm scriptableEquip = (ScriptableHelm)GameManager._current.Locate_Item(_equipSyncStruct._helmTag);
				_equips["Helm"]._heldItem._modifierID = _equipSyncStruct._helmModifier;
				_equips["Helm"]._scriptableEquip = scriptableEquip;
			}
			else
			{
				_equips["Helm"]._scriptableEquip = null;
			}
			if (!string.IsNullOrEmpty(_vanitySyncStruct._helmTag))
			{
				_equips["Helm"]._vanityEquip = (ScriptableHelm)GameManager._current.Locate_Item(_vanitySyncStruct._helmTag);
			}
			else
			{
				_equips["Helm"]._vanityEquip = null;
			}
		}
		void Apply_Leggings()
		{
			if (!string.IsNullOrEmpty(_equipSyncStruct._leggingTag))
			{
				ScriptableLeggings scriptableEquip = (ScriptableLeggings)GameManager._current.Locate_Item(_equipSyncStruct._leggingTag);
				_equips["Leggings"]._heldItem._modifierID = _equipSyncStruct._leggingModifier;
				_equips["Leggings"]._scriptableEquip = scriptableEquip;
			}
			else
			{
				_equips["Leggings"]._scriptableEquip = null;
			}
			if (!string.IsNullOrEmpty(_vanitySyncStruct._leggingTag))
			{
				_equips["Leggings"]._vanityEquip = (ScriptableLeggings)GameManager._current.Locate_Item(_vanitySyncStruct._leggingTag);
			}
			else
			{
				_equips["Leggings"]._vanityEquip = null;
			}
		}
		void Apply_Ring()
		{
			if (!string.IsNullOrEmpty(_equipSyncStruct._ringTag))
			{
				ScriptableRing scriptableEquip = (ScriptableRing)GameManager._current.Locate_Item(_equipSyncStruct._ringTag);
				_equips["Ring"]._heldItem._modifierID = _equipSyncStruct._ringModifier;
				_equips["Ring"]._scriptableEquip = scriptableEquip;
			}
			else
			{
				_equips["Ring"]._scriptableEquip = null;
			}
		}
		void Apply_Shield()
		{
			if (!string.IsNullOrEmpty(_equipSyncStruct._shieldTag))
			{
				ScriptableShield scriptableEquip = (ScriptableShield)GameManager._current.Locate_Item(_equipSyncStruct._shieldTag);
				_equips["Shield"]._heldItem._modifierID = _equipSyncStruct._shieldModifier;
				_equips["Shield"]._scriptableEquip = scriptableEquip;
			}
			else
			{
				_equips["Shield"]._scriptableEquip = null;
			}
			if (!string.IsNullOrEmpty(_vanitySyncStruct._shieldTag))
			{
				_equips["Shield"]._vanityEquip = (ScriptableShield)GameManager._current.Locate_Item(_vanitySyncStruct._shieldTag);
			}
			else
			{
				_equips["Shield"]._vanityEquip = null;
			}
		}
		void Apply_Weapon()
		{
			if (!string.IsNullOrEmpty(_equipSyncStruct._weaponTag))
			{
				_equips["Weapon"]._heldItem._modifierID = _equipSyncStruct._weaponModifier;
				_equips["Weapon"]._scriptableEquip = (ScriptableWeapon)GameManager._current.Locate_Item(_equipSyncStruct._weaponTag);
				_equips["Weapon"]._heldItem._useDamageTypeOverride = _equipSyncStruct._useDamageOverride_mainWep;
				_equips["Weapon"]._heldItem._damageTypeOverride = _equipSyncStruct._damageOverride_mainWep;
			}
			else
			{
				_equips["Weapon"]._scriptableEquip = null;
			}
		}
	}

	[ClientCallback]
	public void Init_EquipItem(ItemData _itemData)
	{
		if (!NetworkClient.active || !base.isLocalPlayer || _pInventory._heldItems.Count <= 0)
		{
			return;
		}
		if (_player._currentPlayerAction == PlayerAction.ATTACKING || _pCombat._isAirAttacked || (bool)_player._pCasting._currentCastSkill)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Busy");
			return;
		}
		ScriptableItem scriptableItem = GameManager._current.Locate_Item(_itemData._itemName);
		if (!scriptableItem || scriptableItem._itemType != ItemType.GEAR)
		{
			return;
		}
		ScriptableEquipment scriptableEquipment = (ScriptableEquipment)scriptableItem;
		bool isAltWeapon = _itemData._isAltWeapon;
		_itemData._isEquipped = true;
		_itemData._slotNumber = 0;
		if (scriptableEquipment._equipType == ItemEquipType.ARMOR)
		{
			ScriptableArmor scriptableArmor = (ScriptableArmor)scriptableItem;
			string key = "";
			if (scriptableArmor.GetType() == typeof(ScriptableHelm))
			{
				key = "Helm";
			}
			else if (scriptableArmor.GetType() == typeof(ScriptableChestpiece))
			{
				key = "Chest";
			}
			else if (scriptableArmor.GetType() == typeof(ScriptableLeggings))
			{
				key = "Leggings";
			}
			else if (scriptableArmor.GetType() == typeof(ScriptableCape))
			{
				key = "Cape";
			}
			else if (scriptableArmor.GetType() == typeof(ScriptableRing))
			{
				key = "Ring";
			}
			StartCoroutine(Init_Equip(key, _itemData, scriptableEquipment));
			_itemData._damageTypeOverride = DamageType.Strength;
			_itemData._useDamageTypeOverride = false;
		}
		else if (scriptableItem.GetType() == typeof(ScriptableWeapon))
		{
			if (_pCombat._isDisarmed)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Disarmed");
				return;
			}
			string key2 = "Weapon";
			if (isAltWeapon)
			{
				key2 = "Alt Weapon";
			}
			if (!_player._bufferingStatus)
			{
				_itemData._isAltWeapon = _pCombat._isUsingAltWeapon && !_pCombat._lockWeapon;
			}
			else
			{
				_itemData._isAltWeapon = isAltWeapon;
			}
			if (_itemData._isAltWeapon)
			{
				key2 = "Alt Weapon";
			}
			StartCoroutine(Init_Equip(key2, _itemData, scriptableEquipment));
		}
		else if (scriptableItem.GetType() == typeof(ScriptableShield))
		{
			StartCoroutine(Init_Equip("Shield", _itemData, scriptableEquipment));
			_itemData._damageTypeOverride = DamageType.Strength;
			_itemData._useDamageTypeOverride = false;
		}
	}

	[IteratorStateMachine(typeof(_003CInit_Equip_003Ed__17))]
	[ClientCallback]
	private IEnumerator Init_Equip(string _key, ItemData _itemData, ScriptableEquipment _scriptEquip)
	{
		if (!NetworkClient.active)
		{
			return null;
		}
		return new _003CInit_Equip_003Ed__17(0)
		{
			_003C_003E4__this = this,
			_key = _key,
			_itemData = _itemData,
			_scriptEquip = _scriptEquip
		};
	}

	[ClientCallback]
	public void Init_UnEquipItem(ItemData _itemData)
	{
		if (!NetworkClient.active || !base.isLocalPlayer || _pInventory._heldItems.Count <= 0 || !_itemData._isEquipped)
		{
			return;
		}
		if (_pInventory._usedEquipmentSlots >= _pInventory._maxInventoryStruct._maxEquipmentSlots)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Equipment Inventory Full");
			return;
		}
		if (_player._currentPlayerAction == PlayerAction.ATTACKING || _pCombat._isAirAttacked || (bool)_player._pCasting._currentCastSkill)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Busy");
			return;
		}
		ScriptableItem scriptableItem = GameManager._current.Locate_Item(_itemData._itemName);
		if (!scriptableItem || scriptableItem._itemType != ItemType.GEAR)
		{
			return;
		}
		ScriptableEquipment obj = (ScriptableEquipment)scriptableItem;
		string key = string.Empty;
		if (obj._equipType == ItemEquipType.ARMOR)
		{
			ScriptableArmor scriptableArmor = (ScriptableArmor)scriptableItem;
			if (scriptableArmor.GetType() == typeof(ScriptableHelm))
			{
				key = "Helm";
			}
			else if (scriptableArmor.GetType() == typeof(ScriptableChestpiece))
			{
				key = "Chest";
			}
			else if (scriptableArmor.GetType() == typeof(ScriptableLeggings))
			{
				key = "Leggings";
			}
			else if (scriptableArmor.GetType() == typeof(ScriptableCape))
			{
				key = "Cape";
			}
			else if (scriptableArmor.GetType() == typeof(ScriptableRing))
			{
				key = "Ring";
			}
		}
		else if (scriptableItem.GetType() == typeof(ScriptableWeapon))
		{
			if (_pCombat._isDisarmed)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Disarmed");
				return;
			}
			key = "Weapon";
			if (_itemData._isAltWeapon)
			{
				key = "Alt Weapon";
			}
		}
		else if (scriptableItem.GetType() == typeof(ScriptableShield))
		{
			key = "Shield";
		}
		_itemData._isEquipped = false;
		_itemData._isAltWeapon = false;
		_itemData._slotNumber = 0;
		_equips[key]._scriptableEquip = null;
		_equips[key]._heldItem = null;
		if (!_player._bufferingStatus)
		{
			_cBehaviour.Init_GameLogicMessage("Un-equipped " + _itemData._itemName + ".");
			ProfileDataManager._current.Save_ProfileData();
		}
		TabMenu._current._itemCell.Refresh_EquipCells();
		Init_SendEquipStruct(EquipSfx.REMOVE);
	}

	[ClientCallback]
	private void Init_SendEquipStruct(EquipSfx _setSfx)
	{
		if (NetworkClient.active)
		{
			EquipSyncStruct newStruct = default;
			if (_equips["Helm"]._heldItem != null)
			{
				newStruct._helmTag = _equips["Helm"]._heldItem._itemName;
				newStruct._helmModifier = _equips["Helm"]._heldItem._modifierID;
			}
			if (_equips["Chest"]._heldItem != null)
			{
				newStruct._chestTag = _equips["Chest"]._heldItem._itemName;
				newStruct._chestModifier = _equips["Chest"]._heldItem._modifierID;
			}
			if (_equips["Leggings"]._heldItem != null)
			{
				newStruct._leggingTag = _equips["Leggings"]._heldItem._itemName;
				newStruct._leggingModifier = _equips["Leggings"]._heldItem._modifierID;
			}
			if (_equips["Cape"]._heldItem != null)
			{
				newStruct._capeTag = _equips["Cape"]._heldItem._itemName;
				newStruct._capeModifier = _equips["Cape"]._heldItem._modifierID;
			}
			if (_equips["Ring"]._heldItem != null)
			{
				newStruct._ringTag = _equips["Ring"]._heldItem._itemName;
				newStruct._ringModifier = _equips["Ring"]._heldItem._modifierID;
			}
			if (_equips["Weapon"]._heldItem != null)
			{
				newStruct._weaponTag = _equips["Weapon"]._heldItem._itemName;
				newStruct._weaponModifier = _equips["Weapon"]._heldItem._modifierID;
				newStruct._useDamageOverride_mainWep = _equips["Weapon"]._heldItem._useDamageTypeOverride;
				newStruct._damageOverride_mainWep = _equips["Weapon"]._heldItem._damageTypeOverride;
			}
			if (_equips["Alt Weapon"]._heldItem != null)
			{
				newStruct._altWeaponTag = _equips["Alt Weapon"]._heldItem._itemName;
				newStruct._altWeaponModifier = _equips["Alt Weapon"]._heldItem._modifierID;
				newStruct._useDamageOverride_altWep = _equips["Alt Weapon"]._heldItem._useDamageTypeOverride;
				newStruct._damageOverride_altWep = _equips["Alt Weapon"]._heldItem._damageTypeOverride;
			}
			if (_equips["Shield"]._heldItem != null)
			{
				newStruct._shieldTag = _equips["Shield"]._heldItem._itemName;
				newStruct._shieldModifier = _equips["Shield"]._heldItem._modifierID;
			}
			Cmd_SyncEquipStruct(newStruct, _setSfx);
		}
	}

	[Command]
	public void Cmd_SyncEquipStruct(EquipSyncStruct _newStruct, EquipSfx _equipSfx)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_EquipSyncStruct(writer, _newStruct);
		GeneratedNetworkCode._Write_EquipSfx(writer, _equipSfx);
		SendCommandInternal("System.Void PlayerEquipment::Cmd_SyncEquipStruct(EquipSyncStruct,EquipSfx)", -743098660, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_SyncVanityStruct(EquipSyncStruct _newStruct)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_EquipSyncStruct(writer, _newStruct);
		SendCommandInternal("System.Void PlayerEquipment::Cmd_SyncVanityStruct(EquipSyncStruct)", 1944244296, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_RemoveVanity(string _key)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_key);
		SendCommandInternal("System.Void PlayerEquipment::Cmd_RemoveVanity(System.String)", -756909404, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Init_TransmogItem(ItemData _itemData)
	{
		if (!NetworkClient.active || _pInventory._heldItems.Count <= 0 || !base.isLocalPlayer)
		{
			return;
		}
		ScriptableItem scriptableItem = GameManager._current.Locate_Item(_itemData._itemName);
		if (!scriptableItem)
		{
			return;
		}
		if (Retrieve_IllusionStoneCount() <= 0)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Not Enough Illusion Stones");
			return;
		}
		for (int i = 0; i < _pInventory._heldItems.Count; i++)
		{
			if (_pInventory._heldItems[i]._itemName == "Illusion Stone")
			{
				_pInventory.Remove_Item(_pInventory._heldItems[i], 1);
				break;
			}
		}
		ScriptableEquipment scriptableEquipment = (ScriptableEquipment)scriptableItem;
		string key = string.Empty;
		if (scriptableItem._itemType == ItemType.GEAR && scriptableEquipment._equipType == ItemEquipType.ARMOR)
		{
			ScriptableArmor scriptableArmor = (ScriptableArmor)scriptableItem;
			if (scriptableArmor.GetType() == typeof(ScriptableHelm))
			{
				key = "Helm";
			}
			else if (scriptableArmor.GetType() == typeof(ScriptableChestpiece))
			{
				key = "Chest";
			}
			else if (scriptableArmor.GetType() == typeof(ScriptableLeggings))
			{
				key = "Leggings";
			}
			else if (scriptableArmor.GetType() == typeof(ScriptableCape))
			{
				key = "Cape";
			}
		}
		if (scriptableItem.GetType() == typeof(ScriptableShield))
		{
			key = "Shield";
		}
		_equips[key]._vanityEquip = scriptableEquipment;
		TabMenu._current._itemCell.Refresh_EquipCells();
		_pVisual.Cmd_VanitySparkleEffect();
		Init_SendVanityStruct();
		static int Retrieve_IllusionStoneCount()
		{
			PlayerInventory component = Player._mainPlayer.GetComponent<PlayerInventory>();
			int num = 0;
			for (int j = 0; j < component._heldItems.Count; j++)
			{
				if (component._heldItems[j]._itemName == "Illusion Stone")
				{
					num += component._heldItems[j]._quantity;
				}
			}
			return num;
		}
	}

	[ClientCallback]
	private void Init_SendVanityStruct()
	{
		if (NetworkClient.active)
		{
			EquipSyncStruct newStruct = default;
			if (_equips["Helm"]._vanityEquip != null)
			{
				newStruct._helmTag = _equips["Helm"]._vanityEquip._itemName;
			}
			if (_equips["Chest"]._vanityEquip != null)
			{
				newStruct._chestTag = _equips["Chest"]._vanityEquip._itemName;
			}
			if (_equips["Leggings"]._vanityEquip != null)
			{
				newStruct._leggingTag = _equips["Leggings"]._vanityEquip._itemName;
			}
			if (_equips["Cape"]._vanityEquip != null)
			{
				newStruct._capeTag = _equips["Cape"]._vanityEquip._itemName;
			}
			if (_equips["Ring"]._vanityEquip != null)
			{
				newStruct._ringTag = _equips["Ring"]._vanityEquip._itemName;
			}
			if (_equips["Weapon"]._vanityEquip != null)
			{
				newStruct._weaponTag = _equips["Weapon"]._vanityEquip._itemName;
			}
			if (_equips["Alt Weapon"]._vanityEquip != null)
			{
				newStruct._altWeaponTag = _equips["Alt Weapon"]._vanityEquip._itemName;
			}
			if (_equips["Shield"]._vanityEquip != null)
			{
				newStruct._shieldTag = _equips["Shield"]._vanityEquip._itemName;
			}
			Cmd_SyncVanityStruct(newStruct);
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Rpc_EquipSfx__EquipSfx(EquipSfx _setSfx)
	{
		if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && !_player._bufferingStatus && (base.isLocalPlayer || !(Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance)) && (bool)_pVisual._playerRaceModel && !(_equipSoundInitBuffer < 0.25f))
		{
			switch (_setSfx)
			{
			case EquipSfx.EQUIP:
				_pSound._aSrcGeneral.PlayOneShot(_pSound._equipSound, 0.75f);
				break;
			case EquipSfx.REMOVE:
				_pSound._aSrcGeneral.PlayOneShot(_pSound._unEquipSound, 0.75f);
				break;
			}
			_equipSoundInitBuffer = 0f;
		}
	}

	protected static void InvokeUserCode_Rpc_EquipSfx__EquipSfx(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC Rpc_EquipSfx called on server.");
		}
		else
		{
			((PlayerEquipment)obj).UserCode_Rpc_EquipSfx__EquipSfx(GeneratedNetworkCode._Read_EquipSfx(reader));
		}
	}

	protected void UserCode_Cmd_SyncEquipStruct__EquipSyncStruct__EquipSfx(EquipSyncStruct _newStruct, EquipSfx _equipSfx)
	{
		Network_equipSyncStruct = _newStruct;
		_pVisual._raceModelED._displayBuffer = 12.5f;
		Set_EquipParams_BySyncStructs();
		Rpc_EquipSfx(_equipSfx);
		_pStats.Apply_StatCalculations();
	}

	protected static void InvokeUserCode_Cmd_SyncEquipStruct__EquipSyncStruct__EquipSfx(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command Cmd_SyncEquipStruct called on client.");
		}
		else
		{
			((PlayerEquipment)obj).UserCode_Cmd_SyncEquipStruct__EquipSyncStruct__EquipSfx(GeneratedNetworkCode._Read_EquipSyncStruct(reader), GeneratedNetworkCode._Read_EquipSfx(reader));
		}
	}

	protected void UserCode_Cmd_SyncVanityStruct__EquipSyncStruct(EquipSyncStruct _newStruct)
	{
		Network_vanitySyncStruct = _newStruct;
		_pVisual._raceModelED._displayBuffer = 12.5f;
		Set_EquipParams_BySyncStructs();
		_pStats.Apply_StatCalculations();
	}

	protected static void InvokeUserCode_Cmd_SyncVanityStruct__EquipSyncStruct(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command Cmd_SyncVanityStruct called on client.");
		}
		else
		{
			((PlayerEquipment)obj).UserCode_Cmd_SyncVanityStruct__EquipSyncStruct(GeneratedNetworkCode._Read_EquipSyncStruct(reader));
		}
	}

	protected void UserCode_Cmd_RemoveVanity__String(string _key)
	{
		if (_player._bufferingStatus)
		{
			return;
		}
		EquipSyncStruct vanitySyncStruct = _vanitySyncStruct;
		_equips[_key]._vanityEquip = null;
		_pVisual.Rpc_PoofSmokeEffect();
		switch (_key)
		{
		case "Helm":
			vanitySyncStruct._helmTag = "";
			if (!string.IsNullOrEmpty(_vanitySyncStruct._helmTag))
			{
				_cBehaviour.Target_RecieveMessage("The Vanity in the Helmet Slot dissipates.");
			}
			break;
		case "Chest":
			vanitySyncStruct._chestTag = "";
			if (!string.IsNullOrEmpty(_vanitySyncStruct._chestTag))
			{
				_cBehaviour.Target_RecieveMessage("The Vanity in the Chestpiece Slot dissipates.");
			}
			break;
		case "Leggings":
			vanitySyncStruct._leggingTag = "";
			if (!string.IsNullOrEmpty(_vanitySyncStruct._leggingTag))
			{
				_cBehaviour.Target_RecieveMessage("The Vanity in the Leggings Slot dissipates.");
			}
			break;
		case "Cape":
			vanitySyncStruct._capeTag = "";
			if (!string.IsNullOrEmpty(_vanitySyncStruct._capeTag))
			{
				_cBehaviour.Target_RecieveMessage("The Vanity in the Cape Slot dissipates.");
			}
			break;
		case "Shield":
			vanitySyncStruct._shieldTag = "";
			if (!string.IsNullOrEmpty(_vanitySyncStruct._shieldTag))
			{
				_cBehaviour.Target_RecieveMessage("The Vanity in the Shield Slot dissipates.");
			}
			break;
		}
		Network_vanitySyncStruct = vanitySyncStruct;
		_pVisual._raceModelED._displayBuffer = 12.5f;
	}

	protected static void InvokeUserCode_Cmd_RemoveVanity__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command Cmd_RemoveVanity called on client.");
		}
		else
		{
			((PlayerEquipment)obj).UserCode_Cmd_RemoveVanity__String(reader.ReadString());
		}
	}

	static PlayerEquipment()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerEquipment), "System.Void PlayerEquipment::Cmd_SyncEquipStruct(EquipSyncStruct,EquipSfx)", InvokeUserCode_Cmd_SyncEquipStruct__EquipSyncStruct__EquipSfx, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerEquipment), "System.Void PlayerEquipment::Cmd_SyncVanityStruct(EquipSyncStruct)", InvokeUserCode_Cmd_SyncVanityStruct__EquipSyncStruct, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerEquipment), "System.Void PlayerEquipment::Cmd_RemoveVanity(System.String)", InvokeUserCode_Cmd_RemoveVanity__String, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerEquipment), "System.Void PlayerEquipment::Rpc_EquipSfx(EquipSfx)", InvokeUserCode_Rpc_EquipSfx__EquipSfx);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			GeneratedNetworkCode._Write_EquipSyncStruct(writer, _equipSyncStruct);
			GeneratedNetworkCode._Write_EquipSyncStruct(writer, _vanitySyncStruct);
			writer.WriteBool(_setToDisplay);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			GeneratedNetworkCode._Write_EquipSyncStruct(writer, _equipSyncStruct);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			GeneratedNetworkCode._Write_EquipSyncStruct(writer, _vanitySyncStruct);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteBool(_setToDisplay);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _equipSyncStruct, null, GeneratedNetworkCode._Read_EquipSyncStruct(reader));
			GeneratedSyncVarDeserialize(ref _vanitySyncStruct, null, GeneratedNetworkCode._Read_EquipSyncStruct(reader));
			GeneratedSyncVarDeserialize(ref _setToDisplay, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _equipSyncStruct, null, GeneratedNetworkCode._Read_EquipSyncStruct(reader));
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _vanitySyncStruct, null, GeneratedNetworkCode._Read_EquipSyncStruct(reader));
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _setToDisplay, null, reader.ReadBool());
		}
	}
}
