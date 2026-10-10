using UnityEngine;

public class RotateObject : MonoBehaviour
{
	[Range(-360f, 360f)]
	[SerializeField]
	private float _rotX;

	[Range(-360f, 360f)]
	[SerializeField]
	private float _rotY;

	[Range(-360f, 360f)]
	[SerializeField]
	private float _rotZ;

	[SerializeField]
	private bool _ignoreX;

	[SerializeField]
	private bool _ignoreY;

	[SerializeField]
	private bool _ignoreZ;

	[Header("Rotation Interpolation")]
	[SerializeField]
	private bool _lerpRotation;

	[SerializeField]
	private float _lerpTime = 16.5f;

	public float startRotX;

	public float startRotY;

	public float startRotZ;

	public float endRotX;

	public float endRotY;

	public float endRotZ;

	[Header("Optimization")]
	[Tooltip("Set this to true if the rotation should be happening even if the host player is not in the same scene.")]
	[SerializeField]
	private bool _forceRotation;

	private bool _alwaysRun;

	private void Awake()
	{
		_alwaysRun = (bool)GetComponentInParent<CombatCollider>() || (bool)GetComponentInChildren<CombatCollider>() || (bool)GetComponentInParent<RectTransform>() || (bool)GetComponentInChildren<RectTransform>() || (bool)GetComponentInParent<PlayerRaceModel>();
		if (_forceRotation)
		{
			_alwaysRun = true;
		}
	}

	private void OnEnable()
	{
		Init_LerpValues();
	}

	public void Init_LerpValues()
	{
		if (_lerpRotation)
		{
			_rotX = startRotX;
			_rotY = startRotY;
			_rotZ = startRotZ;
		}
	}

	public void Alter_RotationValues(Vector3 _rot)
	{
		_rotX = _rot.x;
		_rotY = _rot.y;
		_rotZ = _rot.z;
	}

	private void Update()
	{
		Handle_Rotation();
	}

	private void Handle_Rotation()
	{
		if (_alwaysRun || !Player._mainPlayer || !Player._mainPlayer._isHostPlayer || !(Player._mainPlayer.gameObject.scene != base.gameObject.scene))
		{
			if (_lerpRotation)
			{
				_rotX = Mathf.Lerp(_rotX, endRotX, _lerpTime * Time.deltaTime);
				_rotY = Mathf.Lerp(_rotY, endRotY, _lerpTime * Time.deltaTime);
				_rotZ = Mathf.Lerp(_rotZ, endRotZ, _lerpTime * Time.deltaTime);
			}
			if (_ignoreX)
			{
				_rotX = base.transform.rotation.x;
			}
			if (_ignoreY)
			{
				_rotY = base.transform.rotation.y;
			}
			if (_ignoreZ)
			{
				_rotZ = base.transform.rotation.z;
			}
			base.transform.Rotate(_rotX * 25.5f * Time.deltaTime, _rotY * 25.5f * Time.deltaTime, _rotZ * 25.5f * Time.deltaTime, Space.Self);
		}
	}
}
