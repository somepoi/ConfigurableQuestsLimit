using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	public static PoolManager _current;

	public Transform containerParent;

	public GameObject _emptyContainerFab;

	public float _defaultInactivityTimer = 80f;

	public List<ObjectPool> objectPools = new List<ObjectPool>();

	private ObjectPool requestedObjectPool;

	private void Awake()
	{
		if (_current == null)
		{
			_current = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		Initialize_PoolManager();
	}

	private void Update()
	{
		Iterate_ObjectPools();
	}

	private void Initialize_PoolManager()
	{
		for (int i = 0; i < objectPools.Count; i++)
		{
			objectPools[i]._objectPool = GenerateObjects(objectPools[i]._objectCount, objectPools[i]);
		}
	}

	private void Iterate_ObjectPools()
	{
		for (int i = 0; i < objectPools.Count; i++)
		{
			if (objectPools[i]._inactivityTimer > 0f)
			{
				objectPools[i]._flaggedInactivity = true;
				objectPools[i]._inactivityTimer -= Time.deltaTime;
			}
			if (objectPools[i]._inactivityTimer <= 0f && objectPools[i]._flaggedInactivity)
			{
				Object.Destroy(objectPools[i]._objectContainer);
				objectPools.RemoveAt(i);
			}
		}
	}

	private List<GameObject> GenerateObjects(int numOfObjects, ObjectPool objPool)
	{
		for (int i = 0; i < numOfObjects; i++)
		{
			GameObject gameObject = Object.Instantiate(objPool._spawnedPrefab);
			gameObject.name = objPool._spawnedPrefab.name;
			if ((bool)objPool._objectContainer)
			{
				gameObject.transform.SetParent(objPool._objectContainer.transform);
			}
			gameObject.SetActive(value: false);
			objPool._objectPool.Add(gameObject);
		}
		return objPool._objectPool;
	}

	public GameObject GenerateObject(GameObject _requestedObject, string _tag, float _time)
	{
		for (int i = 0; i < objectPools.Count; i++)
		{
			if (objectPools[i]._spawnedPrefab == _requestedObject || objectPools[i]._poolTag == _tag)
			{
				requestedObjectPool = objectPools[i];
				break;
			}
			if (i >= objectPools.Count - 1)
			{
				GameObject gameObject = Object.Instantiate(_emptyContainerFab, base.transform.position, Quaternion.identity);
				gameObject.transform.SetParent(containerParent);
				gameObject.name = "_container" + _requestedObject.name;
				ObjectPool item = new ObjectPool
				{
					_poolTag = _requestedObject.name,
					_objectContainer = gameObject,
					_objectPool = new List<GameObject>(),
					_spawnedPrefab = _requestedObject,
					_objectCount = 3,
					_timer = 0f,
					_inactivityTimer = _defaultInactivityTimer
				};
				objectPools.Add(item);
				requestedObjectPool = item;
			}
		}
		for (int j = 0; j < requestedObjectPool._objectPool.Count; j++)
		{
			if (!(requestedObjectPool._objectPool[j] != null))
			{
				continue;
			}
			GameObject gameObject2 = requestedObjectPool._objectPool[j];
			if (!gameObject2.activeInHierarchy)
			{
				gameObject2.SetActive(value: true);
				if (requestedObjectPool._flaggedInactivity)
				{
					requestedObjectPool._inactivityTimer = _defaultInactivityTimer;
				}
				if (_time > 0f)
				{
					StartCoroutine(DisableObject_Timer(gameObject2, _time));
				}
				return gameObject2;
			}
		}
		GameObject gameObject3 = Object.Instantiate(requestedObjectPool._spawnedPrefab);
		gameObject3.name = requestedObjectPool._spawnedPrefab.name;
		if ((bool)requestedObjectPool._objectContainer)
		{
			gameObject3.transform.SetParent(requestedObjectPool._objectContainer.transform);
		}
		requestedObjectPool._objectPool.Add(gameObject3);
		if (_time > 0f)
		{
			StartCoroutine(DisableObject_Timer(gameObject3, _time));
		}
		else if (requestedObjectPool._timer > 0f)
		{
			StartCoroutine(DisableObject_Timer(gameObject3, requestedObjectPool._timer));
		}
		if (requestedObjectPool._flaggedInactivity)
		{
			requestedObjectPool._inactivityTimer = _defaultInactivityTimer;
		}
		return gameObject3;
	}

	private IEnumerator DisableObject_Timer(GameObject _obj, float _timer)
	{
		yield return new WaitForSeconds(_timer);
		if (!(_obj == null))
		{
			Init_ReturnObjectToContainer(_obj);
		}
	}

	public void Init_ReturnObjectToContainer(GameObject _obj)
	{
		if (_obj == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < objectPools.Count; i++)
		{
			if (objectPools[i]._spawnedPrefab.name == _obj.name)
			{
				_obj.transform.SetParent(objectPools[i]._objectContainer.transform);
				if (objectPools[i]._flaggedInactivity)
				{
					objectPools[i]._inactivityTimer = _defaultInactivityTimer;
				}
				flag = true;
				break;
			}
		}
		if (flag)
		{
			_obj.SetActive(value: false);
		}
		else
		{
			Object.Destroy(_obj);
		}
	}
}
