using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ErrorPromptTextManager : MonoBehaviour
{
	public static ErrorPromptTextManager current;

	[Header("Error Prompt UI Params")]
	[SerializeField]
	private CanvasGroup errorPromptGroup;

	[SerializeField]
	private Text errorPromptText;

	[SerializeField]
	private float errorPromptFadeTime = 0.85f;

	[SerializeField]
	private float _errorPromptStayTime = 1f;

	[SerializeField]
	private AudioSource _errorSoundASrc;

	private bool isFadingPrompt;

	private void Start()
	{
		errorPromptText.text = string.Empty;
		errorPromptGroup.alpha = 0f;
		current = this;
	}

	public void Init_ErrorPrompt(string text)
	{
		if (errorPromptText.text != text)
		{
			_errorSoundASrc.pitch = Random.Range(0.97231f, 1.109384f);
			_errorSoundASrc.Play();
		}
		errorPromptText.text = text;
		errorPromptGroup.alpha = 1f;
		isFadingPrompt = false;
		StopAllCoroutines();
		StartCoroutine(TextDisplayTime());
	}

	private void Update()
	{
		if (isFadingPrompt)
		{
			errorPromptGroup.alpha = Mathf.Lerp(errorPromptGroup.alpha, 0f, errorPromptFadeTime * Time.deltaTime);
		}
		if (!AtlyssNetworkManager._current.isNetworkActive)
		{
			errorPromptGroup.alpha = 0f;
		}
		if (errorPromptGroup.alpha <= 0.05f)
		{
			errorPromptText.text = string.Empty;
		}
	}

	private IEnumerator TextDisplayTime()
	{
		yield return new WaitForSeconds(_errorPromptStayTime);
		isFadingPrompt = true;
	}
}
