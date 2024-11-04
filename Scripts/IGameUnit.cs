using Godot;
using System;

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

public interface IGameUnit
{
	void UpdateUnit(double delta);
	void TakeDamage(float damage);
	Vector2 GetPosition();
	void ApplyEffect(IEffect effect);
}

public interface IEffect
{
	void Apply(IGameUnit unit);
}
