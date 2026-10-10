using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestTrackerManager : MonoBehaviour
{
	public static QuestTrackerManager _current;

	[Header("Quest Tracker Manager")]
	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private CanvasGroup _questNote;

	[SerializeField]
	private Text _questNoteText;

	[Space]
	[SerializeField]
	private Transform _questTrackContainer;

	[SerializeField]
	private GameObject _questTrackPrefab;

	public List<QuestTrackElement> _questTrackElements = new List<QuestTrackElement>();

	[Space]
	public bool _hideTracker;

	public bool _refreshingElements;

	private float _questNoteBuffer;

	private void Awake()
	{
		_current = this;
	}

	private void Update()
	{
		if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus)
		{
			Handle_QuestNoteMessageBuffer();
			Handle_TrackerVisibility();
		}
	}

	private void Handle_QuestNoteMessageBuffer()
	{
		if (_questNoteBuffer > 0f)
		{
			_questNoteBuffer -= Time.deltaTime;
			return;
		}
		_questNote.alpha = Mathf.Lerp(_questNote.alpha, 0f, 13.5f * Time.deltaTime);
		_questNoteBuffer = 0f;
	}

	private void Handle_TrackerVisibility()
	{
		for (int i = 0; i < _questTrackElements.Count; i++)
		{
			_questTrackElements[i].gameObject.SetActive(!_questTrackElements[i]._isHidden);
		}
		if (Init_Visibility())
		{
			_canvasGroup.alpha = 1f;
		}
		else
		{
			_canvasGroup.alpha = 0f;
		}
		bool Init_Visibility()
		{
			if (Player._mainPlayer._playerZoneType == ZoneType.Arena)
			{
				return false;
			}
			if (SettingsManager._current._hideQuestTracker.isOn)
			{
				return false;
			}
			if (TabMenu._current._isOpen)
			{
				return false;
			}
			if (_questTrackElements.Count <= 0)
			{
				return false;
			}
			if (_hideTracker)
			{
				return false;
			}
			return true;
		}
	}

	public void Refresh_ProgressElements()
	{
		ClearAll_QuestTrackElements();
		Create_QuestTrackElements();
	}

	public void Update_ProgressElements()
	{
		for (int i = 0; i < _questTrackElements.Count; i++)
		{
			_questTrackElements[i].Update_QuestTrackElement();
		}
	}

	private void Create_QuestTrackElements()
	{
		StopAllCoroutines();
		StartCoroutine(Create_Elements());
	}

	private void ClearAll_QuestTrackElements()
	{
		for (int i = 0; i < _questTrackElements.Count; i++)
		{
			Object.Destroy(_questTrackElements[i].gameObject);
		}
		_questTrackElements.Clear();
	}

	private IEnumerator Create_Elements()
	{
		PlayerQuesting _pQuest = Player._mainPlayer._pQuest;
		_refreshingElements = true;
		while (_pQuest._questProgressData.Count <= 0)
		{
			yield return null;
		}
		for (int i = 0; i < _pQuest._questProgressData.Count; i++)
		{
			GameObject obj = Object.Instantiate(_questTrackPrefab, base.transform.position, Quaternion.identity);
			obj.transform.SetParent(_questTrackContainer);
			obj.transform.position = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			QuestTrackElement component = obj.GetComponent<QuestTrackElement>();
			component._scriptQuest = _pQuest._scriptableQuests[i];
			component._questIndex = i;
			_questTrackElements.Add(component);
		}
		Update_ProgressElements();
		_refreshingElements = false;
	}

	public void Init_QuestNote(string _questNoteString)
	{
		PlayerSound pSound = Player._mainPlayer._pSound;
		pSound._aSrcPlayerUI.PlayOneShot(pSound._questProgressTick);
		_questNoteText.text = _questNoteString ?? "";
		_questNote.alpha = 1f;
		_questNoteBuffer = 1.95f;
	}
}
