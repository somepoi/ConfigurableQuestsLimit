using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameWorldManager : MonoBehaviour
{
	public static GameWorldManager _current;

	[Header("Day/Night Cycle")]
	public WorldTime _worldTime;

	public ClockSetting _clockSetting;

	public int _timeDisplay = 7;

	public float _dayNightCycleInterval = 285.9f;

	public float _dayNightCycleSmooth = 0.1f;

	private float _currentDayNightCycleBuffer;

	[Header("Day/Night Audio")]
	public AudioSource _dayNightASrc;

	public AudioClip _rooster;

	public AudioClip _wolf;

	[Header("Instancing")]
	public List<DungeonPatternQueue> _dungeonPatternQueues = new List<DungeonPatternQueue>();

	private void Awake()
	{
		_current = this;
	}

	private void Start()
	{
		if (Random.Range(0, 2) == 1)
		{
			_worldTime = WorldTime.NIGHT;
			_timeDisplay = Random.Range(6, 11);
			_clockSetting = ClockSetting.PM;
		}
		else
		{
			_worldTime = WorldTime.DAY;
			_timeDisplay = Random.Range(7, 12);
			_clockSetting = ClockSetting.AM;
		}
	}

	public static Queue<T> ShuffleQueue<T>(Queue<T> originalQueue)
	{
		List<T> list = new List<T>(originalQueue);
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = Random.Range(0, num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
		return new Queue<T>(list);
	}

	private Queue<int> Enqueue_DungeonIndex(MapInstance _mapInstance)
	{
		Queue<int> queue = new Queue<int>();
		for (int i = 0; i < _mapInstance._patternInstance._instancePatterns.Length; i++)
		{
			queue.Enqueue(i);
		}
		return ShuffleQueue(queue);
	}

	public void Create_NewDungeonQueue(MapInstance _mapInstance)
	{
		if (!_mapInstance._patternInstance)
		{
			return;
		}
		Queue<int> queue = new Queue<int>();
		for (int i = 0; i < _dungeonPatternQueues.Count; i++)
		{
			if (_dungeonPatternQueues[i]._dungeonInstanceName == _mapInstance._mapName)
			{
				if (_dungeonPatternQueues[i]._patternQueue.Count <= 0)
				{
					queue = Enqueue_DungeonIndex(_mapInstance);
					_dungeonPatternQueues[i]._patternQueue = queue;
				}
				return;
			}
		}
		queue = Enqueue_DungeonIndex(_mapInstance);
		DungeonPatternQueue item = new DungeonPatternQueue
		{
			_dungeonInstanceName = _mapInstance._mapName,
			_patternQueue = queue
		};
		_dungeonPatternQueues.Add(item);
	}

	public int Retrieve_NextDungeonQueue(MapInstance _mapInstance)
	{
		if (!_mapInstance._patternInstance)
		{
			return 0;
		}
		DungeonPatternQueue dungeonPatternQueue = new DungeonPatternQueue();
		for (int i = 0; i < _dungeonPatternQueues.Count; i++)
		{
			if (_dungeonPatternQueues[i]._dungeonInstanceName == _mapInstance._mapName)
			{
				dungeonPatternQueue = _dungeonPatternQueues[i];
			}
		}
		dungeonPatternQueue._patternQueue.Dequeue();
		if (dungeonPatternQueue._patternQueue.Count <= 0)
		{
			dungeonPatternQueue._patternQueue = Enqueue_DungeonIndex(_mapInstance);
		}
		return dungeonPatternQueue._patternQueue.Peek();
	}

	private void Update()
	{
		Server_DayNightCycleRuntime();
	}

	private void Server_DayNightCycleRuntime()
	{
		if (!NetworkServer.active || _dayNightCycleInterval <= 0f)
		{
			return;
		}
		if (_currentDayNightCycleBuffer < _dayNightCycleInterval)
		{
			_currentDayNightCycleBuffer += Time.deltaTime;
			return;
		}
		_currentDayNightCycleBuffer = 0f;
		_timeDisplay++;
		if (_timeDisplay > 11 && _clockSetting == ClockSetting.AM && _worldTime == WorldTime.DAY)
		{
			_clockSetting = ClockSetting.PM;
		}
		else if (_timeDisplay > 11 && _clockSetting == ClockSetting.PM && _worldTime == WorldTime.NIGHT)
		{
			_clockSetting = ClockSetting.AM;
		}
		if (_timeDisplay > 12)
		{
			_timeDisplay = 1;
		}
		if (_timeDisplay == 6 && _clockSetting == ClockSetting.AM)
		{
			_worldTime = WorldTime.DAY;
		}
		else if (_timeDisplay == 8 && _clockSetting == ClockSetting.PM)
		{
			_worldTime = WorldTime.NIGHT;
		}
	}
}
