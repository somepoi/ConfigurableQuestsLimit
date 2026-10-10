using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MenuElement))]
public abstract class TabMenuCell : MonoBehaviour
{
	[HideInInspector]
	public Player _mainPlayer;

	[Header("In Game Menu Cell")]
	public string _menuCell_tag = string.Empty;

	public MenuElement _menuCellElement;

	public RectTransform _menuCellRect;

	[Header("Menu Cell Scrollview List")]
	public GameObject _listDataObject_prefab;

	public Transform _cell_ScrollViewContent;

	public bool _bufferCommandRequest;

	public List<ListDataEntry> _listDataEntries = new List<ListDataEntry>();

	public abstract void Cell_OnAwake();

	public abstract void Handle_CellUpdate();

	private void Awake()
	{
		_menuCellElement = GetComponent<MenuElement>();
		_menuCellRect = GetComponent<RectTransform>();
		Cell_OnAwake();
	}

	private void Update()
	{
		if ((bool)GameManager._current && (bool)Player._mainPlayer)
		{
			if (_mainPlayer == null)
			{
				_mainPlayer = Player._mainPlayer;
			}
			Handle_CellUpdate();
		}
	}

	public void Clear_ListData()
	{
		if (_listDataEntries.Count > 0)
		{
			for (int i = 0; i < _listDataEntries.Count; i++)
			{
				Object.Destroy(_listDataEntries[i].gameObject);
			}
			_listDataEntries.Clear();
		}
	}
}
