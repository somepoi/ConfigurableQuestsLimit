using UnityEngine;

public class PositionSet : MonoBehaviour
{
	[Header("Transform Key")]
	public Transform target;

	public Transform LookDirection;

	public Vector3 positionAdjust = Vector3.zero;

	public bool SetRotation;

	public bool SetRotation_ToCameraRotation;

	public bool SetToPlayer;

	public bool lookAtPlayer;

	public bool useLocalPosition;

	private void Update()
	{
		if (!target && SetToPlayer && (bool)Player._mainPlayer)
		{
			target = Player._mainPlayer.transform;
		}
		if ((bool)target)
		{
			if (!useLocalPosition)
			{
				base.transform.position = target.position + positionAdjust;
			}
			else
			{
				base.transform.localPosition = target.position + positionAdjust;
			}
			if (SetRotation)
			{
				base.transform.rotation = target.rotation;
			}
			if (SetRotation_ToCameraRotation)
			{
				base.transform.localRotation = Quaternion.Euler(new Vector3(0f, CameraFunction._current._RotY, 0f));
			}
		}
		if ((bool)LookDirection)
		{
			Vector3 vector = LookDirection.position - base.transform.position;
			base.transform.rotation = Quaternion.LookRotation(vector.normalized);
		}
		else if (lookAtPlayer)
		{
			Vector3 vector2 = Player._mainPlayer.transform.position - base.transform.position;
			base.transform.rotation = Quaternion.LookRotation(vector2.normalized);
		}
	}
}
