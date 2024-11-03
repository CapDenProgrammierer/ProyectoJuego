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
	public float Amplitude { get; set; } = 150f;
	public float Frequency { get; set; } = 1.0f;
	private float _timePassed = 0f;

	public Vector2 GetMovement(Enemy enemy, float delta)
	{
		_timePassed += delta;
		
		// Movimiento horizontal normal
		float horizontalMove = enemy.CurrentSpeed * delta;
		
		// Movimiento vertical más suave
		float verticalMove = Mathf.Sin(_timePassed * Frequency * Mathf.Pi) * Amplitude * delta;
		
		return new Vector2(horizontalMove, verticalMove);
	}
}
