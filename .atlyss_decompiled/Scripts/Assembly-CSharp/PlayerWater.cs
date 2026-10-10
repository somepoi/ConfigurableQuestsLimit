using UnityEngine;

public class PlayerWater : MonoBehaviour
{
	private Player _player;

	private PlayerMove _pMove;

	private PlayerVisual _pVisual;

	private PlayerSound _pSound;

	[Header("Player Water")]
	[SerializeField]
	private float _currentLoadBuffer = 0.65f;

	public bool _isInWater;

	public bool _canSwimUp;

	private float _allowSwimUpBuffer;

	private void Awake()
	{
		_player = GetComponentInParent<Player>();
		_pMove = GetComponentInParent<PlayerMove>();
		_pVisual = GetComponentInParent<PlayerVisual>();
		_pSound = GetComponentInParent<PlayerSound>();
	}

	private void Update()
	{
		Handle_UnderwaterGravity();
		Handle_LoadBuffer();
		if (!_isInWater)
		{
			_pMove._underwaterGravityScaleMod = 0f;
		}
		if (_player._bufferingStatus && _isInWater)
		{
			Init_ExitWater();
		}
		if ((bool)Player._mainPlayer && Player._mainPlayer._bufferingStatus && _isInWater)
		{
			Init_ExitWater();
		}
	}

	private void Handle_UnderwaterGravity()
	{
		if (!_isInWater)
		{
			_canSwimUp = false;
			_allowSwimUpBuffer = 0f;
			return;
		}
		_canSwimUp = _allowSwimUpBuffer >= 0.05f;
		if (_allowSwimUpBuffer < 0.05f)
		{
			_allowSwimUpBuffer += 0.12f * Time.deltaTime;
		}
		if (_pMove._underwaterGravityScaleMod > -0.095f)
		{
			_pMove._underwaterGravityScaleMod -= 0.12f * Time.deltaTime;
		}
	}

	private void Handle_LoadBuffer()
	{
		if (!_player._bufferingStatus && _currentLoadBuffer > 0f)
		{
			_currentLoadBuffer -= Time.deltaTime;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Water") && !_player._bufferingStatus && !(_currentLoadBuffer > 0f) && !_isInWater)
		{
			_pVisual._waterEnterEffect.Play();
			_pVisual._waterExitEffect.Stop();
			if (_pVisual.isServer)
			{
				PlayerVisual pVisual = _pVisual;
				pVisual.Network_movementAnimSpeedMod = pVisual._movementAnimSpeedMod - 0.55f;
			}
			_pSound._aSrcGeneral.PlayOneShot(_pSound._waterEnter, 0.45f);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Water") && !(_currentLoadBuffer > 0f) && !_player._bufferingStatus)
		{
			if (_pMove.isLocalPlayer)
			{
				_pMove._currentRevUpSpeed = -3.5f;
			}
			_isInWater = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Water") && !_player._bufferingStatus && !(_currentLoadBuffer > 0f) && _isInWater)
		{
			Init_ExitWater();
		}
	}

	private void Init_ExitWater()
	{
		_pVisual._waterExitEffect.Play();
		if (_pVisual.isServer)
		{
			PlayerVisual pVisual = _pVisual;
			pVisual.Network_movementAnimSpeedMod = pVisual._movementAnimSpeedMod + 0.55f;
		}
		if (_pSound._aSrcGeneral.enabled)
		{
			_pSound._aSrcGeneral.PlayOneShot(_pSound._waterExit, 0.45f);
		}
		if (_pMove.isLocalPlayer && _pMove._currentRevUpSpeed < 0f)
		{
			_pMove._currentRevUpSpeed = 0f;
			_pMove._airTime = 0f;
		}
		_isInWater = false;
	}
}
