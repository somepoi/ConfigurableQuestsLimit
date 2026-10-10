using UnityEngine;

public class GroundSnapPosition : MonoBehaviour
{
	private Player _isPlayer;

	[Header("Ground Snap Position")]
	[SerializeField]
	private Transform _transformObject;

	[SerializeField]
	private Renderer _objectRend;

	[SerializeField]
	private Vector3 _snapPositionAdjust = Vector3.zero;

	private Vector3 _hitPointNormal;

	[Space]
	[SerializeField]
	private float _groundDetectRadius = 1.25f;

	[SerializeField]
	private float _groundDetectLength = 10.5f;

	[SerializeField]
	private float _groundDetectionAdjust = 1.5f;

	[SerializeField]
	private bool _snapToGround = true;

	[SerializeField]
	private bool _applyRotationAngle = true;

	[SerializeField]
	private bool _useLocalPosition;

	[SerializeField]
	private bool _useLocalRotation;

	private void Start()
	{
		if ((bool)GetComponentInParent<Player>())
		{
			_isPlayer = GetComponentInParent<Player>();
		}
	}

	private void Update()
	{
		_transformObject.gameObject.SetActive(Apply_Active());
		if (!Apply_Active())
		{
			return;
		}
		if (base.gameObject.scene.GetPhysicsScene().SphereCast(base.transform.position + new Vector3(0f, _groundDetectionAdjust, 0f), _groundDetectRadius, Vector3.down, out var hitInfo, _groundDetectLength, CastLayers.GroundMask))
		{
			_hitPointNormal = hitInfo.normal;
			if ((bool)_objectRend)
			{
				_objectRend.enabled = true;
			}
			if (_applyRotationAngle)
			{
				if (_useLocalRotation)
				{
					_transformObject.localRotation = Quaternion.LookRotation(_hitPointNormal);
				}
				else if (_hitPointNormal != Vector3.zero)
				{
					_transformObject.rotation = Quaternion.LookRotation(_hitPointNormal);
				}
			}
			if (_snapToGround)
			{
				if (_useLocalPosition)
				{
					_transformObject.localPosition = hitInfo.point + _snapPositionAdjust;
				}
				else
				{
					_transformObject.position = hitInfo.point + _snapPositionAdjust;
				}
			}
		}
		else if ((bool)_objectRend)
		{
			_objectRend.enabled = false;
		}
		bool Apply_Active()
		{
			if (!Player._mainPlayer)
			{
				return false;
			}
			if (Player._mainPlayer.gameObject.scene != base.gameObject.scene)
			{
				return false;
			}
			if ((bool)_isPlayer && Player._mainPlayer.Network_playerMapInstance != _isPlayer.Network_playerMapInstance)
			{
				return false;
			}
			if ((bool)_isPlayer && _isPlayer._pVisual._forceHidden)
			{
				return false;
			}
			return true;
		}
	}
}
