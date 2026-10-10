using UnityEngine;
using UnityEngine.UI;

public class NameTagElement : MonoBehaviour
{
	[Header("In-Game Nametag Element")]
	public Text _nicknameText;

	public GameObject _nameTagObject;

	[SerializeField]
	private Transform _nameTagScaleObject;

	[SerializeField]
	private float _distanceToShowTag = 50.95f;

	[SerializeField]
	private float _distanceToScaleTag = 30.5f;

	[SerializeField]
	private bool _distanceDisableNameTag = true;

	public bool _enableNameTag = true;

	private void Start()
	{
		_nameTagObject.layer = LayerMask.NameToLayer("UI");
	}

	private void Update()
	{
		if (!Player._mainPlayer)
		{
			return;
		}
		if (_nicknameText.text == string.Empty || _nicknameText.text == "Entity Nametag")
		{
			_nameTagObject.SetActive(value: false);
			return;
		}
		if (!_enableNameTag)
		{
			_nameTagObject.SetActive(value: false);
			return;
		}
		if (DialogManager._current._isDialogEnabled)
		{
			_nameTagObject.SetActive(value: false);
			return;
		}
		if ((bool)Player._mainPlayer && Player._mainPlayer._isHostPlayer && Player._mainPlayer.gameObject.scene != base.gameObject.scene)
		{
			_nameTagObject.SetActive(value: false);
			return;
		}
		float num = Vector3.Distance(base.transform.position, Player._mainPlayer.transform.position);
		float num2 = Vector3.Distance(base.transform.position, CameraFunction._current._mainCamera.transform.position);
		_distanceDisableNameTag = num > _distanceToShowTag;
		if (!_distanceDisableNameTag)
		{
			_nameTagObject.SetActive(value: true);
		}
		else
		{
			_nameTagObject.SetActive(value: false);
		}
		if (num2 > _distanceToScaleTag)
		{
			_nameTagScaleObject.localScale = Vector3.Lerp(_nameTagScaleObject.localScale, new Vector3(0.0205f, 0.0135f, 1f), 8.5f * Time.deltaTime);
		}
		else
		{
			_nameTagScaleObject.localScale = Vector3.Lerp(_nameTagScaleObject.localScale, new Vector3(0.0115f, 0.0065f, 1f), 8.5f * Time.deltaTime);
		}
	}
}
