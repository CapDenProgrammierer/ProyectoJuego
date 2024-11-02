using Godot;
using System;

public partial class Projectile : Node2D
{
	private Vector2 _targetPosition;
	private float _speed = 300f;
	private float _damage;
	private Enemy _target;

	public void Initialize(Enemy target, float damage)
	{
		_target = target;
		_damage = damage;
		
		// Crear sprite del proyectil
		var sprite = new Sprite2D();
		var colorRect = new ColorRect();
		colorRect.Size = new Vector2(8, 8); // Tamaño pequeño para el proyectil
		colorRect.Position = new Vector2(-4, -4); // Centrar el colorRect
		colorRect.Color = Colors.Yellow; // Color amarillo para el proyectil
		AddChild(colorRect);
	}

	public override void _Process(double delta)
	{
		if (_target == null || !IsInstanceValid(_target))
		{
			QueueFree();
			return;
		}

		Vector2 direction = (_target.GlobalPosition - GlobalPosition).Normalized();
		Position += direction * _speed * (float)delta;

		// Si el proyectil está lo suficientemente cerca del objetivo, impacta
		if (GlobalPosition.DistanceTo(_target.GlobalPosition) < 10)
		{
			_target.TakeDamage(_damage);
			QueueFree();
		}
	}
}
