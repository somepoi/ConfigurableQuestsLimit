using UnityEngine;

public class MenuElement : MonoBehaviour
{
	private enum LerpType
	{
		Positional = 0,
		Scale = 1,
		Fade = 2
	}

	public bool isEnabled;

	private RectTransform menuRect;

	[SerializeField]
	private Vector2[] rectValue = new Vector2[2];

	[SerializeField]
	private float lerpSpeed;

	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private bool _startDisabledOnEnable;

	[SerializeField]
	private LerpType lerpType;

	public void EnableElement(bool isTrue)
	{
		isEnabled = isTrue;
	}

	private void Start()
	{
		menuRect = GetComponent<RectTransform>();
		Init_StartElement();
	}

	private void OnEnable()
	{
		menuRect = GetComponent<RectTransform>();
		Init_StartElement();
	}

	private void Init_StartElement()
	{
		switch (lerpType)
		{
		case LerpType.Positional:
			if (isEnabled)
			{
				if (_startDisabledOnEnable)
				{
					menuRect.anchoredPosition = rectValue[1];
				}
				else
				{
					menuRect.anchoredPosition = rectValue[0];
				}
			}
			else
			{
				menuRect.anchoredPosition = rectValue[1];
			}
			break;
		case LerpType.Scale:
			if (isEnabled)
			{
				if (_startDisabledOnEnable)
				{
					menuRect.localScale = rectValue[1];
				}
				else
				{
					menuRect.localScale = rectValue[0];
				}
			}
			else
			{
				menuRect.localScale = rectValue[1];
			}
			break;
		case LerpType.Fade:
			if (isEnabled)
			{
				_canvasGroup.alpha = 1f;
			}
			else
			{
				_canvasGroup.alpha = 0f;
			}
			break;
		}
	}

	private void LateUpdate()
	{
		switch (lerpType)
		{
		case LerpType.Positional:
			if (isEnabled)
			{
				menuRect.anchoredPosition = Vector2.Lerp(menuRect.anchoredPosition, rectValue[0], lerpSpeed * Time.deltaTime);
			}
			else
			{
				menuRect.anchoredPosition = Vector2.Lerp(menuRect.anchoredPosition, rectValue[1], lerpSpeed * Time.deltaTime);
			}
			break;
		case LerpType.Scale:
			if (isEnabled)
			{
				menuRect.localScale = Vector2.Lerp(menuRect.localScale, rectValue[0], lerpSpeed * Time.deltaTime);
			}
			else
			{
				menuRect.localScale = Vector2.Lerp(menuRect.localScale, rectValue[1], lerpSpeed * Time.deltaTime);
			}
			break;
		case LerpType.Fade:
			if (isEnabled)
			{
				_canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1f, lerpSpeed * Time.deltaTime);
			}
			else
			{
				_canvasGroup.alpha = 0f;
			}
			break;
		}
	}
}
