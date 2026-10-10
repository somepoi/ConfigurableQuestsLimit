using System;

[Serializable]
public struct GamepadInfo(int id, GamepadType type)
{
	public int id = id;

	public GamepadType type = type;
}
