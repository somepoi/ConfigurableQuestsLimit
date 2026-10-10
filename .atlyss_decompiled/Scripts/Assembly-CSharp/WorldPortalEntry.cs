using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldPortalEntry : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	public ScriptableMapData _scriptMapData;

	public int _index;

	public Image _zoneIcon;

	public Image _backdropSprite;

	public GameObject _zoneSelector;

	public GameObject _selectIndicator;

	public RectTransform _rectTransform;

	public bool _isSelected;

	public void OnPointerDown(PointerEventData eventData)
	{
		WorldPortalManager._current._selectedWorldPortalEntry = this;
		WorldPortalManager._current._worldPortalSelection = _index;
		_isSelected = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
	}

	private void Awake()
	{
		_zoneSelector.SetActive(value: false);
	}

	private void Update()
	{
		_zoneSelector.SetActive(_isSelected);
		_selectIndicator.SetActive(_isSelected);
		_rectTransform.anchoredPosition = _scriptMapData._mapIconPosition;
		_backdropSprite.transform.localPosition = _scriptMapData._backdropPosition;
		_backdropSprite.rectTransform.localScale = _scriptMapData._backdropScaling;
	}
}
