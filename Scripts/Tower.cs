using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Tower : Node2D
{
	protected float dmg;
	protected float range;
	protected float atkSpeed;
	protected int cost;
	protected Color rangeCol = new Color(0, 1, 0, 0.1f);
	
	protected float timeToNextAttack = 0;
	protected Enemy target;
	private static PackedScene projScene;
	protected Sprite2D sprite;
	private RangeIndicator rangeInd;
	private bool init = false;
	private bool mouseOver = false;
	protected IAttackStrategy atkStrat;

	public static PackedScene ProjectileScene => projScene;
	public float Damage => dmg;

	public int Cost 
	{ 
		get 
		{
			if (!init) {
				InitializeTower();
				init = true;
			}
			return cost;
		} 
	}

	public int GetSellValue() => cost / 2;

	public void UpdateUnit(double dt)
	{
		timeToNextAttack += (float)dt;

		if (target == null || !IsInstanceValid(target) || 
			!target.IsAlive || !IsEnemyInRange(target))
		{
			target = FindNearestEnemy();
		}

		if (target != null && timeToNextAttack >= atkSpeed && 
			IsEnemyInRange(target))
		{
			Attack();
			timeToNextAttack = 0;
		}
	}

	public Vector2 GetPosition() => GlobalPosition;

	public override void _Ready()
	{
		if (!init) {
			InitializeTower();
			init = true;
		}

		CreateRangeIndicator();
		sprite = GetNode<Sprite2D>("Sprite2D");
		if (sprite != null && sprite.Texture != null)
		{
			float size = 64.0f;
			float imgSize = sprite.Texture.GetWidth();
			float scl = size / imgSize;
			sprite.Scale = new Vector2(scl, scl);
		}

		projScene = GD.Load<PackedScene>("res://Scenes/Projectile.tscn");
		UnitManager.Instance?.RegisterTower(this);
	}

	public override void _ExitTree()
	{
		UnitManager.Instance?.UnregisterTower(this);
	}

	public override void _Process(double dt)
	{
		timeToNextAttack += (float)dt;

		if (target == null || !IsInstanceValid(target) || !target.IsAlive || !IsEnemyInRange(target))
		{
			target = FindNearestEnemy();
		}

		if (target != null && timeToNextAttack >= atkSpeed && IsEnemyInRange(target))
		{
			Attack();
			timeToNextAttack = 0;
		}
	}

	public override void _Input(InputEvent evt)
	{
		if (evt is InputEventMouseMotion mouseMov)
		{
			Vector2 mousePos = GetGlobalMousePosition();
			bool wasOver = mouseOver;
			mouseOver = mousePos.DistanceTo(GlobalPosition) < 32;

			if (wasOver != mouseOver)
				QueueRedraw();
		}
	}

	public override void _Draw()
	{
		base._Draw();
		
		if (mouseOver)
		{
			DrawArc(Vector2.Zero, 36, 0, Mathf.Tau, 32, new Color(1, 1, 1, 0.5f));
			int sellVal = GetSellValue();
			GameManager.Instance?.ShowMessage($"Click derecho para vender por {sellVal} oro");
		}
	}

	private void CreateRangeIndicator()
	{
		if (rangeInd != null)
			rangeInd.QueueFree();

		rangeInd = new RangeIndicator(range, rangeCol);
		AddChild(rangeInd);
	}

	protected virtual void Attack()
	{
		if (target != null && IsEnemyInRange(target))
		{
			if (atkStrat != null)
				atkStrat.Attack(this, target);
		}
	}

	protected bool IsEnemyInRange(Enemy enemy)
		=> GlobalPosition.DistanceTo(enemy.GlobalPosition) <= range;

	private Enemy FindNearestEnemy()
	{
		var enemies = GetTree().GetNodesInGroup("Enemies");
		Enemy nearest = null;
		float nearestDist = range;

		foreach (Node n in enemies)
		{
			if (n is Enemy enemy)
			{
				float dist = GlobalPosition.DistanceTo(enemy.GlobalPosition);
				if (dist <= range && (nearest == null || dist < nearestDist))
				{
					nearest = enemy;
					nearestDist = dist;
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
		dmg = 0;
		range = 0;
		atkSpeed = 0;
		cost = 0;
	}
}
