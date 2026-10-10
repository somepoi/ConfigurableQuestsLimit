using System;
using System.Collections.Generic;

[Serializable]
public class DungeonPatternQueue
{
	public string _dungeonInstanceName = string.Empty;

	public Queue<int> _patternQueue = new Queue<int>();
}
