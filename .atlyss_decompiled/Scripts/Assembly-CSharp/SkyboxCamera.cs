using UnityEngine;

public class SkyboxCamera : MonoBehaviour
{
	[SerializeField]
	private Transform _mainCamera;

	[SerializeField]
	private float _skyboxScale = 100f;

	private void LateUpdate()
	{
		Handle_SkyboxCameraParams();
	}

	private void Handle_SkyboxCameraParams()
	{
		if ((bool)CameraFunction._current)
		{
			_mainCamera = CameraFunction._current._mainCamera.transform;
			base.transform.rotation = _mainCamera.rotation;
			base.transform.localPosition = _mainCamera.position / _skyboxScale;
		}
	}
}
