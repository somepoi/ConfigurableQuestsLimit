using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LerpPosition : MonoBehaviour
{
	private NetworkTransformUnreliable _netTransform;

	[Header("Lerp Transform")]
	[SerializeField]
	private float _lerpSpeed = 12.65f;

	[SerializeField]
	private bool _isSmoothLerp;

	[SerializeField]
	private bool _autoMove;

	[SerializeField]
	private bool _switchIndexOnWallCollide;

	public bool _isMoving;

	[Header("Trigger Relation")]
	[SerializeField]
	private NetTrigger _interactTrigger;

	[Header("Collider")]
	[SerializeField]
	private Transform _doorCollider;

	[Header("Position Points")]
	[SerializeField]
	private int _startingIndex;

	[SerializeField]
	private bool _collectChildPoints = true;

	[SerializeField]
	private List<Transform> _positionIndex = new List<Transform>(0);

	private int _currentIndex;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _aSrc;

	[SerializeField]
	private AudioClip _beginMove;

	[SerializeField]
	private AudioClip _moveLoop;

	[SerializeField]
	private AudioClip _endMove;

	private AudioSource[] _audioCache;

	private PhysicsScene _physicsScene;

	public int CurrentIndex
	{
		get
		{
			return _currentIndex;
		}
		set
		{
			if (value <= 0)
			{
				value = 0;
			}
			if (value > _positionIndex.Count - 1)
			{
				value = 0;
			}
			_currentIndex = value;
		}
	}

	private void Awake()
	{
		_netTransform = GetComponent<NetworkTransformUnreliable>();
		_audioCache = GetComponentsInChildren<AudioSource>();
		_physicsScene = base.gameObject.scene.GetPhysicsScene();
	}

	private void Start()
	{
		Init_ExtractPositionIndex();
	}

	private void Init_ExtractPositionIndex()
	{
		if (_collectChildPoints)
		{
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (!componentsInChildren[i].gameObject.GetComponent<Collider>() && !(componentsInChildren[i] == base.transform))
				{
					_positionIndex.Add(componentsInChildren[i]);
				}
			}
		}
		for (int j = 0; j < _positionIndex.Count; j++)
		{
			_positionIndex[j].name = $"_lerpPositionPoint{j} ({base.gameObject.name})";
			_positionIndex[j].SetParent(null);
			_positionIndex[j].SetAsLastSibling();
		}
		CurrentIndex = _startingIndex;
	}

	public void Apply_PositionIndex(int _index)
	{
		if (_index != _currentIndex && !_isMoving)
		{
			CurrentIndex = _index;
			if ((bool)_aSrc && _aSrc.isActiveAndEnabled)
			{
				_aSrc.PlayOneShot(_beginMove, 0.45f);
				_aSrc.Play();
			}
			_isMoving = true;
		}
	}

	public void Next_PositionIndex()
	{
		if (!_isMoving)
		{
			CurrentIndex++;
			if ((bool)_aSrc)
			{
				_aSrc.PlayOneShot(_beginMove, 0.45f);
				_aSrc.Play();
			}
			_isMoving = true;
		}
	}

	public void Set_LerpSpeed(float _speed)
	{
		_lerpSpeed = _speed;
	}

	private void Update()
	{
		Handle_MoveToPosition();
	}

	private void Handle_MoveToPosition()
	{
		bool _disableAsrc = (bool)Player._mainPlayer && (Player._mainPlayer._bufferingStatus || (Player._mainPlayer.isServer && Player._mainPlayer.gameObject.scene != base.gameObject.scene));
		for (int i = 0; i < _audioCache.Length; i++)
		{
			_audioCache[i].mute = _disableAsrc;
		}
		if ((bool)_netTransform && _netTransform.isClientOnly)
		{
			return;
		}
		if (!_isSmoothLerp)
		{
			base.transform.SetPositionAndRotation(Vector3.MoveTowards(base.transform.position, _positionIndex[_currentIndex].position, _lerpSpeed * Time.deltaTime), Quaternion.RotateTowards(base.transform.rotation, _positionIndex[_currentIndex].rotation, _lerpSpeed * Time.deltaTime));
		}
		else
		{
			base.transform.SetPositionAndRotation(Vector3.Lerp(base.transform.position, _positionIndex[_currentIndex].position, _lerpSpeed * Time.deltaTime), Quaternion.Lerp(base.transform.rotation, _positionIndex[_currentIndex].rotation, _lerpSpeed * Time.deltaTime));
		}
		if ((bool)_doorCollider)
		{
			_doorCollider.SetPositionAndRotation(_positionIndex[_currentIndex].position, _positionIndex[_currentIndex].rotation);
		}
		if (!_isMoving && _autoMove)
		{
			Next_PositionIndex();
			return;
		}
		if (_switchIndexOnWallCollide)
		{
			LayerMask layerMask = CastLayers.GroundMask;
			Vector3 direction = _positionIndex[_currentIndex].position - base.transform.position;
			if (_physicsScene.Raycast(base.transform.position + Vector3.up * 1.35f, direction, 1.25f, layerMask))
			{
				Init_EndMove();
			}
		}
		float num = Vector3.Distance(base.transform.position, _positionIndex[_currentIndex].position);
		if (!_isSmoothLerp && num <= 0.001f)
		{
			Init_EndMove();
		}
		else if (_isSmoothLerp && num <= 0.12f)
		{
			Init_EndMove();
		}
		void Init_EndMove()
		{
			if (_isMoving)
			{
				if ((bool)_interactTrigger && _interactTrigger.isServer)
				{
					_interactTrigger.Network_triggerEnabled = false;
				}
				if ((bool)_aSrc && (bool)_endMove && !_disableAsrc)
				{
					_aSrc.Stop();
					_aSrc.PlayOneShot(_endMove, 0.45f);
				}
				_isMoving = false;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!(componentsInChildren[i] == base.transform))
			{
				Gizmos.matrix = componentsInChildren[i].localToWorldMatrix;
				Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			}
		}
	}
}
