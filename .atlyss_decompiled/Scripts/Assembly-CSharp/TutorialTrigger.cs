using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
	public string _tutorialTag = string.Empty;

	[TextArea(6, 6)]
	public string _tutorialPrompt = string.Empty;

	public bool _isGamepadTutorial;

	public bool _isKeyboardTutorial;

	private void OnValidate()
	{
		base.name = "TutorialTrigger(" + _tutorialTag + ")";
	}

	private void Awake()
	{
		base.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponentInParent<Player>())
		{
			Player componentInParent = other.GetComponentInParent<Player>();
			if ((!_isGamepadTutorial || InputControlManager.current._setGamepadType != GamepadType.Keyboard) && (!_isKeyboardTutorial || InputControlManager.current._setGamepadType == GamepadType.Keyboard) && !LoadSceneManager._current._isLoadingScreen && !componentInParent._bufferingStatus && componentInParent.isLocalPlayer)
			{
				TutorialManager._current.Init_TutorialMessage(_tutorialPrompt, _tutorialTag);
			}
		}
	}
}
