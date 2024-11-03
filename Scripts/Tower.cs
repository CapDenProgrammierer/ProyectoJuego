using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Tower : Node2D
{
	protected float _damage;
	protected float _range;
	protected float _attackSpeed;
	protected int _cost;
	protected Color _rangeColor = new Color(0, 1, 0, 0.1f);
	
	protected float _attackTimer = 0;
	protected Enemy _currentTarget;
	private static PackedScene _projectileScene;
	protected Sprite2D _towerSprite;
	private RangeIndicator _rangeIndicator;
	private bool _initialized = false;
	private bool _isMouseOver = false;
	protected IAttackStrategy _attackStrategy;

	public static PackedScene ProjectileScene => _projectileScene;
	public float Damage => _damage;

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

	public int GetSellValue()
	{
		return _cost / 2;
	}

	public void UpdateUnit(double delta)
	{
		_attackTimer += (float)delta;

		if (_currentTarget == null || !IsInstanceValid(_currentTarget) || 
			!_currentTarget.IsAlive || !IsEnemyInRange(_currentTarget))
		{
			_currentTarget = FindNearestEnemy();
		}

		if (_currentTarget != null && _attackTimer >= _attackSpeed && 
			IsEnemyInRange(_currentTarget))
		{
			Attack();
			_attackTimer = 0;
		}
	}

	public Vector2 GetPosition()
	{
		return GlobalPosition;
	}

	public override void _Ready()
	{
		if (!_initialized)
		{
			InitializeTower();
			_initialized = true;
		}

		CreateRangeIndicator();
		_towerSprite = GetNode<Sprite2D>("Sprite2D");
		if (_towerSprite != null && _towerSprite.Texture != null)
		{
			float targetSize = 64.0f;
			float imageSize = _towerSprite.Texture.GetWidth();
			float scale = targetSize / imageSize;
			_towerSprite.Scale = new Vector2(scale, scale);
		}

		_projectileScene = GD.Load<PackedScene>("res://Scenes/Projectile.tscn");
		UnitManager.Instance?.RegisterTower(this);
	}

	public override void _ExitTree()
	{
		UnitManager.Instance?.UnregisterTower(this);
	}

	public override void _Process(double delta)
	{
		_attackTimer += (float)delta;

		if (_currentTarget == null || !IsInstanceValid(_currentTarget) || !_currentTarget.IsAlive || !IsEnemyInRange(_currentTarget))
		{
			_currentTarget = FindNearestEnemy();
			if (_currentTarget != null)
			{
				GD.Print($"Objetivo encontrado para {GetType().Name}");
			}
		}

		if (_currentTarget != null && _attackTimer >= _attackSpeed && IsEnemyInRange(_currentTarget))
		{
			Attack();
			_attackTimer = 0;
		}
	}
	

	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			Vector2 mousePosition = GetGlobalMousePosition();
			bool wasMouseOver = _isMouseOver;
			_isMouseOver = mousePosition.DistanceTo(GlobalPosition) < 32;

			if (wasMouseOver != _isMouseOver)
			{
				QueueRedraw();
			}
		}
	}

	public override void _Draw()
	{
		base._Draw();
		
		if (_isMouseOver)
		{
			DrawArc(Vector2.Zero, 36, 0, Mathf.Tau, 32, new Color(1, 1, 1, 0.5f));
			int sellValue = GetSellValue();
			GameManager.Instance?.ShowMessage($"Click derecho para vender por {sellValue} oro");
		}
	}

	private void CreateRangeIndicator()
	{
		if (_rangeIndicator != null)
		{
			_rangeIndicator.QueueFree();
		}

		_rangeIndicator = new RangeIndicator(_range, _rangeColor);
		AddChild(_rangeIndicator);
	}

	protected virtual void Attack()
	{
		if (_currentTarget != null && IsEnemyInRange(_currentTarget))
		{
			if (_attackStrategy != null)
			{
				GD.Print($"Torre {GetType().Name} atacando con estrategia {_attackStrategy.GetType().Name}");
				_attackStrategy.Attack(this, _currentTarget);
			}
			else
			{
				GD.PrintErr($"Error: Torre {GetType().Name} no tiene estrategia de ataque");
			}
		}
	}

	protected bool IsEnemyInRange(Enemy enemy)
	{
		return GlobalPosition.DistanceTo(enemy.GlobalPosition) <= _range;
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

	public List<Enemy> GetNearestEnemies(int count)
	{
		var enemies = GetTree().GetNodesInGroup("Enemies")
			.OfType<Enemy>()
			.Where(e => IsEnemyInRange(e))
			.OrderBy(e => GlobalPosition.DistanceTo(e.GlobalPosition))
			.Take(count)
			.ToList();

		return enemies;
	}

	protected virtual void InitializeTower()
	{
		_damage = 0;
		_range = 0;
		_attackSpeed = 0;
		_cost = 0;
	}
}
