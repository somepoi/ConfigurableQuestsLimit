using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
	public static TutorialManager _current;

	public CanvasGroup _tutorialCanvasGroup;

	public Text _tutorialText;

	public float _displayBuffer;

	public bool _disableWhenOpenTabMenu;

	public List<string> _finishedTutorialTags = new List<string>();

	private string _cachedMessage;

	private void Awake()
	{
		_current = this;
		_tutorialText.text = string.Empty;
	}

	private void Start()
	{
		ProfileDataManager._current.Load_TutorialData();
	}

	private void Update()
	{
		if ((bool)Player._mainPlayer)
		{
			if (Player._mainPlayer._bufferingStatus)
			{
				_tutorialCanvasGroup.alpha = 0f;
				_displayBuffer = 0f;
			}
			if (_disableWhenOpenTabMenu && TabMenu._current._isOpen)
			{
				_displayBuffer = 0f;
			}
			if (_displayBuffer > 0f)
			{
				_tutorialCanvasGroup.alpha = Mathf.Lerp(_tutorialCanvasGroup.alpha, 1f, 12.5f * Time.deltaTime);
				_displayBuffer -= Time.deltaTime;
			}
			else
			{
				_tutorialCanvasGroup.alpha = Mathf.Lerp(_tutorialCanvasGroup.alpha, 0f, 12.5f * Time.deltaTime);
			}
		}
	}

	public void Init_TutorialMessage(string _message, string _tutorialTag)
	{
		if (LoadSceneManager._current._isLoadingScreen || !Player._mainPlayer || Player._mainPlayer._bufferingStatus || !Player._mainPlayer.isLocalPlayer)
		{
			return;
		}
		for (int i = 0; i < ProfileDataManager._current._tutorialProfile._tutorialTags.Length; i++)
		{
			if (ProfileDataManager._current._tutorialProfile._tutorialTags[i] == _tutorialTag)
			{
				return;
			}
		}
		if (!_finishedTutorialTags.Contains(_tutorialTag))
		{
			_tutorialText.text = _message;
			_displayBuffer = 9.5f;
			_finishedTutorialTags.Add(_tutorialTag);
			_disableWhenOpenTabMenu = true;
			_cachedMessage = _message;
			Invoke("Init_ChatMessageRelay", 10.5f);
			ProfileDataManager._current.Save_TutorialData();
		}
	}

	private void Init_ChatMessageRelay()
	{
		if (!string.IsNullOrEmpty(_cachedMessage))
		{
			Player._mainPlayer.GetComponent<ChatBehaviour>().New_ChatMessage(_cachedMessage);
		}
	}
}
