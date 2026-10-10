using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TooltipElement : MonoBehaviour
{
	private RectTransform mrect;

	[Header("Tooltip Element")]
	public MenuElement _toolTipMenuElement;

	public CanvasGroup _tooltipGroup;

	public Text _toolTipName;

	public Text _toolTipSubName;

	public Text _toolTipDescription;

	public Image _toolTipIcon;

	[Space]
	public Transform _anchorReference;

	[SerializeField]
	private Vector3 _toolTipAnchorCursor = Vector3.zero;

	[SerializeField]
	private float _tooltipTime = 0.35f;

	private void Awake()
	{
		mrect = GetComponent<RectTransform>();
	}

	public void Enable_ToolTip()
	{
		StopAllCoroutines();
		StartCoroutine(Init_TooltipDisplay());
	}

	public virtual void Disable_ToolTip()
	{
		StopAllCoroutines();
		if ((bool)_toolTipMenuElement)
		{
			_toolTipMenuElement.isEnabled = false;
		}
		_anchorReference = null;
	}

	public virtual void Update()
	{
		Vector3 position = (Cursor.visible ? (new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z) + _toolTipAnchorCursor) : ((!_anchorReference) ? (new Vector3(Screen.width / 2, Screen.height / 2, 0f) + _toolTipAnchorCursor) : (_anchorReference.position + _toolTipAnchorCursor)));
		float x = position.x;
		float y = position.y;
		x = Mathf.Clamp(x, 0f, (float)Screen.width - mrect.sizeDelta.x);
		y = Mathf.Clamp(y, 0f, (float)Screen.height - mrect.sizeDelta.y);
		position.x = x;
		position.y = y;
		base.transform.position = position;
	}

	public void Set_TooltipAnchorPos(Vector3 _newPos)
	{
		_toolTipAnchorCursor = _newPos;
	}

	private IEnumerator Init_TooltipDisplay()
	{
		yield return new WaitForSeconds(_tooltipTime);
		if ((bool)_toolTipMenuElement)
		{
			_toolTipMenuElement.isEnabled = true;
		}
	}
}
