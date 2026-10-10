using UnityEngine;

[RequireComponent(typeof(SkillObject))]
public abstract class SkillObjectBehavior : MonoBehaviour
{
	[Header("Skill Object General Parameters")]
	public SkillObject _skillObject;

	public Renderer[] _miscRenderCache;

	public MeshRenderer[] _meshRenderCache;

	public AudioSource[] _aSrcCache;

	public bool _destroyWhileParentLoading;

	[Header("Runtime Passive Skill Objects")]
	public GameObject[] _runtimeObjects;

	public void Activate_RuntimeObject(int _index, bool _isTrue)
	{
		_runtimeObjects[_index].SetActive(_isTrue);
	}

	private void Awake()
	{
		_skillObject = GetComponent<SkillObject>();
		_miscRenderCache = GetComponentsInChildren<Renderer>(includeInactive: true);
		_meshRenderCache = GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		_aSrcCache = GetComponentsInChildren<AudioSource>(includeInactive: true);
	}

	private void Start()
	{
		Init_SkillBehavior();
	}

	private void OnEnable()
	{
		Init_SkillBehavior();
	}

	private void Update()
	{
		if ((bool)Player._mainPlayer)
		{
			bool flag = false;
			if ((bool)_skillObject._parentPlayer)
			{
				flag = Player._mainPlayer.Network_playerMapInstance == _skillObject._parentPlayer.Network_playerMapInstance;
			}
			if ((bool)_skillObject._parentCreep)
			{
				flag = Player._mainPlayer.Network_playerMapInstance == _skillObject._parentCreep._parentCreepSpawner._mapInstance;
			}
			for (int i = 0; i < _miscRenderCache.Length; i++)
			{
				_miscRenderCache[i].enabled = flag;
			}
			for (int j = 0; j < _meshRenderCache.Length; j++)
			{
				_meshRenderCache[j].enabled = flag;
			}
			for (int k = 0; k < _aSrcCache.Length; k++)
			{
				_aSrcCache[k].mute = !flag;
			}
		}
		if ((bool)_skillObject._parentPlayer && _skillObject._parentPlayer._bufferingStatus)
		{
			if (_destroyWhileParentLoading)
			{
				Object.Destroy(base.gameObject);
			}
		}
		else
		{
			Update_SkillBehavior();
		}
	}

	public abstract void Init_SkillBehavior();

	public abstract void Update_SkillBehavior();

	public abstract void End_SkillBehavior();
}
