using UnityEngine;

public class MouseWorldPosition : MonoBehaviour
{
	[Header("Indicator Mouse World Positioner")]
	[SerializeField]
	private Transform _displayObj;

	[SerializeField]
	private Renderer _objRend;

	public SkillIndicator_Parameters _IP;

	public bool _isConfirm;

	private float _distanceFromPlayer;

	private RaycastHit hitData;

	private Ray ray;

	private Vector3 worldPosition;

	private Vector3 _normal;

	public static MouseWorldPosition _current { get; private set; }

	private void Awake()
	{
		_current = this;
		_objRend.enabled = false;
	}

	private void Update()
	{
		if (!Player._mainPlayer || !GameManager._current)
		{
			return;
		}
		if (Player._mainPlayer._bufferingStatus)
		{
			base.transform.SetParent(Player._mainPlayer.transform);
		}
		else
		{
			base.transform.SetParent(null);
		}
		PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
		ray = CameraFunction._current._mainCamera.ScreenPointToRay(Input.mousePosition);
		if (physicsScene.Raycast(ray.origin, ray.direction, out hitData, 1000f, _IP.layerMask))
		{
			_objRend.enabled = true;
			worldPosition = hitData.point + new Vector3(0f, 0.5f, 0f);
			_normal = hitData.normal;
			_objRend.material.SetColor("_TintColor", _IP._confirmColor);
			_isConfirm = true;
			if (_IP._measureDistanceFromPlayer)
			{
				_distanceFromPlayer = Vector3.Distance(_displayObj.position, Player._mainPlayer.transform.position);
				if (_distanceFromPlayer >= _IP._distanceToDeny)
				{
					_objRend.material.SetColor("_TintColor", _IP._denyColor);
					_isConfirm = false;
				}
			}
			if (hitData.normal.z <= 0f - _IP._clampAngleAxis.z || hitData.normal.x <= 0f - _IP._clampAngleAxis.x)
			{
				_objRend.material.SetColor("_TintColor", _IP._denyColor);
				_isConfirm = false;
			}
			if (hitData.normal.z >= _IP._clampAngleAxis.z || hitData.normal.x >= _IP._clampAngleAxis.x)
			{
				_objRend.material.SetColor("_TintColor", _IP._denyColor);
				_isConfirm = false;
			}
			if (_IP._snapAngle)
			{
				_displayObj.SetPositionAndRotation(worldPosition, Quaternion.LookRotation(_normal));
			}
			if (!_IP._snapAngle)
			{
				_displayObj.position = worldPosition;
			}
		}
		else
		{
			_objRend.enabled = false;
		}
	}
}
