using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mirror;
using UnityEngine;

public class ItemDropEntity : NetworkBehaviour
{
	[CompilerGenerated]
	private sealed class _003CInit_CoinRate_003Ed__20 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public ItemDropEntity _003C_003E4__this;

		public int _rate;

		public float _currencyPercent;

		private GameManager _003C_gm_003E5__2;

		private NetworkIdentity _003C_netID_003E5__3;

		private Vector3 _003C_spawnPos_003E5__4;

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
		public _003CInit_CoinRate_003Ed__20(int _003C_003E1__state)
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
			ItemDropEntity itemDropEntity = _003C_003E4__this;
			int num2;
			ItemData itemData;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_gm_003E5__2 = GameManager._current;
				_003C_netID_003E5__3 = Player._mainPlayer.netIdentity;
				_003C_spawnPos_003E5__4 = new Vector3(itemDropEntity.transform.position.x, itemDropEntity.transform.position.y + 1.85f, itemDropEntity.transform.position.z);
				itemDropEntity._currentCoinRate = _rate;
				if ((bool)itemDropEntity._itemDropEntity_chest)
				{
					_003C_003E2__current = new WaitForSeconds(0.4f);
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_00be;
			case 1:
				_003C_003E1__state = -1;
				goto IL_00be;
			case 2:
				{
					_003C_003E1__state = -1;
					if (itemDropEntity._currentCoinRate <= 0)
					{
						itemDropEntity._initialCurrencyDrop = true;
						return false;
					}
					goto IL_00be;
				}
				IL_00be:
				num2 = (int)((float)itemDropEntity._currencyDrop + (float)UnityEngine.Random.Range(0, 4) * _currencyPercent);
				if (num2 <= 0)
				{
					num2 = 1;
				}
				itemData = new ItemData();
				if (Player._mainPlayer._playerZoneType != ZoneType.Dungeon && Vector3.Distance(Player._mainPlayer.transform.position, itemDropEntity.transform.position) >= 75f)
				{
					_003C_netID_003E5__3 = null;
				}
				if ((bool)_003C_netID_003E5__3)
				{
					_003C_gm_003E5__2.Client_SpawnLocalItemObject(itemData, _003C_netID_003E5__3, _003C_spawnPos_003E5__4, num2, Player._mainPlayer.transform, itemDropEntity._disableDropTimers);
				}
				else
				{
					_003C_gm_003E5__2.Client_SpawnLocalItemObject(itemData, null, _003C_spawnPos_003E5__4, num2, Player._mainPlayer.transform, itemDropEntity._disableDropTimers);
				}
				itemDropEntity._currentCoinRate--;
				_003C_003E2__current = new WaitForSeconds(0.1f);
				_003C_003E1__state = 2;
				return true;
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

	private ItemDropEntity_Chest _itemDropEntity_chest;

	[Header("Loot Array")]
	public ItemDrop[] _itemDrops = new ItemDrop[1];

	public ScriptableLootTable _lootTableOverride;

	public bool _disableDropTimers;

	private readonly List<ItemDrop> _queriedItemDrops = new List<ItemDrop>();

	[Header("Item Type Limiters")]
	[SerializeField]
	private int _gearItemDropLimit = 1;

	[SerializeField]
	private int _consumableItemDropLimit = 1;

	[SerializeField]
	private int _tradeItemDropLimit = 2;

	private int _gearItemsDropped;

	private int _consumableItemsDropped;

	private int _tradeItemsDropped;

	[Header("Currency Parameters")]
	public int _currencyDrop;

	[Range(1f, 20f)]
	public int _currencyRate = 1;

	private int _currentCoinRate;

	[Space]
	public bool _dropCurrencyOnce;

	private bool _initialCurrencyDrop;

	protected override void OnValidate()
	{
		base.OnValidate();
		if ((bool)_lootTableOverride)
		{
			_currencyDrop = _lootTableOverride._currencyAmount;
		}
		for (int i = 0; i < _itemDrops.Length; i++)
		{
			if (_itemDrops[i]._dropQuantity <= 0)
			{
				_itemDrops[i]._dropQuantity = 1;
			}
			if (_itemDrops[i]._dropChance <= 0f)
			{
				_itemDrops[i]._dropChance = 0.001f;
			}
			if (_itemDrops[i]._item == null)
			{
				_itemDrops[i]._itemTag = "Empty Item Drop Slot";
			}
			else
			{
				_itemDrops[i]._itemTag = _itemDrops[i]._item._itemName;
			}
		}
	}

	private void Awake()
	{
		_itemDropEntity_chest = GetComponent<ItemDropEntity_Chest>();
	}

	[ClientCallback]
	public void Interactable_DropItems()
	{
		if (NetworkClient.active && (bool)Player._mainPlayer && (!Player._mainPlayer._isHostPlayer || !(Player._mainPlayer.gameObject.scene != base.gameObject.scene)))
		{
			Client_DropItems(Player._mainPlayer.netIdentity);
			Client_DropCurrency(_currencyRate, 1f);
		}
	}

	[ClientCallback]
	public void Client_DropCurrency(int _setRate, float _currencyPercent)
	{
		if (NetworkClient.active && (bool)Player._mainPlayer && (!Player._mainPlayer._isHostPlayer || !(Player._mainPlayer.gameObject.scene != base.gameObject.scene)) && _currencyDrop > 0 && (!_dropCurrencyOnce || !_initialCurrencyDrop))
		{
			StartCoroutine(Init_CoinRate(_setRate, _currencyPercent));
		}
	}

	[IteratorStateMachine(typeof(_003CInit_CoinRate_003Ed__20))]
	[ClientCallback]
	private IEnumerator Init_CoinRate(int _rate, float _currencyPercent)
	{
		if (!NetworkClient.active)
		{
			return null;
		}
		return new _003CInit_CoinRate_003Ed__20(0)
		{
			_003C_003E4__this = this,
			_rate = _rate,
			_currencyPercent = _currencyPercent
		};
	}

	[ClientCallback]
	public void Client_DropItems(NetworkIdentity _netIDTag)
	{
		if (!NetworkClient.active || !GameManager._current || !Player._mainPlayer || (Player._mainPlayer._isHostPlayer && Player._mainPlayer.gameObject.scene != base.gameObject.scene))
		{
			return;
		}
		ItemDrop[] itemDrops = _itemDrops;
		if ((bool)_lootTableOverride)
		{
			itemDrops = _lootTableOverride._itemDrops;
		}
		_queriedItemDrops.Clear();
		_queriedItemDrops.AddRange(itemDrops);
		GameManager._current.Shuffle_List(_queriedItemDrops);
		GameManager current = GameManager._current;
		Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y + 1.85f, base.transform.position.z);
		_gearItemsDropped = 0;
		_consumableItemsDropped = 0;
		_tradeItemsDropped = 0;
		for (int i = 0; i < _queriedItemDrops.Count; i++)
		{
			if (UnityEngine.Random.Range(0f, 1f) > _queriedItemDrops[i]._dropChance || _queriedItemDrops[i]._item == null || (_queriedItemDrops[i]._useEventMonth && DateTime.Now.Month != _queriedItemDrops[i]._eventMonth) || ((bool)_queriedItemDrops[i]._questRequirement && !Player._mainPlayer._pQuest.Check_HasQuest(_queriedItemDrops[i]._questRequirement)) || (_queriedItemDrops[i]._checkIfPlayerHasItem && Player._mainPlayer._pInventory.Fetch_HeldItem(_queriedItemDrops[i]._item) != null))
			{
				continue;
			}
			ItemData itemData = new ItemData();
			itemData._itemName = _queriedItemDrops[i]._item._itemName;
			switch (_queriedItemDrops[i]._item._itemType)
			{
			case ItemType.GEAR:
			{
				if (_gearItemsDropped >= _gearItemDropLimit || _consumableItemsDropped > 0 || _tradeItemsDropped > 0)
				{
					continue;
				}
				ScriptableEquipment scriptableEquipment = (ScriptableEquipment)_queriedItemDrops[i]._item;
				itemData._modifierID = 0;
				itemData._quantity = 1;
				if ((bool)scriptableEquipment._statModifierTable)
				{
					for (int k = 0; k < scriptableEquipment._statModifierTable._statModifierSlots.Length; k++)
					{
						if (UnityEngine.Random.Range(0f, 1f) <= scriptableEquipment._statModifierTable._statModifierSlots[k]._modifierChance)
						{
							itemData._modifierID = scriptableEquipment._statModifierTable._statModifierSlots[k]._equipModifier._modifierID;
						}
					}
				}
				_gearItemsDropped++;
				break;
			}
			case ItemType.CONSUMABLE:
				if (_consumableItemsDropped >= _consumableItemDropLimit || _gearItemsDropped > 0)
				{
					continue;
				}
				if (_queriedItemDrops[i]._item.GetType() == typeof(ScriptableSkillScroll))
				{
					ScriptableSkillScroll scriptableSkillScroll = (ScriptableSkillScroll)_queriedItemDrops[i]._item;
					bool flag = false;
					for (int j = 0; j < Player._mainPlayer._pCasting._bonusSkillSlots.Length; j++)
					{
						if (flag)
						{
							break;
						}
						BonusSkillSlot bonusSkillSlot = Player._mainPlayer._pCasting._bonusSkillSlots[j];
						flag = bonusSkillSlot._bonusSkillTag == scriptableSkillScroll._scriptableSkill._skillName && bonusSkillSlot._isLearned;
					}
					if (flag && AtlyssNetworkManager._current._soloMode)
					{
						continue;
					}
				}
				itemData._maxQuantity = _queriedItemDrops[i]._item._maxStackAmount;
				itemData._quantity = UnityEngine.Random.Range(1, _queriedItemDrops[i]._dropQuantity);
				_consumableItemsDropped++;
				break;
			case ItemType.TRADE:
				if (_tradeItemsDropped >= _tradeItemDropLimit || _gearItemsDropped > 0)
				{
					continue;
				}
				itemData._maxQuantity = _queriedItemDrops[i]._item._maxStackAmount;
				itemData._quantity = UnityEngine.Random.Range(1, _queriedItemDrops[i]._dropQuantity);
				_tradeItemsDropped++;
				break;
			}
			if ((bool)_netIDTag)
			{
				current.Client_SpawnLocalItemObject(itemData, _netIDTag, position, 0, Player._mainPlayer.transform, _disableDropTimers);
			}
			else
			{
				current.Client_SpawnLocalItemObject(itemData, null, position, 0, Player._mainPlayer.transform, _disableDropTimers);
			}
		}
	}

	public override bool Weaved()
	{
		return true;
	}
}
