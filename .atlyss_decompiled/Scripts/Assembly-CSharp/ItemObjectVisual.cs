using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;

public class ItemObjectVisual : MonoBehaviour
{
	private static readonly int _HUE = Shader.PropertyToID("_Hue");

	private static readonly int _SATURATION = Shader.PropertyToID("_Saturation");

	private static readonly int _BRIGHTNESS = Shader.PropertyToID("_Brightness");

	private static readonly int _CONTRAST = Shader.PropertyToID("_Contrast");

	private static readonly int _TINT = Shader.PropertyToID("_ColorTint");

	private ItemObject _itemObject;

	[Header("Item Object Nametag")]
	public TextMeshPro _itemNametagTextMesh;

	public TextMeshPro _itemQuantityTextMesh;

	[Header("Item Object Visual")]
	public GameObject _visualRootObject;

	public GameObject _itemVisualObject;

	public ParticleSystem _itemObjectParticle;

	private Color _setItemRarityColor;

	public bool _followPlayerOnPickup;

	public bool _flashVisual;

	private bool _isFlashing;

	[Header("Sprite Icon Visual")]
	[SerializeField]
	private GameObject _spriteVisualContainer;

	[SerializeField]
	private SpriteRenderer _itemVisualSpriteRend;

	[SerializeField]
	private SpriteRenderer _itemVisualErrorSpriteRend;

	public GameObject _itemVisualMeshObject;

	[SerializeField]
	private float _pickupFadeSpeed = 0.02f;

	[Header("Equip Display Visual")]
	[SerializeField]
	private GameObject _equipMeshVisualContainer;

	[SerializeField]
	private MeshRenderer _shieldMeshObject;

	[SerializeField]
	private ParticleSystem _rareParticleEffect;

	[SerializeField]
	private ParticleSystem _exoticParticleEffect;

	[SerializeField]
	private ParticleSystem _legendaryParticleEffect;

	[Space]
	[SerializeField]
	private MeshRenderer _weaponMeshObject;

	[SerializeField]
	private MeshRenderer _doubleWeaponMeshObject;

	[Header("Currency Display Visual")]
	[SerializeField]
	private GameObject _currencyMeshVisualContainer;

	[SerializeField]
	private GameObject[] _currencyMeshes;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _spawnSound_00;

	[SerializeField]
	private AudioSource _spawnSound_01;

	[SerializeField]
	private AudioSource _aSrc_rareSpawn;

	private AudioSource[] _aSrcCache;

	private void Awake()
	{
		_itemObject = GetComponent<ItemObject>();
		_aSrcCache = GetComponentsInChildren<AudioSource>();
	}

	private void OnEnable()
	{
		Init_ItemVisual();
	}

	private void Init_ItemVisual()
	{
		_flashVisual = false;
		_isFlashing = false;
		_visualRootObject.SetActive(value: true);
		for (int i = 0; i < _currencyMeshes.Length; i++)
		{
			_currencyMeshes[i].SetActive(value: false);
		}
		_followPlayerOnPickup = false;
		_weaponMeshObject.gameObject.SetActive(value: false);
		_doubleWeaponMeshObject.gameObject.SetActive(value: false);
		_shieldMeshObject.gameObject.SetActive(value: false);
		_spriteVisualContainer.SetActive(value: false);
		_equipMeshVisualContainer.SetActive(value: false);
		_currencyMeshVisualContainer.SetActive(value: false);
		_itemVisualErrorSpriteRend.gameObject.SetActive(value: false);
		Invoke("Init_ItemDropSfx", 0.092f);
		Apply_ItemObjectVisual();
	}

	private void Init_ItemDropSfx()
	{
		if (!(Player._mainPlayer.gameObject.scene != base.gameObject.scene))
		{
			_spawnSound_00.Play();
			_spawnSound_01.Play();
		}
	}

	private void Update()
	{
		Handle_ItemPickupVisual();
		Handle_ItemObjAudio();
		if (_flashVisual && !_isFlashing)
		{
			StartCoroutine(Init_FlashObject());
		}
		if (!_flashVisual)
		{
			_visualRootObject.SetActive(value: true);
		}
	}

	private IEnumerator Init_FlashObject()
	{
		_isFlashing = true;
		while (_flashVisual)
		{
			_visualRootObject.SetActive(value: false);
			yield return new WaitForSeconds(0.06f);
			_visualRootObject.SetActive(value: true);
			yield return null;
		}
		_isFlashing = false;
	}

	[ClientCallback]
	private void Handle_ItemObjAudio()
	{
		if (NetworkClient.active)
		{
			for (int i = 0; i < _aSrcCache.Length; i++)
			{
				_aSrcCache[i].mute = Player._mainPlayer.gameObject.scene != base.gameObject.scene;
			}
		}
	}

	public void Apply_ItemObjectVisual()
	{
		if (_itemObject._currencyDropAmount > 0)
		{
			Apply_CurrencyVisual();
		}
		else
		{
			Apply_ItemVisual();
		}
		void Apply_CurrencyVisual()
		{
			_currencyMeshVisualContainer.SetActive(value: true);
			int currencyDropAmount = _itemObject._currencyDropAmount;
			if (currencyDropAmount <= 1)
			{
				_itemNametagTextMesh.text = $"{currencyDropAmount:n0} Crown";
			}
			else
			{
				_itemNametagTextMesh.text = $"{currencyDropAmount:n0} Crowns";
			}
			_itemNametagTextMesh.color = Color.yellow;
			_itemQuantityTextMesh.text = "";
			if (currencyDropAmount >= 200)
			{
				_currencyMeshes[3].SetActive(value: true);
			}
			else if (currencyDropAmount >= 100)
			{
				_currencyMeshes[2].SetActive(value: true);
			}
			else if (currencyDropAmount >= 50)
			{
				_currencyMeshes[1].SetActive(value: true);
			}
			else if (currencyDropAmount < 50)
			{
				_currencyMeshes[0].SetActive(value: true);
			}
		}
		void Apply_ItemVisual()
		{
			if ((bool)_itemObject._foundItem)
			{
				for (int i = 0; i < _currencyMeshes.Length; i++)
				{
					_currencyMeshes[i].SetActive(value: false);
				}
				_currencyMeshVisualContainer.SetActive(value: false);
				ItemRarity itemRarity = _itemObject._foundItem._itemRarity;
				string text = "";
				if (_itemObject._foundItem._itemType == ItemType.GEAR)
				{
					ScriptableEquipment scriptableEquipment = (ScriptableEquipment)_itemObject._foundItem;
					_itemVisualErrorSpriteRend.gameObject.SetActive(!scriptableEquipment.CanEquipItem(Player._mainPlayer._pStats, _isClickPrompt: false));
					if (_itemObject._local_itemData._modifierID > 0)
					{
						text = GameManager._current.Locate_StatModifier(_itemObject._local_itemData._modifierID)._modifierTag + " ";
						itemRarity++;
					}
				}
				else
				{
					_spriteVisualContainer.SetActive(value: true);
					_itemVisualSpriteRend.sprite = _itemObject._foundItem._itemIcon;
				}
				switch (itemRarity)
				{
				case ItemRarity.Rare:
					_rareParticleEffect.Play();
					_aSrc_rareSpawn.Play();
					break;
				case ItemRarity.Exotic:
					_exoticParticleEffect.Play();
					_aSrc_rareSpawn.Play();
					break;
				case ItemRarity.Legendary:
					_legendaryParticleEffect.Play();
					_aSrc_rareSpawn.Play();
					break;
				case ItemRarity.Cosmetic:
					_legendaryParticleEffect.Play();
					_aSrc_rareSpawn.Play();
					break;
				}
				_setItemRarityColor = GameManager._current._statLogics._itemRarityColors[(uint)itemRarity];
				_itemNametagTextMesh.text = text + _itemObject._foundItem._itemName;
				_itemNametagTextMesh.color = _setItemRarityColor;
				if (_itemObject._local_itemData._quantity > 1)
				{
					_itemQuantityTextMesh.text = $"x{_itemObject._local_itemData._quantity}";
				}
				else
				{
					_itemQuantityTextMesh.text = "";
				}
			}
		}
	}

	private void Handle_ItemPickupVisual()
	{
		if (!Player._mainPlayer)
		{
			return;
		}
		float num = Vector3.Distance(Player._mainPlayer.transform.position, base.transform.position);
		_itemNametagTextMesh.enabled = num <= 8.5f && !_followPlayerOnPickup && Player._mainPlayer.gameObject.scene == base.gameObject.scene;
		_visualRootObject.SetActive(Player._mainPlayer.gameObject.scene == base.gameObject.scene);
		if (Player._mainPlayer.gameObject.scene != base.gameObject.scene)
		{
			return;
		}
		if ((bool)_itemObject._foundItem && _itemObject._foundItem._itemType == ItemType.GEAR)
		{
			Apply_EquipmentDisplayParams();
		}
		if (_followPlayerOnPickup && (bool)_itemObject._pickupDestination)
		{
			_itemObject._rb.isKinematic = true;
			Vector3 vector = _itemObject._pickupDestination.position - base.transform.position;
			base.transform.Translate(17.85f * Time.deltaTime * vector);
			_equipMeshVisualContainer.transform.localScale = Vector3.Lerp(_equipMeshVisualContainer.transform.localScale, Vector3.zero, 8.35f * Time.deltaTime);
			_itemVisualErrorSpriteRend.transform.localScale = Vector3.Lerp(_equipMeshVisualContainer.transform.localScale, Vector3.zero, 8.35f * Time.deltaTime);
			_currencyMeshVisualContainer.transform.localScale = Vector3.Lerp(_currencyMeshVisualContainer.transform.localScale, Vector3.zero, 8.35f * Time.deltaTime);
			_spriteVisualContainer.transform.localScale = Vector3.Lerp(_currencyMeshVisualContainer.transform.localScale, Vector3.zero, 8.35f * Time.deltaTime);
			_rareParticleEffect.Stop();
			_exoticParticleEffect.Stop();
			_legendaryParticleEffect.Stop();
			_legendaryParticleEffect.Stop();
			_rareParticleEffect.Clear();
			_exoticParticleEffect.Clear();
			_legendaryParticleEffect.Clear();
			_legendaryParticleEffect.Clear();
		}
		else
		{
			_itemVisualObject.transform.localScale = Vector3.one;
			_equipMeshVisualContainer.transform.localScale = Vector3.one;
			_itemVisualErrorSpriteRend.transform.localScale = Vector3.one * 4f;
			_currencyMeshVisualContainer.transform.localScale = Vector3.one;
			_spriteVisualContainer.transform.localScale = Vector3.one;
		}
		if ((bool)_itemObject._local_taggedID)
		{
			if (_itemObject._local_taggedID == Player._mainPlayer.netIdentity)
			{
				if (!_itemObjectParticle.isPlaying)
				{
					_itemObjectParticle.Play();
				}
				_itemVisualSpriteRend.color = Color.Lerp(_itemVisualSpriteRend.color, Color.white, 6.25f * Time.deltaTime);
			}
			else
			{
				_itemObjectParticle.Stop();
				_itemVisualSpriteRend.color = Color.Lerp(_itemVisualSpriteRend.color, new Color(0.1f, 0.1f, 0.1f, 0.3f), 6.25f * Time.deltaTime);
			}
		}
		if (!_itemObject._isPickedUp && !_itemObject._local_taggedID)
		{
			if (!_itemObjectParticle.isPlaying)
			{
				_itemObjectParticle.Play();
			}
			_itemVisualSpriteRend.color = Color.Lerp(_itemVisualSpriteRend.color, Color.white, 6.25f * Time.deltaTime);
		}
	}

	private void Apply_EquipmentDisplayParams()
	{
		_equipMeshVisualContainer.SetActive(value: true);
		if (_itemObject._foundItem.GetType() == typeof(ScriptableWeapon))
		{
			ScriptableWeapon scriptableWeapon = (ScriptableWeapon)_itemObject._foundItem;
			_weaponMeshObject.gameObject.SetActive(value: true);
			_doubleWeaponMeshObject.gameObject.SetActive(scriptableWeapon.weaponType._weaponDisplayMode == WeaponRenderDisplaymode.DUAL);
			_weaponMeshObject.GetComponent<MeshFilter>().mesh = scriptableWeapon.weaponMesh;
			_weaponMeshObject.material.mainTexture = scriptableWeapon.weaponTexture;
			Set_MeshColorParams(_weaponMeshObject.material, scriptableWeapon._textureColorAdjust);
			_doubleWeaponMeshObject.GetComponent<MeshFilter>().mesh = scriptableWeapon.weaponMesh;
			_doubleWeaponMeshObject.material.mainTexture = scriptableWeapon.weaponTexture;
			Set_MeshColorParams(_doubleWeaponMeshObject.material, scriptableWeapon._textureColorAdjust);
		}
		else if (_itemObject._foundItem.GetType() == typeof(ScriptableShield))
		{
			ScriptableShield scriptableShield = (ScriptableShield)_itemObject._foundItem;
			_shieldMeshObject.gameObject.SetActive(value: true);
			_shieldMeshObject.GetComponent<MeshFilter>().mesh = scriptableShield._shieldMesh;
			_shieldMeshObject.material.mainTexture = scriptableShield._shieldTexture;
			Set_MeshColorParams(_shieldMeshObject.material, scriptableShield._textureColorAdjust);
		}
		else
		{
			_equipMeshVisualContainer.SetActive(value: false);
			_spriteVisualContainer.SetActive(value: true);
			_itemVisualSpriteRend.sprite = _itemObject._foundItem._itemIcon;
		}
		void Set_MeshColorParams(Material _mat, ColorAdjustShader_Profile _colorAdjustProfile)
		{
			if ((bool)Player._mainPlayer)
			{
				float value = -0.45f;
				_mat.SetFloat(_HUE, _colorAdjustProfile._hue);
				_mat.SetFloat(_CONTRAST, _colorAdjustProfile._contrast);
				_mat.SetFloat(_SATURATION, _colorAdjustProfile._saturation);
				if (_itemObject._local_taggedID != null)
				{
					if (!(_itemObject._local_taggedID != null) || !(_itemObject._local_taggedID == Player._mainPlayer.netIdentity))
					{
						_mat.SetFloat(_BRIGHTNESS, value);
					}
					else
					{
						_mat.SetFloat(_BRIGHTNESS, _colorAdjustProfile._brightness);
					}
				}
				if (_itemObject._local_taggedID == null)
				{
					_mat.SetFloat(_BRIGHTNESS, _colorAdjustProfile._brightness);
				}
			}
		}
	}
}
