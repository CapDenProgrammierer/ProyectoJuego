using Godot;
using System;

public partial class Tower : Node2D
{
	protected float _damage;
	protected float _range;
	protected float _attackSpeed;
	protected int _cost;
	
	protected float _attackTimer = 0;
	protected Enemy _currentTarget;

	private bool _initialized = false;
	public int Cost 
	{ 
		get 
		{
			if (!_initialized)
			{
				InitializeTower();
				_initialized = true;
			}
			return _cost;
		} 
	}

	public override void _Ready()
	{
		if (!_initialized)
		{
			InitializeTower();
			_initialized = true;
		}

		var rangeIndicator = new ColorRect();
		rangeIndicator.Color = new Color(0, 1, 0, 0.2f);
		rangeIndicator.Size = new Vector2(_range * 2, _range * 2);
		rangeIndicator.Position = new Vector2(-_range, -_range);
		AddChild(rangeIndicator);
	}

	public override void _Process(double delta)
	{
		_attackTimer += (float)delta;

		if (_currentTarget == null || !IsInstanceValid(_currentTarget) || !_currentTarget.IsAlive)
		{
			_currentTarget = FindNearestEnemy();
		}

		if (_currentTarget != null && _attackTimer >= _attackSpeed)
		{
			Attack();
			_attackTimer = 0;
		}
	}

	protected virtual void InitializeTower()
	{
		_damage = 0;
		_range = 0;
		_attackSpeed = 0;
		_cost = 0;
	}

	protected virtual void Attack()
	{
		if (_currentTarget != null)
		{
			_currentTarget.TakeDamage(_damage);
			GD.Print($"Torre atacando a enemigo. Daño: {_damage}");
		}
	}

	private Enemy FindNearestEnemy()
	{
		var enemies = GetTree().GetNodesInGroup("Enemies");
		Enemy nearest = null;
		float nearestDistance = _range;

		foreach (Node node in enemies)
		{
			if (node is Enemy enemy)
			{
				float distance = GlobalPosition.DistanceTo(enemy.GlobalPosition);
				if (distance <= _range && (nearest == null || distance < nearestDistance))
				{
					nearest = enemy;
					nearestDistance = distance;
				}
			}
		}

		return nearest;
	}
}
