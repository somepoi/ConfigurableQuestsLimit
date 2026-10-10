using System.Collections;
using MilkShake;
using UnityEngine;

public class CameraFunction : MonoBehaviour
{
	public static CameraFunction _current;

	public Shaker MyShaker;

	public ShakePreset ShakePreset;

	public ShakePreset _verticalShakePreset;

	[Header("Cameras")]
	public Camera _mainCamera;

	public Camera _UICamera;

	public Camera _fieldMinimapCamera;

	[Header("Transform Key")]
	public Transform _CameraFollowObj;

	public Vector3 positionAdjust = Vector3.zero;

	[Space]
	public Vector3 _overridePosition = Vector3.zero;

	public Vector3 _overrideRotation = Vector3.zero;

	public bool _initCameraOverride;

	[Header("Control Values")]
	public float step;

	public float _clampAngleUp = 80f;

	public float _clampAngleDown = 50f;

	public float inputSensitivity;

	private float finalInputX;

	private float finalInputZ;

	public float _RotX;

	public float _RotY;

	private Quaternion localRotation;

	public bool _unlockedCamera;

	private float _snapLoadCameraBuffer;

	private bool _isLerpingReset;

	private float _cachedEuler;

	private void Awake()
	{
		if (_current == null)
		{
			_current = this;
			Vector3 eulerAngles = base.transform.localRotation.eulerAngles;
			_RotY = eulerAngles.y;
			_RotX = eulerAngles.x;
			_fieldMinimapCamera.transform.SetParent(null);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		if (_initCameraOverride && !DialogManager._current._isDialogEnabled)
		{
			_initCameraOverride = false;
		}
		if (!_CameraFollowObj)
		{
			base.transform.rotation = Quaternion.Euler(Vector3.zero);
		}
		_UICamera.fieldOfView = _mainCamera.fieldOfView;
		if (!Player._mainPlayer)
		{
			_mainCamera.enabled = false;
			_UICamera.enabled = false;
			_fieldMinimapCamera.enabled = false;
			if (InputControlManager.current._setGamepadType == GamepadType.Keyboard)
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
			else if (InputControlManager.current._gamepadMouseCursorBuffer > 0f && !Input.GetMouseButton(1))
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
			else
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
		}
		else
		{
			_CameraFollowObj = Player._mainPlayer.transform;
			_fieldMinimapCamera.gameObject.SetActive(Player._mainPlayer._playerZoneType != ZoneType.Dungeon);
			if (Player._mainPlayer._currentGameCondition == GameCondition.IN_GAME)
			{
				_mainCamera.enabled = true;
				_UICamera.enabled = true;
				_fieldMinimapCamera.enabled = true;
				Handle_CameraControl();
			}
			else
			{
				CameraReset();
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = InputControlManager.current._setGamepadType == GamepadType.Keyboard;
			}
		}
	}

	private void Handle_CameraControl()
	{
		bool flag = Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(InputControlManager.current._hostConsole) || Input.GetKeyDown(InputControlManager.current._tabMenu);
		if ((bool)TabMenu._current._itemCell._heldItemListDataEntry)
		{
			flag = false;
			if (InputControlManager.current._setGamepadType != GamepadType.Keyboard && InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
			{
				Cursor.lockState = CursorLockMode.Locked;
				return;
			}
		}
		if (flag)
		{
			_unlockedCamera = !_unlockedCamera;
		}
		if (_unlockedCamera)
		{
			if (!Player._mainPlayer._pTargeting._targetedEntity && !DialogManager._current._isDialogEnabled)
			{
				if (InputControlManager.current._setGamepadType == GamepadType.Keyboard)
				{
					Cursor.visible = true;
				}
				else
				{
					if (!Cursor.visible && InputControlManager.current._gamepadMouseCursorBuffer > 0f)
					{
						ToolTipManager._current.Disable_AllTooltips();
					}
					Cursor.visible = InputControlManager.current._gamepadMouseCursorBuffer > 0f;
				}
				if (Cursor.visible)
				{
					Cursor.lockState = CursorLockMode.None;
				}
				else
				{
					Cursor.lockState = CursorLockMode.Locked;
				}
			}
		}
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			if (Input.GetKeyDown(InputControlManager.current._resetCamera) || (GamepadInput.GetButton(GamepadButton.RightStick) && !Player._mainPlayer._pTargeting._foundEntity))
			{
				CameraReset_Lerp();
			}
		}
		if ((bool)Player._mainPlayer._pTargeting._targetedEntity)
		{
			if (TabMenu._current._isOpen)
			{
				_unlockedCamera = false;
			}
			else
			{
				_unlockedCamera = true;
			}
			return;
		}
		if (InGameUI._current._escapeMenuBuffer > 0f)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			_unlockedCamera = false;
		}
		if (TabMenu._current._isOpen)
		{
			_unlockedCamera = true;
		}
		else if (Input.GetKey(InputControlManager.current._tabMenu))
		{
			_unlockedCamera = false;
		}
		if (Player._mainPlayer._inChat)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			_unlockedCamera = true;
		}
		if (_initCameraOverride)
		{
			base.transform.SetPositionAndRotation(Vector3.Lerp(base.transform.position, _overridePosition, 3f * Time.deltaTime), Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(_overrideRotation), 3f * Time.deltaTime));
			CameraCollision._current.maxDistance = Mathf.Lerp(CameraCollision._current.maxDistance, 30f, 3f * Time.deltaTime);
		}
		finalInputX = InputControlManager.current._altHoriz_input;
		finalInputZ = InputControlManager.current._altVert_input;
		if (_unlockedCamera)
		{
			finalInputX = 0f;
			finalInputZ = 0f;
		}
		if (!DialogManager._current._isDialogEnabled)
		{
			_RotX += finalInputZ * inputSensitivity;
			_RotY += finalInputX * inputSensitivity;
			_RotX = Mathf.Clamp(_RotX, 0f - _clampAngleDown, _clampAngleUp);
			Vector3 euler = new Vector3(_RotX, _RotY, 0f);
			localRotation = Quaternion.Euler(euler);
			if (InputControlManager.current._setGamepadType == GamepadType.Xbox360 && _isLerpingReset)
			{
				finalInputZ = 0f;
				finalInputX = 0f;
			}
			if (_isLerpingReset && finalInputZ == 0f && finalInputX == 0f)
			{
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(new Vector3(0f, _cachedEuler, 0f)), 12.5f * Time.deltaTime);
				_RotY = _cachedEuler;
				_RotX = 0f;
			}
			else if ((bool)_CameraFollowObj)
			{
				base.transform.rotation = localRotation;
				_isLerpingReset = false;
			}
		}
	}

	public void CameraReset()
	{
		base.transform.position = _CameraFollowObj.position + positionAdjust;
		_RotY = _CameraFollowObj.transform.eulerAngles.y;
		_RotX = 0f;
	}

	public void CameraReset_Lerp()
	{
		if ((bool)Player._mainPlayer && !_isLerpingReset)
		{
			StartCoroutine(Handle_CamReset_Lerp());
		}
	}

	private IEnumerator Handle_CamReset_Lerp()
	{
		_cachedEuler = _CameraFollowObj.transform.eulerAngles.y;
		_isLerpingReset = true;
		yield return new WaitForSeconds(0.45f);
		_isLerpingReset = false;
	}

	private void LateUpdate()
	{
		FollowTargetObj();
	}

	private void FollowTargetObj()
	{
		if (!Player._mainPlayer || !_CameraFollowObj)
		{
			return;
		}
		bool num = Player._mainPlayer._currentGameCondition == GameCondition.LOADING_GAME;
		bool flag = num && _snapLoadCameraBuffer >= 0.3f;
		if (num && _snapLoadCameraBuffer < 0.3f)
		{
			_snapLoadCameraBuffer += Time.deltaTime;
		}
		if (!num)
		{
			_snapLoadCameraBuffer = 0f;
		}
		if (!_initCameraOverride)
		{
			if (flag)
			{
				base.transform.position = _CameraFollowObj.position + positionAdjust;
			}
			else
			{
				base.transform.position = Vector3.Lerp(base.transform.position, _CameraFollowObj.position + positionAdjust, step * Time.deltaTime);
			}
		}
	}
}
