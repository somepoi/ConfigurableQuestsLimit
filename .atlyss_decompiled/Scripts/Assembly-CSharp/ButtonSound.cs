using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerClickHandler
{
	public Button _button;

	public AudioClip _hoverSoundClip;

	public AudioClip _clickSoundClip;

	private AudioSource _aSrc;

	private void Awake()
	{
		_button = GetComponent<Button>();
		_aSrc = GetComponent<AudioSource>();
		_aSrc.playOnAwake = false;
	}

	private void Start()
	{
		if (!_aSrc)
		{
			base.enabled = false;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if ((!_button || _button.interactable) && (bool)_hoverSoundClip && _aSrc.enabled)
		{
			_aSrc.PlayOneShot(_hoverSoundClip, 1f);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if ((!_button || _button.interactable) && (bool)_clickSoundClip && _aSrc.enabled)
		{
			Play_ClickSound();
		}
	}

	public void Play_ClickSound()
	{
		_aSrc.PlayOneShot(_clickSoundClip, 0.45f);
	}
}
