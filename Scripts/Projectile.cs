using Godot;
using System;

public partial class Projectile : Node2D
{
	Vector2 targetPos;
	float speed = 300f;
	float dmg;
	Enemy target;
	bool isAoe;
	float aoeDist;
	bool isSlowing;
	float slowAmt;
	float slowTime;
	ColorRect visual;

	public void Initialize(Enemy tgt, float damage)
	{
		target = tgt;
		dmg = damage;
		isAoe = false;
		isSlowing = false;
		CreateBullet(Colors.Yellow);
	}

	public void InitializeAreaDamage(Enemy tgt, float damage, float radius)
	{
		target = tgt;
		dmg = damage;
		isAoe = true;
		aoeDist = radius;
		CreateBullet(Colors.Orange);
	}

	public void InitializeSlowing(Enemy tgt, float damage, float slowAmount, float slowDuration)
	{
		target = tgt;
		dmg = damage;
		isSlowing = true;
		slowAmt = slowAmount;
		slowTime = slowDuration;
		CreateBullet(Colors.Blue);
	}

	void CreateBullet(Color color)
	{
		var bullet = new ColorRect();
		bullet.Size = new Vector2(8, 8);
		bullet.Position = new Vector2(-4, -4);
		bullet.Color = color;
		AddChild(bullet);
	}

	public override void _Process(double delta)
	{
		if (target == null || !IsInstanceValid(target))
		{
			QueueFree();
			return;
		}

		var dir = (target.GlobalPosition - GlobalPosition).Normalized();
		Position += dir * speed * (float)delta;

		if (GlobalPosition.DistanceTo(target.GlobalPosition) < 10)
		{
			if (isAoe)
				HitArea();
			else if (isSlowing)
				HitWithSlow();
			else
				target.TakeDamage(dmg);
				
			QueueFree();
		}
	}

	void HitArea()
	{
		var enemies = GetTree().GetNodesInGroup("Enemies");
		foreach (Node n in enemies)
		{
			if (n is Enemy e && IsInstanceValid(e))
			{
				float dist = GlobalPosition.DistanceTo(e.GlobalPosition);
				if (dist <= aoeDist)
				{
					float dmgMult = 1 - (dist / aoeDist);
					e.TakeDamage(dmg * dmgMult);
				}
			}
		}
	}

	void HitWithSlow()
	{
		target.TakeDamage(dmg);
		target.ApplySlow(slowAmt, slowTime);
	}
}
