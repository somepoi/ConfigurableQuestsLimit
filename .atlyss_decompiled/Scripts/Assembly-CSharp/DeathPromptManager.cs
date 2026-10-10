using UnityEngine;
using UnityEngine.UI;

public class DeathPromptManager : MonoBehaviour
{
	[Header("Death Prompt UI")]
	[SerializeField]
	private MenuElement _deathPromptElement;

	[SerializeField]
	private Button _releaseSoulButton;

	[SerializeField]
	private Button _useTearButton;

	[SerializeField]
	private Text _useTearButtonText;

	[SerializeField]
	private float _deathPromptOpenBuffer = 1.25f;

	private float _currentPromptBuffer;

	private bool _requestedRecall;

	[Header("Gamepad")]
	[SerializeField]
	private Image _gamepadIco_releaseSoul;

	[SerializeField]
	private Image _gamepadIco_useTear;

	private void Start()
	{
		_releaseSoulButton.onClick.AddListener(Init_ReleaseSoul);
		_useTearButton.onClick.AddListener(Init_UseTear);
	}

	private void Update()
	{
		_deathPromptElement.transform.parent.gameObject.SetActive((bool)Player._mainPlayer && Player._mainPlayer._currentPlayerCondition == PlayerCondition.DEAD);
		_gamepadIco_releaseSoul.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		_gamepadIco_useTear.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		if (!Player._mainPlayer)
		{
			_currentPromptBuffer = _deathPromptOpenBuffer;
			_requestedRecall = false;
			_releaseSoulButton.interactable = false;
		}
		else if (Player._mainPlayer._currentPlayerCondition == PlayerCondition.DEAD && !Player._mainPlayer._bufferingStatus)
		{
			Handle_DeathPromptWindow();
		}
		else
		{
			_requestedRecall = false;
			_deathPromptElement.isEnabled = false;
			_releaseSoulButton.interactable = false;
			_useTearButton.interactable = false;
			_currentPromptBuffer = _deathPromptOpenBuffer;
		}
	}

	private void Handle_DeathPromptWindow()
	{
		if (_currentPromptBuffer > 0f)
		{
			_currentPromptBuffer -= Time.deltaTime;
			_useTearButton.gameObject.SetActive(value: false);
			return;
		}
		_currentPromptBuffer = 0f;
		int num = 0;
		for (int i = 0; i < Player._mainPlayer._pInventory._heldItems.Count; i++)
		{
			if (Player._mainPlayer._pInventory._heldItems[i]._itemName == "Angela's Tear")
			{
				num++;
			}
		}
		_useTearButton.gameObject.SetActive(num > 0);
		_useTearButtonText.text = $"Use Tear (x{num})";
		_deathPromptElement.isEnabled = _currentPromptBuffer <= 0f && !_requestedRecall;
		_releaseSoulButton.interactable = _currentPromptBuffer <= 0f;
		_useTearButton.interactable = _currentPromptBuffer <= 0f;
		if (InputControlManager.current._setGamepadType != GamepadType.Keyboard)
		{
			if (GamepadInput.GetButtonDown(GamepadButton.Button_South))
			{
				Init_ReleaseSoul();
			}
			if (num > 0 && GamepadInput.GetButtonDown(GamepadButton.Button_East))
			{
				Init_UseTear();
			}
		}
	}

	public void Init_ReleaseSoul()
	{
		Player._mainPlayer._pMove._playerController.enabled = false;
		Player._mainPlayer.Cmd_Respawn();
		_currentPromptBuffer = _deathPromptOpenBuffer;
		_requestedRecall = true;
	}

	public void Init_UseTear()
	{
		for (int i = 0; i < Player._mainPlayer._pInventory._heldItems.Count; i++)
		{
			if (Player._mainPlayer._pInventory._heldItems[i]._itemName == "Angela's Tear")
			{
				Player._mainPlayer._pInventory.Remove_Item(Player._mainPlayer._pInventory._heldItems[i], 1);
				break;
			}
		}
		Player._mainPlayer._statusEntity.Cmd_ReplenishAll();
		_currentPromptBuffer = 6.5f;
	}
}
