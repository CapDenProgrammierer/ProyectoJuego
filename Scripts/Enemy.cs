using Godot;
using System;

public partial class Enemy : Node2D
{
	private float _speed = 100;
	private int _health = 100;
	private int _damage = 10;
	
	public bool IsAlive => _health > 0;

	public void TakeDamage(float damage)
	{
		_health -= (int)damage;
		GD.Print($"Enemigo dañado. Vida restante: {_health}");
		
		if (_health <= 0)
		{
			QueueFree();
			GD.Print("Enemigo eliminado");
		}
	}

	public override void _Ready()
	{
		AddToGroup("Enemies");
		
		var colorRect = new ColorRect();
		colorRect.Size = new Vector2(32, 32);
		colorRect.Position = new Vector2(-16, -16);
		colorRect.Color = Colors.Red;
		AddChild(colorRect);
	}

	public override void _Process(double delta)
	{
		Position += Vector2.Right * _speed * (float)delta;
	}
}
