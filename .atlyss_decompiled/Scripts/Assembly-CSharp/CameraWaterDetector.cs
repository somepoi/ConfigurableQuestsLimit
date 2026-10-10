using UnityEngine;

public class CameraWaterDetector : MonoBehaviour
{
	[SerializeField]
	private WaterCamera _waterCam;

	[SerializeField]
	private AudioLowPassFilter _lowPassFilter;

	[SerializeField]
	private AudioSource _aSrc;

	private void Update()
	{
		Handle_CameraWaterDetector();
		if ((bool)Player._mainPlayer && Player._mainPlayer._bufferingStatus && Player._mainPlayer.isLocalPlayer)
		{
			_waterCam.effectActive = false;
			_lowPassFilter.enabled = false;
			_aSrc.volume = 0f;
		}
	}

	private void Handle_CameraWaterDetector()
	{
		if ((bool)CameraFunction._current && (bool)Player._mainPlayer)
		{
			if (Player._mainPlayer._bufferingStatus)
			{
				base.transform.SetParent(Player._mainPlayer.transform);
			}
			else
			{
				base.transform.SetParent(null);
			}
			Vector3 position = CameraFunction._current._mainCamera.transform.position;
			position.y = CameraFunction._current._mainCamera.transform.position.y + 1.25f;
			base.transform.position = position;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if ((bool)CameraFunction._current && (bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && other.gameObject.layer == LayerMask.NameToLayer("Water"))
		{
			_waterCam.effectActive = true;
			_lowPassFilter.enabled = true;
			_aSrc.volume = 0.65f;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((bool)CameraFunction._current && (bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && other.gameObject.layer == LayerMask.NameToLayer("Water"))
		{
			_waterCam.effectActive = false;
			_lowPassFilter.enabled = false;
			_aSrc.volume = 0f;
		}
	}
}
