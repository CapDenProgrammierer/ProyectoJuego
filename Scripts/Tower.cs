using Godot;
using System;

public partial class Tower : Node2D
{
	protected float _damage;
	protected float _range;
	protected float _attackSpeed;
	protected int _cost;
	protected Color _rangeColor = new Color(0, 1, 0, 0.1f); // Color por defecto verde
	
	protected float _attackTimer = 0;
	protected Enemy _currentTarget;
	protected PackedScene _projectileScene;
	protected Sprite2D _towerSprite;
	private RangeIndicator _rangeIndicator;
	private bool _initialized = false;
	private bool _isMouseOver = false;

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
		return _cost / 2; // 50% del costo original
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
			GD.Print($"Sprite de torre ajustado a escala {scale} para alcanzar 64x64 píxeles");
		}
		else
		{
			GD.Print("No se pudo encontrar el sprite o la textura de la torre");
		}

		_projectileScene = GD.Load<PackedScene>("res://Scenes/Projectile.tscn");
	}

	public override void _Process(double delta)
	{
		_attackTimer += (float)delta;

		if (_currentTarget == null || !IsInstanceValid(_currentTarget) || !_currentTarget.IsAlive || !IsEnemyInRange(_currentTarget))
		{
			_currentTarget = FindNearestEnemy();
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

	protected virtual void InitializeTower()
	{
		_damage = 0;
		_range = 0;
		_attackSpeed = 0;
		_cost = 0;
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
		if (_currentTarget != null && IsEnemyInRange(_currentTarget) && _projectileScene != null)
		{
			var projectile = _projectileScene.Instantiate<Projectile>();
			GetTree().Root.AddChild(projectile);
			projectile.GlobalPosition = GlobalPosition;
			projectile.Initialize(_currentTarget, _damage);
			GD.Print($"Torre disparando proyectil. Daño: {_damage}");
		}
	}

	protected bool IsEnemyInRange(Enemy enemy)
	{
		float distance = GlobalPosition.DistanceTo(enemy.GlobalPosition);
		bool inRange = distance <= _range;
		
		if (_currentTarget == enemy && !inRange)
		{
			GD.Print($"Enemigo fuera de rango. Distancia: {distance}, Rango máximo: {_range}");
		}
		
		return inRange;
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

		if (nearest != null)
		{
			GD.Print($"Nuevo objetivo encontrado a distancia: {nearestDistance}");
		}

		return nearest;
	}
}

public partial class RangeIndicator : Node2D
{
	private float _range;
	private Color _color;

	public RangeIndicator(float range, Color color)
	{
		_range = range;
		_color = color;
	}

	public override void _Draw()
	{
		DrawCircle(Vector2.Zero, _range, _color);
	}
}
