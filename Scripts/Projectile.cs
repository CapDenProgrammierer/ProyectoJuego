using Godot;
using System;

public partial class Projectile : Node2D
{
	private Vector2 _targetPosition;
	private float _speed = 300f;
	private float _damage;
	private Enemy _target;
	private bool _isAreaDamage;
	private float _damageRadius;
	private bool _isSlowing;
	private float _slowAmount;
	private float _slowDuration;
	private ColorRect _visual;

	public void Initialize(Enemy target, float damage)
	{
		_target = target;
		_damage = damage;
		_isAreaDamage = false;
		_isSlowing = false;
		CreateProjectileSprite(Colors.Yellow);
		GD.Print($"Proyectil simple inicializado. Daño: {damage}");
	}

	public void InitializeAreaDamage(Enemy target, float damage, float radius)
	{
		_target = target;
		_damage = damage;
		_isAreaDamage = true;
		_damageRadius = radius;
		CreateProjectileSprite(Colors.Orange);
		GD.Print($"Proyectil de área inicializado. Daño: {damage}, Radio: {radius}");
	}

	public void InitializeSlowing(Enemy target, float damage, float slowAmount, float slowDuration)
	{
		_target = target;
		_damage = damage;
		_isSlowing = true;
		_slowAmount = slowAmount;
		_slowDuration = slowDuration;
		CreateProjectileSprite(Colors.Blue);
		GD.Print($"Proyectil ralentizador inicializado. Daño: {damage}, Ralentización: {slowAmount*100}%");
	}

	private void CreateProjectileSprite(Color color)
	{
		var colorRect = new ColorRect();
		colorRect.Size = new Vector2(8, 8);
		colorRect.Position = new Vector2(-4, -4);
		colorRect.Color = color;
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

		if (GlobalPosition.DistanceTo(_target.GlobalPosition) < 10)
		{
			if (_isAreaDamage)
			{
				ApplyAreaDamage();
			}
			else if (_isSlowing)
			{
				ApplySlowingEffect();
			}
			else
			{
				_target.TakeDamage(_damage);
			}
			QueueFree();
		}
	}

	private void ApplyAreaDamage()
	{
		var enemies = GetTree().GetNodesInGroup("Enemies");
		foreach (Node node in enemies)
		{
			if (node is Enemy enemy && IsInstanceValid(enemy))
			{
				float distance = GlobalPosition.DistanceTo(enemy.GlobalPosition);
				if (distance <= _damageRadius)
				{
					float damageMultiplier = 1 - (distance / _damageRadius);
					enemy.TakeDamage(_damage * damageMultiplier);
					GD.Print($"Daño de área aplicado: {_damage * damageMultiplier}");
				}
			}
		}
	}

	private void ApplySlowingEffect()
	{
		_target.TakeDamage(_damage);
		_target.ApplySlow(_slowAmount, _slowDuration);
		GD.Print($"Efecto de ralentización aplicado: {_slowAmount*100}% durante {_slowDuration}s");
	}
}
