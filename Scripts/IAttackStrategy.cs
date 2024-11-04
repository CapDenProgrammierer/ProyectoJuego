using Godot;
using System.Collections.Generic;
using System.Linq;

public interface IAttackStrategy
{
	void Attack(Tower tower, Enemy target);
}

public class SingleTargetAttack : IAttackStrategy
{
	public void Attack(Tower tower, Enemy target)
	{
		var bullet = Tower.ProjectileScene.Instantiate<Projectile>();
		if (bullet != null)
		{
			tower.GetTree().Root.AddChild(bullet);
			bullet.GlobalPosition = tower.GlobalPosition;
			bullet.Initialize(target, tower.Damage);
		}
	}
}

public class AreaDamageAttack : IAttackStrategy
{
	float blastRadius = 120f;

	public void Attack(Tower tower, Enemy target)
	{
		var bullet = Tower.ProjectileScene.Instantiate<Projectile>();
		if (bullet != null)
		{
			tower.GetTree().Root.AddChild(bullet);
			bullet.GlobalPosition = tower.GlobalPosition;
			bullet.InitializeAreaDamage(target, tower.Damage * 0.7f, blastRadius);
		}
	}
}

public class SlowingAttack : IAttackStrategy
{
	float slowPower = 0.5f;
	float slowDur = 3.0f;

	public void Attack(Tower tower, Enemy target)
	{
		var bullet = Tower.ProjectileScene.Instantiate<Projectile>();
		if (bullet != null)
		{
			tower.GetTree().Root.AddChild(bullet);
			bullet.GlobalPosition = tower.GlobalPosition;
			bullet.InitializeSlowing(target, tower.Damage * 0.6f, slowPower, slowDur);
		}
	}
}

public class MultiTargetAttack : IAttackStrategy
{
	int maxTargets = 3;

	public void Attack(Tower tower, Enemy mainTarget)
	{
		var enemies = tower.GetNearestEnemies(maxTargets);
		
		foreach (var enemy in enemies)
		{
			var bullet = Tower.ProjectileScene.Instantiate<Projectile>();
			if (bullet != null)
			{
				tower.GetTree().Root.AddChild(bullet);
				bullet.GlobalPosition = tower.GlobalPosition;
				bullet.Initialize(enemy, tower.Damage * 0.8f);
			}
		}
	}
}
