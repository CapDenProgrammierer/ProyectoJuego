using Godot;
using System;

public interface IMovementStrategy
{
	Vector2 GetMovement(Enemy enemy, float delta);
}

public class StraightLineMovement : IMovementStrategy
{
	public Vector2 GetMovement(Enemy enemy, float delta)
	{
		return new Vector2(enemy.CurrentSpeed * delta, 0);
	}
}

public class ZigZagMovement : IMovementStrategy
{
	private float _amplitude = 100f;
	private float _frequency = 2f;
	private float _timePassed = 0f;

	public Vector2 GetMovement(Enemy enemy, float delta)
	{
		_timePassed += delta;
		float verticalOffset = Mathf.Cos(_timePassed * _frequency) * _amplitude * delta;
		return new Vector2(enemy.CurrentSpeed * delta, verticalOffset);
	}
}
