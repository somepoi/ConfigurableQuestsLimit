using UnityEngine;

public class StatLogics : MonoBehaviour
{
	[Header("Player Default Names")]
	public string[] _defaultNames;

	[Header("Player Class Params")]
	public string _emptyClassName = "Novice";

	public Sprite _emptyClassEmblem;

	public ScriptableSkill[] _generalSkills;

	public ScriptableWeapon[] _startingWeapons = new ScriptableWeapon[0];

	[Space]
	public int _baseClassLevelRequirement = 10;

	public int _tierClassLevelRequirement = 30;

	[Header("Soulbound Skill Params")]
	public ScriptableSkill _recallSkill;

	public ScriptableSkill _reviveSkill;

	public ScriptableItem _soulPearl;

	[Header("Friendly NPC Params")]
	public Sprite _questQuestionIcon;

	public Sprite _merchantIcon;

	public Sprite _gambleIcon;

	[Header("Global Movement Params")]
	public float _baseMoveSpeed = 25f;

	public float _maxRevUpMoveSpeed = 8.15f;

	public float _dashIFrameTime = 0.65f;

	[Header("Netplay / Party Conditions")]
	[Tooltip("Radius to check how far members are to recieve exp points.")]
	public float _partyExperienceProximity = 100.5f;

	[Tooltip("Radius to check how far members are to recieve quest progress points.")]
	public float _partyQuestPointProximity = 100.5f;

	[Tooltip("The quest required to be able to teleport to the party leader's map instance.")]
	public ScriptableQuest _partyLeaderTeleportRequirement;

	[Tooltip("The cooldown after successfully teleporting to the party leader via client -> server request.")]
	public float _teleportToLeaderCooldown = 30f;

	[Header("Creeps")]
	[Tooltip("% increase of creep max health stat for each player within the parent spawner radius.")]
	public float _creepMaxHealthModifier = 0.28f;

	[Tooltip("% increase of creep experience gain stat for each player within the parent spawner radius.")]
	public float _creepExperienceModifier = 0.15f;

	[Space]
	[Tooltip("The time before the creep aggros while being interested (?) in the potential target..")]
	public float _awareToAggroTime = 0.46f;

	[Tooltip("The chance of awareness.")]
	public float _awarenessChance = 0.72f;

	public float _awarenessRadius = 45.5f;

	public float _lineOfSightRadius = 35.5f;

	public float _aggroLength = 45.5f;

	public Vector3 _aggroCapsuleCenter = new Vector3(0f, 3f, 20f);

	public Vector3 _creepRigidbodyCenter = new Vector3(0f, 2f, 0f);

	[Space]
	public float _creepGravityScale = 0.385f;

	[Space]
	public float _creepHeightJumpForce = 125f;

	public float _creepHeightJumpForwardForce = 45.5f;

	[Space]
	public float _creepLedgeJumpForce = 58f;

	public float _creepLedgeJumpForwardForce = 20.85f;

	[Header("Elements")]
	public ScriptableCombatElement _defaultCombatElement;

	public float _elementResistancePercent = 0.65f;

	public float _creepResistModifierPercent = 0.4f;

	[Header("Stat Bases / Growth Modifiers")]
	public int _maxMainLevel = 100;

	public float _experienceBonusModifier = 0.048f;

	public AnimationCurve _experienceCurve;

	public AnimationCurve _experienceGainCurve;

	[Space]
	public AnimationCurve _maxHealthCurve;

	public AnimationCurve _maxManaCurve;

	public AnimationCurve _maxStaminaCurve;

	[Space]
	public float _baseEvasion = 0.0125f;

	[Header("Stat Regeneration Values")]
	[Range(0f, 1f)]
	public float _sitRestHealPercent = 0.06f;

	[Space]
	public float _timeBeforeManaRegen = 7.5f;

	public float _baseManaRegenRate = 3.25f;

	[Space]
	public float _timeBeforeStaminaRegen = 3f;

	public float _baseStaminaRegenRate = 0.65f;

	[Header("Misc Combat Params")]
	[Tooltip("Base time of how long the parried entity stays stunned. Modified with Fighter's Bulwark passive.")]
	public float _baseParryApplyBuffer = 1.6f;

	[Tooltip("The deep rooted IFrames after getting hit by a creep within the status entity class (Take_Damage) function.")]
	public float _pvePlayerHitBuffer = 0.3f;

	[Tooltip("The deep rooted IFrames after getting hit by another player within the status entity class (Take_Damage) function.")]
	public float _pvpPlayerHitBuffer = 0.25f;

	[Header("Base Stat Power Percentages")]
	public float _scale_weaponFloor = 1.45f;

	public float _scale_weaponCeiling = 0.005f;

	[Space]
	[Tooltip("Percentage of the overall effectiveness of hitting things unarmed. May be adjusted in the future.")]
	[Range(0f, 1f)]
	public float _unarmedAttackPercent = 0.12f;

	[Tooltip("Percentage of the overall effectiveness of power stats (Attack power, dex power, magic power).")]
	[Range(0f, 1f)]
	public float _powerStatPercent = 0.36f;

	[Tooltip("Percentage of the overall damage output given in pvp against other player characters. Adjust to dictate survivability.")]
	[Range(0f, 1f)]
	public float _pvpOutputPercent = 0.68f;

	[Header("Defensive Values")]
	[Tooltip("Percentage of overall defense dampening against incoming damage.")]
	[Range(0f, 1f)]
	public float _defensePercent = 0.42f;

	[Tooltip("Percentage bonus of the defense dampening against incoming damage from weapons (not skills).")]
	[Range(0f, 1f)]
	public float _weaponDefenseBonus = 0.12f;

	[Space]
	public float _blockStaminaReductionRate = 0.3f;

	public float _baseParryChanceBuffer = 0.3f;

	[Header("Skill / Attribute Point")]
	public float _skillGlobalCooldown = 1f;

	public float _lowestCooldownValue = 0.6f;

	[Space]
	public int _attributePointsEachLevel = 5;

	public int _skillPointsEachLevel = 3;

	public ScriptableStatAttribute[] _statAttributes;

	[Space]
	public ScriptableProfession[] _scriptableProfessions;

	[Header("Inventory / Currency")]
	public int _maxNetDroppedNetItems = 12;

	public MaxInventorySlotStruct _maxInventorySlotStruct;

	[Space]
	public string _currencyName = "Currency";

	public Sprite _currencyIcon;

	public int _maxCurrency = 99999;

	[Range(0f, 1f)]
	public float _vendorBuybackPercent = 0.56f;

	public ScriptableItem _transmogCurrency;

	public AnimationCurve _currencyDropCurve;

	[Space]
	public Color[] _itemRarityColors = new Color[3];

	public Color[] _itemRarityGlowColors = new Color[3];

	[Header("Armor Dyes")]
	public ScriptableArmorDye _defaultArmorDye;

	public ScriptableArmorDye[] _armorDyes = new ScriptableArmorDye[4];
}
