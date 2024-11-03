using Godot;
using System;
using System.Collections.Generic;

public interface IGameEventObserver
{
	void OnGameEvent(GameEvent gameEvent);
}

public abstract class GameEvent
{
	public DateTime Timestamp { get; private set; }

	 protected GameEvent()
	{
		Timestamp = DateTime.Now;
	}
}

public class ResourceEvent : GameEvent
{
	public ResourceType Type { get; private set; }
	public int PreviousAmount { get; private set; }
	public int NewAmount { get; private set; }

	public ResourceEvent(ResourceType type, int previousAmount, int newAmount)
	{
		Type = type;
		PreviousAmount = previousAmount;
		NewAmount = newAmount;
	}
}

public class WaveEvent : GameEvent
{
	public int WaveNumber { get; private set; }
	public WaveState State { get; private set; }
	public int EnemyCount { get; private set; }

	 public WaveEvent(int waveNumber, WaveState state, int enemyCount)
	{
		WaveNumber = waveNumber;
		State = state;
		EnemyCount = enemyCount;
	}
}

public class TowerEvent : GameEvent
{
	public TowerEventType Type { get; private set; }
	public Vector2 Position { get; private set; }
	public int Cost { get; private set; }
	public string TowerType { get; private set; }

	public TowerEvent(TowerEventType type, Vector2 position, int cost, string towerType)
	{
		Type = type;
		Position = position;
		Cost = cost;
		TowerType = towerType;
	}
}

  public class EnemyEvent : GameEvent
{
	public EnemyEventType Type { get; private set; }
	  public Vector2 Position { get; private set; }
	public int Reward { get; private set; }
	public float HealthPercentage { get; private set; }

	public EnemyEvent(EnemyEventType type, Vector2 position, int reward = 0, float healthPercentage = 0)
	{
		  Type = type;
		Position = position;
		Reward = reward;
		HealthPercentage = healthPercentage;
	}
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

 public class GameOverEvent : GameEvent
{
	public int FinalWave { get; private set; }
	  public int FinalGold { get; private set; }
	public int FinalScore { get; private set; }

	public GameOverEvent(int finalWave, int finalGold, int finalScore)
	{
		FinalWave = finalWave;
		FinalGold = finalGold;
		FinalScore = finalScore;
	}
}

public class MessageEvent : GameEvent
{
	public string Message { get; private set; }
	
	  public MessageEvent(string message)
	{
		Message = message;
	}
}

public partial class GameEventSystem : Node
{
	private static GameEventSystem _instance;
	  private List<IGameEventObserver> _observers = new List<IGameEventObserver>();

	public static GameEventSystem Instance
	{
		get { return _instance; }
	}

	public override void _EnterTree()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	  public void AddObserver(IGameEventObserver observer)
	{
		if (!_observers.Contains(observer))
		{
			_observers.Add(observer);
		}
	}

	public void RemoveObserver(IGameEventObserver observer)
	{
		_observers.Remove(observer);
	}

	public void NotifyObservers(GameEvent gameEvent)
	 {
		foreach (var observer in _observers.ToArray())
		{
			observer.OnGameEvent(gameEvent);
		}
	}
}
