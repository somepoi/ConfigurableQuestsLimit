using UnityEngine;
using UnityEngine.UI;

public abstract class ListDataEntry : MonoBehaviour
{
	[Header("List Data Entry")]
	public Text _dataNameText;

	public int _dataID;

	public bool _isHighlighted;
}
