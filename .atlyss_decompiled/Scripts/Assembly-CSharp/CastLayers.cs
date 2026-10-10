using UnityEngine;

public static class CastLayers
{
	private static int _cullLayer = -1;

	private static int _defaultLayer = -1;

	private static int _playerMask = -1;

	private static int _creepMask = -1;

	private static int _creepPushMask = -1;

	private static int _itemObjectMask = -1;

	private static int _targetableMask = -1;

	private static int _pathfinderMask = -1;

	private static int _groundMask = -1;

	private static int _waterMask = -1;

	private static int _footstepMask = -1;

	private static int _aiNavMask = -1;

	private static int _interactableMask = -1;

	public static int CullLayer
	{
		get
		{
			if (_cullLayer == -1)
			{
				_cullLayer = LayerMask.NameToLayer("Cull Object Render");
			}
			return _cullLayer;
		}
	}

	public static int DefaultLayer
	{
		get
		{
			if (_defaultLayer == -1)
			{
				_defaultLayer = LayerMask.NameToLayer("Default");
			}
			return _defaultLayer;
		}
	}

	public static int PlayerMask
	{
		get
		{
			if (_playerMask == -1)
			{
				_playerMask = LayerMask.GetMask("PlayerObject", "Player_TriggerCollider");
			}
			return _playerMask;
		}
	}

	public static int CreepMask
	{
		get
		{
			if (_creepMask == -1)
			{
				_creepMask = LayerMask.GetMask("Creep_TriggerCollider");
			}
			return _creepMask;
		}
	}

	public static int CreepPushMask
	{
		get
		{
			if (_creepPushMask == -1)
			{
				_creepPushMask = LayerMask.GetMask("Creep_PushCollider", "CreepObject");
			}
			return _creepPushMask;
		}
	}

	public static int ItemObjectMask
	{
		get
		{
			if (_itemObjectMask == -1)
			{
				_itemObjectMask = LayerMask.GetMask("ItemObject");
			}
			return _itemObjectMask;
		}
	}

	public static int TargetableMask
	{
		get
		{
			if (_targetableMask == -1)
			{
				_targetableMask = LayerMask.GetMask("Ground", "Creep_TriggerCollider");
			}
			return _targetableMask;
		}
	}

	public static int PathfinderMask
	{
		get
		{
			if (_pathfinderMask == -1)
			{
				_pathfinderMask = LayerMask.GetMask("Ground", "PlayerObject", "Player_TriggerCollider");
			}
			return _pathfinderMask;
		}
	}

	public static int GroundMask
	{
		get
		{
			if (_groundMask == -1)
			{
				_groundMask = LayerMask.GetMask("Ground");
			}
			return _groundMask;
		}
	}

	public static int WaterMask
	{
		get
		{
			if (_waterMask == -1)
			{
				_waterMask = LayerMask.GetMask("Water");
			}
			return _waterMask;
		}
	}

	public static int FootstepMask
	{
		get
		{
			if (_footstepMask == -1)
			{
				_footstepMask = LayerMask.GetMask("Ground", "Water");
			}
			return _footstepMask;
		}
	}

	public static int AINavMask
	{
		get
		{
			if (_aiNavMask == -1)
			{
				_aiNavMask = LayerMask.GetMask("AI_Navigation");
			}
			return _aiNavMask;
		}
	}

	public static int InteractableMask
	{
		get
		{
			if (_interactableMask == -1)
			{
				_interactableMask = LayerMask.GetMask("Interactable");
			}
			return _interactableMask;
		}
	}
}
