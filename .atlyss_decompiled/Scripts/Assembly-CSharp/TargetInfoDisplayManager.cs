using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetInfoDisplayManager : MonoBehaviour
{
	public static TargetInfoDisplayManager _current;

	private Creep _cachedCreepTarget;

	private CreepBehavior_StandardCreep _cachedStandardCreep;

	[Header("Target Info Display")]
	public StatusEntity _setStatusEntityTarget;

	[SerializeField]
	private Transform _conditionIconContent;

	[SerializeField]
	private GameObject _conditionIconPrefab;

	public List<ConditionListDataEntry> _conditionDataEntries = new List<ConditionListDataEntry>();

	public List<ConditionData> _cachedConditionData = new List<ConditionData>();

	[Space]
	[SerializeField]
	private CanvasGroup _displayGroup;

	[SerializeField]
	private CanvasGroup _targetElementIconGroup;

	[Space]
	[SerializeField]
	private Text _targetNameField;

	[SerializeField]
	private Text _targetLevelCounter;

	[SerializeField]
	private Text _targetDefenseCounter;

	[SerializeField]
	private Text _targetMagicDefenseCounter;

	[SerializeField]
	private Color _lowDefColor = Color.red;

	[SerializeField]
	private Image _targetHealthFill;

	[SerializeField]
	private Image _targetHealthGhostFill;

	[SerializeField]
	private Image _targetElementIcon;

	[SerializeField]
	private GameObject _skullIcon;

	[SerializeField]
	private GameObject _eliteBackdrop;

	[SerializeField]
	private Image _eliteIcon;

	private float _healthFillAmount;

	private float _ghostHealthFillAmount;

	[Space]
	[SerializeField]
	private MenuElement _targetCastBarElement;

	[SerializeField]
	private Image _targetCastTimeFill;

	[SerializeField]
	private Text _targetCastNameText;

	private void Awake()
	{
		_current = this;
		_displayGroup.alpha = 0f;
	}

	private void LateUpdate()
	{
		Handle_TargetInfoDisplay();
	}

	private void Handle_TargetInfoDisplay()
	{
		if (!Player._mainPlayer)
		{
			return;
		}
		if (!_setStatusEntityTarget || Player._mainPlayer._currentPlayerCondition != PlayerCondition.ACTIVE)
		{
			_setStatusEntityTarget = null;
			_displayGroup.alpha = 0f;
			Clear_ConditionGrid();
			_cachedConditionData.Clear();
			return;
		}
		_displayGroup.alpha = 1f;
		_targetCastBarElement.isEnabled = (bool)_cachedStandardCreep && _cachedStandardCreep._castTime > 0f;
		if ((bool)_setStatusEntityTarget._isPlayer)
		{
			Handle_PlayerInfoDisplay(_setStatusEntityTarget._isPlayer);
		}
		else if ((bool)_setStatusEntityTarget._isCreep)
		{
			Handle_CreepInfoDisplay(_setStatusEntityTarget._isCreep);
		}
		for (int i = 0; i < _conditionDataEntries.Count; i++)
		{
			if ((bool)_conditionDataEntries[i]._conditionData._conditionEffectObject && (_conditionDataEntries[i]._conditionData._currentDuration <= 0f || !_conditionDataEntries[i]._conditionData._conditionEffectObject.activeInHierarchy))
			{
				Clear_ConditionGrid();
				_cachedConditionData.Clear();
				return;
			}
		}
		for (int j = 0; j < _setStatusEntityTarget._conditionDatas.Count; j++)
		{
			ConditionData conditionData = _setStatusEntityTarget._conditionDatas[j];
			if (!_cachedConditionData.Contains(conditionData))
			{
				_cachedConditionData.Add(conditionData);
				Add_ConditionListEntry(conditionData);
			}
		}
		void Handle_CreepInfoDisplay(Creep _c)
		{
			if ((bool)_c)
			{
				PlayerStats pStats = Player._mainPlayer._pStats;
				if (_cachedCreepTarget != null && _c != _cachedCreepTarget)
				{
					Clear_ConditionGrid();
					_cachedConditionData.Clear();
				}
				_eliteBackdrop.SetActive(_c._scriptCreep._isElite);
				_eliteIcon.enabled = _c._scriptCreep._isElite;
				int num = _c._statStruct._defense;
				int num2 = _c._statStruct._magicDefense;
				if (num < 0)
				{
					num = 0;
				}
				if (num2 < 0)
				{
					num2 = 0;
				}
				_cachedCreepTarget = _c;
				if (_cachedCreepTarget._creepBehavior.GetType() == typeof(CreepBehavior_StandardCreep))
				{
					_cachedStandardCreep = (CreepBehavior_StandardCreep)_cachedCreepTarget._creepBehavior;
				}
				if ((bool)_cachedStandardCreep && (bool)_cachedStandardCreep._currentCastSkill)
				{
					_targetCastNameText.text = _cachedStandardCreep._currentCastSkill._skillName;
					float num3 = _cachedStandardCreep._currentCastSkill._skillRankParams._baseCastTime + _cachedStandardCreep._setCastTimeModifier;
					if (_targetCastTimeFill.fillAmount >= 0.85f)
					{
						_targetCastTimeFill.color = Color.Lerp(_targetCastTimeFill.color, Color.yellow, 9.6f * Time.deltaTime);
					}
					else
					{
						_targetCastTimeFill.color = Color.green;
					}
					_targetCastTimeFill.fillAmount = _cachedStandardCreep._castTime / num3;
				}
				_targetNameField.text = _c._creepDisplayName ?? "";
				_targetDefenseCounter.text = $"{num}";
				_targetMagicDefenseCounter.text = $"{num2}";
				if (_c._statStruct._defense < _c._scriptCreep._creepStatStruct._defense)
				{
					_targetDefenseCounter.color = _lowDefColor;
				}
				else
				{
					_targetDefenseCounter.color = Color.white;
				}
				if (_c._statStruct._magicDefense < _c._scriptCreep._creepStatStruct._magicDefense)
				{
					_targetMagicDefenseCounter.color = _lowDefColor;
				}
				else
				{
					_targetMagicDefenseCounter.color = Color.white;
				}
				int num4 = _c._creepLevel - pStats._currentLevel;
				if (num4 == -3)
				{
					_targetLevelCounter.text = $"<color=#afff00>{_c._creepLevel}</color>";
				}
				else if (num4 == -4 || num4 == -5)
				{
					_targetLevelCounter.text = $"<color=lime>{_c._creepLevel}</color>";
				}
				else if (num4 <= -6)
				{
					_targetLevelCounter.text = $"<color=grey>{_c._creepLevel}</color>";
				}
				if (num4 > -3)
				{
					_targetLevelCounter.text = $"<color=yellow>{_c._creepLevel}</color>";
				}
				if (num4 == 3)
				{
					_targetLevelCounter.text = $"<color=orange>{_c._creepLevel}</color>";
				}
				else if (num4 == 4 || num4 == 5)
				{
					_targetLevelCounter.text = $"<color=red>{_c._creepLevel}</color>";
				}
				else if (num4 >= 6)
				{
					_targetLevelCounter.text = "";
				}
				_skullIcon.SetActive(num4 >= 6);
				if (_c._combatElement == null)
				{
					_targetElementIcon.sprite = GameManager._current._statLogics._defaultCombatElement._elementIcon;
				}
				else
				{
					_targetElementIcon.sprite = _c._combatElement._elementIcon;
				}
				_targetElementIconGroup.alpha = 1f;
				_healthFillAmount = (float)_setStatusEntityTarget._currentHealth / (float)_c._statStruct._maxHealth;
				_targetHealthFill.fillAmount = _healthFillAmount;
				_ghostHealthFillAmount += ((float)_setStatusEntityTarget._currentHealth / (float)_c._statStruct._maxHealth - _ghostHealthFillAmount) * 0.128f;
				if (float.IsNaN(_ghostHealthFillAmount))
				{
					_ghostHealthFillAmount = 0f;
				}
				_targetHealthGhostFill.fillAmount = _ghostHealthFillAmount;
			}
		}
		void Handle_PlayerInfoDisplay(Player _p)
		{
			PlayerStats pStats = _p._pStats;
			_targetNameField.text = _setStatusEntityTarget._isPlayer._nickname ?? "";
			_targetLevelCounter.text = $"{pStats._currentLevel}";
			_targetElementIconGroup.alpha = 0f;
			_healthFillAmount = (float)_setStatusEntityTarget._currentHealth / (float)pStats._statStruct._maxHealth;
			_targetHealthFill.fillAmount = _healthFillAmount;
			_ghostHealthFillAmount += ((float)_setStatusEntityTarget._currentHealth / (float)pStats._statStruct._maxHealth - _ghostHealthFillAmount) * 0.045f;
			if (float.IsNaN(_ghostHealthFillAmount))
			{
				_ghostHealthFillAmount = 0f;
			}
			_targetHealthGhostFill.fillAmount = _ghostHealthFillAmount;
		}
	}

	public void Add_ConditionListEntry(ConditionData _conditionData)
	{
		GameObject obj = Object.Instantiate(_conditionIconPrefab);
		obj.transform.SetParent(_conditionIconContent);
		obj.transform.position = Vector3.zero;
		ConditionListDataEntry component = obj.GetComponent<ConditionListDataEntry>();
		component._conditionData = _conditionData;
		_conditionDataEntries.Add(component);
	}

	public void Clear_ConditionGrid()
	{
		for (int i = 0; i < _conditionDataEntries.Count; i++)
		{
			Object.Destroy(_conditionDataEntries[i].gameObject);
		}
		_conditionDataEntries.Clear();
	}
}
