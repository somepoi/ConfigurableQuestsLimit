using UnityEngine;

[CreateAssetMenu(fileName = "creep_", menuName = "ScriptableObjects/Entity/Creep", order = 1)]
public class ScriptableCreep : ScriptableObject
{
	[Header("Creep Info")]
	public string _creepName = "New Creep";

	public GameObject _creepObject;

	[Header("Creep Stats")]
	public int _creepLevel = 1;

	public bool _unkillable;

	public bool _isElite;

	[Space]
	public StatStruct _creepStatStruct;

	[Header("Combat")]
	public DamageType _creepAttribute;

	public ScriptableCombatElement _combatElement;

	public int _baseDamage = 1;

	public ConditionSlot _conditionSlot;

	[Space]
	public float _parryStunMod;

	public bool _canParry = true;

	public bool _immuneToKnockback;

	public bool _immuneToHitstun;

	[Header("Aggro / Threat")]
	public CreepAssistType _creepAssistType;

	public bool _canAggro = true;

	public bool _ignoreHighLevelPlayer = true;

	public bool _dontFollowAggroTarget;

	public bool _playMapInstanceActionMusic;

	[Header("Item Drops")]
	public int _currencyDropBonus;

	public ItemDrop[] _itemDrops = new ItemDrop[0];

	[Header("Movement / Pathfinding")]
	public PathfindingParams _pathfindParams;

	[Header("Visual / Audio")]
	public GeneralCreepVisual _generalCreepVisual;

	public GeneralCreepAudio _generalCreepAudio;

	private void OnValidate()
	{
		if (_creepLevel <= 0)
		{
			_creepLevel = 1;
		}
		if (_baseDamage <= 0)
		{
			_baseDamage = 1;
		}
		if (_currencyDropBonus < 0)
		{
			_currencyDropBonus = 0;
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
				_itemDrops[i]._itemTag = $"{_itemDrops[i]._item._itemName} ({_itemDrops[i]._dropChance * 100f}%)";
			}
		}
	}
}
