using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
	private Player _mainPlayer;

	private PlayerQuesting _pQuest;

	private DialogManager _dialogManager;

	[Header("Actor Object")]
	[SerializeField]
	private Transform actorObject;

	[SerializeField]
	private Transform _cameraFocusTransform;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private string _initAnimation = string.Empty;

	[SerializeField]
	private bool _localFacePlayer;

	private Quaternion _originRotation;

	[Header("Nametag")]
	[SerializeField]
	private Renderer _nameTagRenderer;

	[SerializeField]
	private Renderer _subNameTagRenderer;

	[Header("Marker Icon")]
	[SerializeField]
	private GameObject _markerIconObject;

	[SerializeField]
	private SpriteRenderer _markerIconRenderer;

	[SerializeField]
	private Sprite _uniqueMarkerSprite;

	[Header("Trigger Settings")]
	public ScriptableMapData _worldPortalMapData;

	public bool _triggerActive;

	[Header("Dialog")]
	public ScriptableDialogData _scriptDialogData;

	public AudioSource _voxAsrc;

	[Space]
	public DialogBranch _localDialogBranch;

	public bool _useLocalDialogBranch;

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "dialog_ico", allowScaling: true);
	}

	private void Start()
	{
		if (!string.IsNullOrEmpty(_initAnimation))
		{
			_animator.CrossFade(_initAnimation, 0f, 0);
		}
		actorObject = base.transform.parent;
		if ((bool)actorObject)
		{
			_originRotation = actorObject.rotation;
		}
		if ((bool)GetComponentInParent<WorldPortalWaypoint>())
		{
			_worldPortalMapData = GetComponentInParent<WorldPortalWaypoint>()._scriptMapData;
		}
	}

	private void Update()
	{
		if ((bool)Player._mainPlayer)
		{
			_mainPlayer = Player._mainPlayer;
			_pQuest = _mainPlayer._pQuest;
		}
		if ((bool)DialogManager._current)
		{
			_dialogManager = DialogManager._current;
		}
		if ((bool)_mainPlayer && (bool)_dialogManager)
		{
			Handle_ActiveTriggerBehavior();
			Handle_TriggerMarkerIcon();
			Handle_LocalActorRotation();
			Handle_NametagDistance();
		}
	}

	private void Handle_LocalActorRotation()
	{
		if (_localFacePlayer && (bool)actorObject)
		{
			if (_dialogManager._currentDialogTrigger == this)
			{
				Vector3 forward = Player._mainPlayer.transform.position - base.transform.position;
				forward.y = 0f;
				actorObject.rotation = Quaternion.Lerp(actorObject.rotation, Quaternion.LookRotation(forward), 12.5f * Time.deltaTime);
			}
			else
			{
				actorObject.rotation = Quaternion.Lerp(actorObject.rotation, _originRotation, 12.5f * Time.deltaTime);
			}
		}
	}

	private void Handle_NametagDistance()
	{
		PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
		Vector3 direction = _mainPlayer.transform.position - base.transform.position;
		float num = Vector3.Distance(_mainPlayer.transform.position, base.transform.position);
		bool flag = false;
		if (physicsScene.Raycast(base.transform.position + Vector3.up * 2.5f, direction, out var hitInfo, float.PositiveInfinity, CastLayers.PathfinderMask) && hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			flag = true;
		}
		bool flag2 = num <= 20.5f && !flag;
		if ((bool)_nameTagRenderer)
		{
			_nameTagRenderer.enabled = flag2;
		}
		if ((bool)_subNameTagRenderer)
		{
			_subNameTagRenderer.enabled = flag2;
		}
	}

	private void Handle_TriggerMarkerIcon()
	{
		if (!_mainPlayer || !_markerIconObject)
		{
			return;
		}
		if (_mainPlayer._isHostPlayer && _mainPlayer.gameObject.scene != base.gameObject.scene)
		{
			_markerIconObject.SetActive(value: false);
			return;
		}
		_markerIconObject.SetActive(_scriptDialogData);
		if (_markerIconRenderer.sprite != null)
		{
			_markerIconRenderer.sprite = null;
		}
		if (!_scriptDialogData)
		{
			return;
		}
		if (_scriptDialogData._scriptableQuests != null)
		{
			for (int i = 0; i < _scriptDialogData._scriptableQuests.Length; i++)
			{
				for (int j = 0; j < _pQuest._questProgressData.Count; j++)
				{
					if (_pQuest._questProgressData[j]._questTag == _scriptDialogData._scriptableQuests[i]._questName && _pQuest._questProgressData[j]._questComplete)
					{
						_markerIconRenderer.sprite = GameManager._current._statLogics._questQuestionIcon;
						return;
					}
				}
			}
		}
		if (SettingsManager._current._hideNpcHeadIcons.isOn)
		{
			return;
		}
		if ((bool)_scriptDialogData._scriptableShopkeep)
		{
			if (_scriptDialogData._scriptableShopkeep._isGambler)
			{
				_markerIconRenderer.sprite = GameManager._current._statLogics._gambleIcon;
			}
			else
			{
				_markerIconRenderer.sprite = GameManager._current._statLogics._merchantIcon;
			}
		}
		if ((bool)_uniqueMarkerSprite)
		{
			_markerIconRenderer.sprite = _uniqueMarkerSprite;
		}
	}

	private void Handle_ActiveTriggerBehavior()
	{
		if (!_triggerActive)
		{
			return;
		}
		Vector3 forward = base.transform.position - _mainPlayer.transform.position;
		forward.y = 0f;
		Quaternion b = Quaternion.LookRotation(forward);
		_mainPlayer.transform.rotation = Quaternion.Slerp(_mainPlayer.transform.rotation, b, 6.45f * Time.deltaTime);
		if ((bool)_cameraFocusTransform)
		{
			CameraFunction._current._overridePosition = _cameraFocusTransform.position;
			CameraFunction._current._overrideRotation = _cameraFocusTransform.eulerAngles;
			if (!IsInvoking("Init_CameraPosOverride") && !CameraFunction._current._initCameraOverride)
			{
				Invoke("Init_CameraPosOverride", 0.4f);
			}
		}
		if ((bool)_worldPortalMapData)
		{
			WorldPortalManager._current._setMapData = _worldPortalMapData;
		}
	}

	public void BeginDialog(ScriptableDialogData _dialogData)
	{
		DialogBranch dialogBranch = null;
		_dialogManager._scriptableDialog = _dialogData;
		if (_dialogData._dialogBranches != null && _dialogData._dialogBranches.Length != 0)
		{
			dialogBranch = _dialogData._dialogBranches[0];
		}
		if (_useLocalDialogBranch)
		{
			dialogBranch = _localDialogBranch;
		}
		if (DialogManager._current._dialogIntroductionTags.Contains(_dialogData._nameTag))
		{
			_dialogManager.Start_Dialog(dialogBranch, this);
		}
		else if (_dialogData._introductionBranches != null && _dialogData._introductionBranches.Length != 0)
		{
			dialogBranch = _dialogData._introductionBranches[0];
			_dialogManager.Start_Dialog(dialogBranch, this);
			DialogManager._current._dialogIntroductionTags.Add(_dialogData._nameTag);
		}
		else
		{
			_dialogManager.Start_Dialog(dialogBranch, this);
		}
		_triggerActive = true;
	}

	private void Init_CameraPosOverride()
	{
		CameraFunction._current._initCameraOverride = true;
	}
}
