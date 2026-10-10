using UnityEngine;

public class CameraCollision : MonoBehaviour
{
	public static CameraCollision _current;

	public float minDistance = 1f;

	public float maxDistance = 4f;

	public float smooth = 10f;

	public float step = 0.9f;

	public Vector3 dollyDir;

	public float loggedDistance;

	public float distance;

	private RaycastHit hit;

	public bool _unhidePlayerModel;

	private void Awake()
	{
		_current = this;
		distance = base.transform.localPosition.magnitude;
	}

	private void Start()
	{
		maxDistance = 25f;
	}

	public bool Handle_DetectGroundLayer()
	{
		if (distance <= 0.6f)
		{
			return false;
		}
		Vector3 end = base.transform.parent.TransformPoint(dollyDir * maxDistance);
		if (Physics.Linecast(base.transform.parent.position, end, out hit, CastLayers.GroundMask, QueryTriggerInteraction.Ignore))
		{
			return true;
		}
		return false;
	}

	private void LateUpdate()
	{
		if ((bool)Player._mainPlayer)
		{
			PlayerVisual pVisual = Player._mainPlayer._pVisual;
			float num = Vector3.Distance(pVisual.transform.position, base.transform.position);
			Handle_DistanceControl();
			if (maxDistance > 60f)
			{
				maxDistance = 60f;
			}
			else if (maxDistance <= 0.6f)
			{
				maxDistance = 0.6f;
			}
			if (maxDistance <= 1.8f || num <= 2.75f)
			{
				pVisual.RenderPlayerModel(_isTrue: false);
				_unhidePlayerModel = true;
			}
			else if (_unhidePlayerModel)
			{
				pVisual.RenderPlayerModel(_isTrue: true);
				_unhidePlayerModel = false;
			}
			if (Handle_DetectGroundLayer())
			{
				distance = Mathf.Clamp(hit.distance * step, minDistance, maxDistance);
			}
			else
			{
				distance = maxDistance;
			}
			base.transform.localPosition = dollyDir * distance;
		}
		void Handle_DistanceControl()
		{
			if (!Player._mainPlayer._bufferingStatus && !Player._mainPlayer._inUI && !Player._mainPlayer._inChat)
			{
				float axis = Input.GetAxis("Mouse ScrollWheel");
				if (axis != 0f)
				{
					maxDistance += axis * 35.5f;
				}
				if (!GamepadInput.GetButton(GamepadButton.LeftShoulder) && !GamepadInput.GetButton(GamepadButton.RightShoulder))
				{
					if (InputControlManager.current.DpadUp(DpadHoldType.OnHold))
					{
						maxDistance -= 65.5f * Time.deltaTime;
					}
					if (InputControlManager.current.DpadDown(DpadHoldType.OnHold))
					{
						maxDistance += 65.5f * Time.deltaTime;
					}
				}
			}
		}
	}
}
