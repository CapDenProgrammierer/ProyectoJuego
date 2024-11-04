using Godot;
using System;
using System.Collections.Generic;

public partial class GameEventSystem : Node
{
	private static GameEventSystem instance;
	public static GameEventSystem Instance => instance;

	private List<IGameEventObserver> observers = new();

	public override void _EnterTree()
	{
		if (instance == null)
			instance = this;
	}

	public void AddObserver(IGameEventObserver observer)
	{
		if (!observers.Contains(observer))
			observers.Add(observer);
	}

	public void RemoveObserver(IGameEventObserver observer) 
		=> observers.Remove(observer);

	public void NotifyObservers(GameEvent gameEvent)
	{
		foreach (var obs in observers.ToArray())
			obs.OnGameEvent(gameEvent);
	}
}

public class ResourceEvent : GameEvent
{
	public ResourceType Type { get; }
	public int PreviousAmount { get; }
	public int NewAmount { get; }

	public ResourceEvent(ResourceType type, int prevAmount, int newAmount)
	{
		Type = type;
		PreviousAmount = prevAmount;
		NewAmount = newAmount;
	}
}

public class WaveEvent : GameEvent
{
	public int WaveNumber { get; }
	public WaveState State { get; }
	public int EnemyCount { get; }

	public WaveEvent(int waveNum, WaveState state, int enemyCount)
	{
		WaveNumber = waveNum;
		State = state;
		EnemyCount = enemyCount;
	}
}

public class GameOverEvent : GameEvent
{
	public int FinalWave { get; }
	public int FinalGold { get; }
	public int FinalScore { get; }

	public GameOverEvent(int wave, int gold, int score)
	{
		FinalWave = wave;
		FinalGold = gold;
		FinalScore = score;
	}
}

public class TowerEvent : GameEvent
{
	public TowerEventType Type { get; }
	public Vector2 Position { get; }
	public int Cost { get; }
	public string TowerType { get; }

	public TowerEvent(TowerEventType type, Vector2 pos, int cost, string towerType)
	{
		Type = type;
		Position = pos;
		Cost = cost;
		TowerType = towerType;
	}
}

public class EnemyEvent : GameEvent
{
	public EnemyEventType Type { get; }
	public Vector2 Position { get; }
	public int Reward { get; }
	public float HealthPercentage { get; }

	public EnemyEvent(EnemyEventType type, Vector2 pos, int reward = 0, float health = 0)
	{
		Type = type;
		Position = pos;
		Reward = reward;
		HealthPercentage = health;
	}
}

public class MessageEvent : GameEvent
{
	public string Message { get; }

	public MessageEvent(string msg) => Message = msg;
}

public enum ResourceType
{
	Gold,
	Lives,
	Score
}

public enum WaveState
{
	Starting,
	InProgress,
	Complete
}

public enum TowerEventType
{
	Placed,
	Sold,
	Attacking,
	Upgraded
}

public enum EnemyEventType
{
	Spawned,
	Damaged,
	Killed,
	ReachedEnd
}
