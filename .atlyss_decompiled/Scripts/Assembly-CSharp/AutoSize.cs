using UnityEngine;

public class AutoSize : MonoBehaviour
{
	public float FixedSize = 0.005f;

	public Camera Camera;

	private void Start()
	{
		Camera = CameraFunction._current._UICamera;
	}

	private void Update()
	{
		float num = (Camera.transform.position - base.transform.position).magnitude * FixedSize * Camera.fieldOfView;
		base.transform.localScale = Vector3.one * num;
		base.transform.forward = base.transform.position - Camera.transform.position;
	}
}
