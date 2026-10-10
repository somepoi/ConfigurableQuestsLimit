using UnityEngine;

public class SkillBehavior_SummonFamiliar : SkillObjectBehavior
{
	private static readonly int _HUE = Shader.PropertyToID("_Hue");

	private static readonly int _SATURATION = Shader.PropertyToID("_Saturation");

	private static readonly int _BRIGHTNESS = Shader.PropertyToID("_Brightness");

	private static readonly int _CONTRAST = Shader.PropertyToID("_Contrast");

	private static readonly int _TINT = Shader.PropertyToID("_ColorTint");

	[Header("Summon Familiar")]
	[SerializeField]
	private Transform _familiarProjectileAimer;

	[SerializeField]
	private Animator _familiarAnimator;

	[SerializeField]
	private Creep_SkillCastSlot[] _familiarSkillSlots;

	[Header("Pathfinding")]
	[SerializeField]
	private Transform _followParentObj;

	[SerializeField]
	private float _followStep = 3.5f;

	[SerializeField]
	private Vector3 _positionOffset = Vector3.zero;

	[Header("Visual / Rendering Cache")]
	[SerializeField]
	private SkinnedMeshRenderer _meshRend;

	[SerializeField]
	private ParticleSystem _particleTrail;

	private Material[] _rendCache;

	private ScriptableArmorDye _dyeCache;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _familiarVoxAsrc;

	[SerializeField]
	private AudioClip[] _familiarVoxClips;

	public void OnDestroy()
	{
		End_SkillBehavior();
	}

	public override void End_SkillBehavior()
	{
		GameObject obj = PoolManager._current.GenerateObject(null, "Puffcloud", 1.5f);
		obj.SetActive(value: true);
		obj.transform.SetPositionAndRotation(base.transform.position, Quaternion.identity);
		if ((bool)_followParentObj)
		{
			Object.Destroy(_followParentObj.gameObject);
		}
	}

	public override void Init_SkillBehavior()
	{
		StatusEntity statusEntity = null;
		if ((bool)_skillObject._parentPlayer)
		{
			statusEntity = _skillObject._parentPlayer._statusEntity;
		}
		if ((bool)_skillObject._parentCreep)
		{
			statusEntity = _skillObject._parentCreep._statusEntity;
		}
		if ((bool)statusEntity)
		{
			statusEntity.Network_summonSkillTag = _skillObject._scriptSkill._skillName;
			if (!_followParentObj)
			{
				GameObject gameObject = new GameObject
				{
					name = "_skillObject_familiarPet_positioner"
				};
				_followParentObj = gameObject.transform;
				_followParentObj.SetParent(statusEntity.transform);
				_followParentObj.localPosition = Vector3.zero;
			}
			_familiarAnimator.CrossFade("Aggro_Init", 0f, 1);
			if (_rendCache == null)
			{
				_rendCache = _meshRend.materials;
			}
			Play_FamiliarVoxClip();
		}
	}

	public override void Update_SkillBehavior()
	{
		Handle_FamiliarColoration();
		Handle_FamiliarMovement();
		void Handle_FamiliarColoration()
		{
			ScriptableArmorDye scriptableArmorDye = GameManager._current._statLogics._defaultArmorDye;
			if ((bool)_skillObject._parentPlayer)
			{
				int dyeIndex = _skillObject._parentPlayer._pVisual._playerAppearanceStruct._dyeIndex;
				scriptableArmorDye = GameManager._current._statLogics._armorDyes[dyeIndex];
			}
			if (!(_dyeCache == scriptableArmorDye))
			{
				float hue = scriptableArmorDye._dyeParams._hue;
				float saturation = scriptableArmorDye._dyeParams._saturation;
				float brightness = scriptableArmorDye._dyeParams._brightness;
				float contrast = scriptableArmorDye._dyeParams._contrast;
				_rendCache[2].SetFloat(_HUE, hue);
				_rendCache[2].SetFloat(_SATURATION, saturation);
				_rendCache[2].SetFloat(_BRIGHTNESS, brightness);
				_rendCache[2].SetFloat(_CONTRAST, contrast);
				_meshRend.materials = _rendCache;
				ParticleSystem.MainModule main = _particleTrail.main;
				main.startColor = scriptableArmorDye._particleColoration;
				_dyeCache = scriptableArmorDye;
			}
		}
		void Handle_FamiliarMovement()
		{
			StatusEntity statusEntity = null;
			if ((bool)_skillObject._parentPlayer)
			{
				statusEntity = _skillObject._parentPlayer._statusEntity;
			}
			if ((bool)_skillObject._parentCreep)
			{
				statusEntity = _skillObject._parentCreep._statusEntity;
			}
			if ((bool)statusEntity)
			{
				_followParentObj.localPosition = _positionOffset;
				PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
				Vector3 vector = _followParentObj.position;
				Vector3 origin = statusEntity.transform.position + statusEntity.transform.up * 1.5f + statusEntity.transform.right * -1.5f;
				if (physicsScene.SphereCast(origin, 1.25f, statusEntity.transform.right, out var hitInfo, 6.5f, CastLayers.GroundMask))
				{
					vector = hitInfo.point;
					vector.y = _followParentObj.position.y;
					vector -= statusEntity.transform.right * 3.5f;
				}
				bool flag = Vector3.Distance(base.transform.position, vector) <= 0.2f;
				_familiarAnimator.SetBool("_isMoving", !flag);
				Vector3 forward = vector - base.transform.position;
				forward.y = 0f;
				Quaternion rotation = ((!flag) ? Quaternion.LookRotation(forward) : Quaternion.Lerp(base.transform.rotation, _skillObject._parentPlayer.transform.rotation, 12.5f * Time.deltaTime));
				base.transform.SetPositionAndRotation(Vector3.Lerp(base.transform.position, vector, _followStep * Time.deltaTime), rotation);
			}
		}
	}

	public void Play_FamiliarVoxClip()
	{
		if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus)
		{
			_familiarVoxAsrc.PlayOneShot(_familiarVoxClips[Random.Range(0, _familiarVoxClips.Length)], 0.85f);
		}
	}
}
