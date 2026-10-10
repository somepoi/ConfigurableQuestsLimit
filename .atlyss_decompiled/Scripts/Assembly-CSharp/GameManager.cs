using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager _current;

	public StatLogics _statLogics;

	[Header("Version Skill / Stat Point Resetting")]
	public string[] _versionIgnoreResetList = new string[1];

	public bool _resetAttributePointsOnNewVersion;

	public bool _resetSkillPointsOnNewVersion;

	[Header("Latency Buffering Control")]
	public LatencyBuffers[] _latencyBuffers;

	public BonusSkillSlot[] _staticBonusSkills;

	private readonly Dictionary<string, ScriptableItem> _cachedScriptableItems = new Dictionary<string, ScriptableItem>();

	private readonly Dictionary<string, ScriptableCreep> _cachedScriptableCreeps = new Dictionary<string, ScriptableCreep>();

	private readonly Dictionary<string, ScriptableQuest> _cachedScriptableQuests = new Dictionary<string, ScriptableQuest>();

	private readonly Dictionary<string, ScriptablePlayerRace> _cachedScriptableRaces = new Dictionary<string, ScriptablePlayerRace>();

	private readonly Dictionary<int, ScriptableCondition> _cachedScriptableConditions = new Dictionary<int, ScriptableCondition>();

	private readonly Dictionary<int, ScriptableConditionGroup> _cachedScriptableConditionGroups = new Dictionary<int, ScriptableConditionGroup>();

	private readonly Dictionary<int, ScriptableStatModifier> _cachedScriptableStatModifiers = new Dictionary<int, ScriptableStatModifier>();

	private readonly Dictionary<int, ScriptableCombatElement> _cachedScriptableCombatElements = new Dictionary<int, ScriptableCombatElement>();

	private readonly Dictionary<string, ScriptablePlayerBaseClass> _cachedScriptablePlayerClasses = new Dictionary<string, ScriptablePlayerBaseClass>();

	private readonly Dictionary<string, ScriptableMapData> _cachedScriptableMapDatas = new Dictionary<string, ScriptableMapData>();

	private readonly Dictionary<string, ScriptableSkill> _cachedScriptableSkills = new Dictionary<string, ScriptableSkill>();

	private List<ScriptableSkill> _foundBonusSkills = new List<ScriptableSkill>();

	private int _bonusSkillCount;

	private int _floatTextDirNum = 1;

	public static float GlobalGravity { get; } = -25.81f;

	private void OnValidate()
	{
		for (int i = 0; i < _latencyBuffers.Length; i++)
		{
			_latencyBuffers[i]._latencyBufferTag = $"PING BUFFER: {_latencyBuffers[i]._latencyThreshold.x}, {_latencyBuffers[i]._latencyThreshold.y}";
		}
	}

	private void Awake()
	{
		if (_current == null)
		{
			Object.DontDestroyOnLoad(this);
			_current = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		Cache_ScriptableAssets();
	}

	private void Cache_ScriptableAssets()
	{
		ScriptableItem[] array = Resources.LoadAll<ScriptableItem>("_ITEM/");
		for (int i = 0; i < array.Length; i++)
		{
			_cachedScriptableItems.Add(array[i]._itemName, array[i]);
		}
		ScriptableCreep[] array2 = Resources.LoadAll<ScriptableCreep>("_PREFAB/_ENTITY/_CREEP/");
		for (int j = 0; j < array2.Length; j++)
		{
			_cachedScriptableCreeps.Add(array2[j]._creepName, array2[j]);
		}
		ScriptableQuest[] array3 = Resources.LoadAll<ScriptableQuest>("");
		for (int k = 0; k < array3.Length; k++)
		{
			_cachedScriptableQuests.Add(array3[k]._questName, array3[k]);
		}
		ScriptableCondition[] array4 = Resources.LoadAll<ScriptableCondition>("");
		for (int l = 0; l < array4.Length; l++)
		{
			_cachedScriptableConditions.Add(l, array4[l]);
		}
		ScriptableConditionGroup[] array5 = Resources.LoadAll<ScriptableConditionGroup>("_GENERALCONDITIONS/_CONDITIONGROUPS/");
		for (int m = 0; m < array5.Length; m++)
		{
			_cachedScriptableConditionGroups.Add(array5[m]._conditionGroupID, array5[m]);
		}
		ScriptableStatModifier[] array6 = Resources.LoadAll<ScriptableStatModifier>("_STAT_MODIFIER/");
		for (int n = 0; n < array6.Length; n++)
		{
			_cachedScriptableStatModifiers.Add(array6[n]._modifierID, array6[n]);
		}
		ScriptablePlayerRace[] array7 = Resources.LoadAll<ScriptablePlayerRace>("_RACE/");
		for (int num = 0; num < array7.Length; num++)
		{
			_cachedScriptableRaces.Add(array7[num]._raceName, array7[num]);
		}
		ScriptableCombatElement[] array8 = Resources.LoadAll<ScriptableCombatElement>("_COMBATELEMENT/");
		for (int num2 = 0; num2 < array8.Length; num2++)
		{
			_cachedScriptableCombatElements.Add(array8[num2]._elementID, array8[num2]);
		}
		ScriptablePlayerBaseClass[] array9 = Resources.LoadAll<ScriptablePlayerBaseClass>("_CLASS/");
		for (int num3 = 0; num3 < array9.Length; num3++)
		{
			_cachedScriptablePlayerClasses.Add(array9[num3]._className, array9[num3]);
		}
		ScriptableMapData[] array10 = Resources.LoadAll<ScriptableMapData>("_MAPDATA/");
		for (int num4 = 0; num4 < array10.Length; num4++)
		{
			_cachedScriptableMapDatas.Add(array10[num4]._mapLockID, array10[num4]);
		}
		ScriptableSkill[] array11 = Resources.LoadAll<ScriptableSkill>("_SKILL/");
		for (int num5 = 0; num5 < array11.Length; num5++)
		{
			if (array11[num5]._allowAsBonusSkill)
			{
				_bonusSkillCount++;
				_foundBonusSkills.Add(array11[num5]);
			}
			_cachedScriptableSkills.Add(array11[num5]._skillName, array11[num5]);
		}
		_foundBonusSkills = (from ch in _foundBonusSkills
			orderby ch._skillUtilityType descending, ch._skillControlType
			select ch).Reverse().ToList();
		Refresh_StaticBonusSkills();
	}

	public void Refresh_StaticBonusSkills()
	{
		_staticBonusSkills = new BonusSkillSlot[_bonusSkillCount];
		for (int i = 0; i < _foundBonusSkills.Count; i++)
		{
			_staticBonusSkills[i] = new BonusSkillSlot
			{
				_bonusSkillTag = _foundBonusSkills[i]._skillName,
				_isLearned = false
			};
		}
	}

	public int HasExternalTools()
	{
		if (NetworkServer.dontListen)
		{
			return 0;
		}
		GameObject obj = GameObject.Find("ExplorerBehavior");
		GameObject gameObject = GameObject.Find("BepInEx_Manager");
		if ((bool)obj || (bool)gameObject)
		{
			return 1;
		}
		return 0;
	}

	public Vector3 CameraRelativeFlatten(Vector3 input, Vector3 localUp)
	{
		Transform transform = CameraFunction._current._mainCamera.transform;
		return Quaternion.LookRotation(-localUp, transform.forward.normalized) * Quaternion.Euler(Vector3.right * -90f) * input;
	}

	public void Shuffle_List<T>(List<T> inputList)
	{
		int num = inputList.Count;
		while (num > 1)
		{
			num--;
			int index = Random.Range(0, num + 1);
			T value = inputList[index];
			inputList[index] = inputList[num];
			inputList[num] = value;
		}
	}

	public float ConvertFloat_ToN2(float _f)
	{
		float.TryParse(_f.ToString("N2"), out var result);
		return result;
	}

	public bool ContainsUnicodeCharacter(string input)
	{
		if (SettingsManager._current._enableUnicode.isOn)
		{
			return false;
		}
		return input.Any((char c) => c > 'ÿ');
	}

	public ScriptableItem Locate_Item(string _tag)
	{
		if (string.IsNullOrWhiteSpace(_tag))
		{
			return null;
		}
		if (!_cachedScriptableItems.ContainsKey(_tag))
		{
			return null;
		}
		return _cachedScriptableItems[_tag];
	}

	[ServerCallback]
	public void Server_SpawnNetItemObject(GameObject _parentObject, ItemData _itemData, NetworkIdentity _taggedID, Vector3 _position, int _currencyAmount, Scene _scene)
	{
		if (NetworkServer.active)
		{
			GameObject obj = Object.Instantiate(AtlyssNetworkManager._current._networkObjectLibrary._networkItemObject);
			obj.transform.position = _position;
			ItemObject component = obj.GetComponent<ItemObject>();
			Net_ItemObject component2 = obj.GetComponent<Net_ItemObject>();
			if (_taggedID != null)
			{
				component2.Network_taggedID = _taggedID;
			}
			NetworkServer.Spawn(obj);
			component2.Network_currencyDropAmount = _currencyAmount;
			component2.Network_syncItemData = _itemData;
			if ((bool)_parentObject && (bool)_parentObject.GetComponent<PlayerInventory>())
			{
				_parentObject.GetComponent<PlayerInventory>()._droppedItemObjects.Add(component);
			}
			SceneManager.MoveGameObjectToScene(obj, _scene);
		}
	}

	public void Client_SpawnLocalItemObject(ItemData _itemData, NetworkIdentity _taggedID, Vector3 _position, int _currencyAmount, Transform _parentTransform, bool _disableDestroyTimer)
	{
		GameObject obj = PoolManager._current.GenerateObject(AtlyssNetworkManager._current._networkObjectLibrary._localItemObject, "", 0f);
		obj.transform.position = _position;
		ItemObject component = obj.GetComponent<ItemObject>();
		component._local_taggedID = _taggedID;
		component._currencyDropAmount = 0;
		if (_currencyAmount > 0)
		{
			component._currencyDropAmount = _currencyAmount;
		}
		else
		{
			component.Client_SetLocalItemData(_itemData);
		}
		obj.transform.SetParent(_parentTransform);
		obj.transform.SetParent(null);
		component._dontDestroyOnTimer = _disableDestroyTimer;
	}

	public ScriptableCreep Locate_Creep(string _tag)
	{
		if (!_cachedScriptableCreeps.ContainsKey(_tag))
		{
			return null;
		}
		return _cachedScriptableCreeps[_tag];
	}

	public ScriptableCondition Locate_Condition(int _id)
	{
		if (!_cachedScriptableConditions.ContainsKey(_id))
		{
			return null;
		}
		return _cachedScriptableConditions[_id];
	}

	public ScriptableConditionGroup Locate_ConditionGroup(int _id)
	{
		if (!_cachedScriptableConditionGroups.ContainsKey(_id))
		{
			return null;
		}
		return _cachedScriptableConditionGroups[_id];
	}

	public ScriptableStatModifier Locate_StatModifier(int _id)
	{
		if (!_cachedScriptableStatModifiers.ContainsKey(_id))
		{
			return null;
		}
		return _cachedScriptableStatModifiers[_id];
	}

	public ScriptablePlayerRace Locate_PlayerRace(string _tag)
	{
		if (!_cachedScriptableRaces.ContainsKey(_tag))
		{
			return null;
		}
		return _cachedScriptableRaces[_tag];
	}

	public ScriptableCombatElement Locate_CombatElement(int _id)
	{
		if (!_cachedScriptableCombatElements.ContainsKey(_id))
		{
			return null;
		}
		return _cachedScriptableCombatElements[_id];
	}

	public ScriptablePlayerBaseClass Locate_PlayerClass(string _tag)
	{
		if (!_cachedScriptablePlayerClasses.ContainsKey(_tag))
		{
			return null;
		}
		return _cachedScriptablePlayerClasses[_tag];
	}

	public ScriptableSkill Locate_Skill(string _tag)
	{
		if (!_cachedScriptableSkills.ContainsKey(_tag))
		{
			return null;
		}
		return _cachedScriptableSkills[_tag];
	}

	public ScriptableQuest Locate_Quest(string _tag)
	{
		if (!_cachedScriptableQuests.ContainsKey(_tag))
		{
			return null;
		}
		return _cachedScriptableQuests[_tag];
	}

	public ScriptableMapData Locate_MapData(string _tag)
	{
		if (!_cachedScriptableMapDatas.ContainsKey(_tag))
		{
			return null;
		}
		return _cachedScriptableMapDatas[_tag];
	}

	public FloatingValue Spawn_FloatingText(int _value, StatusEntity _entityObject, FloatTextColor _gradientIndex, float _scale, float _speed)
	{
		if (_entityObject == null)
		{
			return null;
		}
		_floatTextDirNum *= -1;
		GameObject gameObject = PoolManager._current.GenerateObject(null, "damageNumber", 0f);
		FloatingValue component = gameObject.GetComponent<FloatingValue>();
		TextMeshPro componentInChildren = gameObject.GetComponentInChildren<TextMeshPro>();
		gameObject.transform.position = _entityObject.gameObject.transform.position + Vector3.one * Random.Range(-1.75f, 1.75f) + Vector3.up * 1.25f;
		if ((bool)_entityObject._isCreep)
		{
			ScriptableCreep scriptCreep = _entityObject._isCreep._scriptCreep;
			gameObject.transform.position = _entityObject.gameObject.transform.position + scriptCreep._generalCreepVisual._targeterOffset * Vector3.up;
		}
		component._setFollowObject = _entityObject.gameObject.transform;
		component._dirNum = _floatTextDirNum;
		component._floatTextSpeed = _speed;
		Vector3 insideUnitSphere = Random.insideUnitSphere;
		insideUnitSphere.y = 0f;
		component._value = _value;
		componentInChildren.fontSize = _scale;
		componentInChildren.colorGradientPreset = component._floatGradients[(int)_gradientIndex];
		return gameObject.GetComponent<FloatingValue>();
	}
}
