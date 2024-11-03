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
		var projectile = Tower.ProjectileScene.Instantiate<Projectile>();
		 if (projectile != null)
		{
			 GD.Print("Creando proyectil de ataque simple");
			tower.GetTree().Root.AddChild(projectile);
			projectile.GlobalPosition = tower.GlobalPosition;
			 projectile.Initialize(target, tower.Damage);
		}
		else
		{
			GD.PrintErr("Error al crear proyectil simple");
		}
	}
}

public class AreaDamageAttack : IAttackStrategy
{
	private float _damageRadius = 120f;

	public void Attack(Tower tower, Enemy target)
	{
		var projectile = Tower.ProjectileScene.Instantiate<Projectile>();
		if (projectile != null)
		{
			GD.Print($"Creando proyectil de área con radio {_damageRadius}");
			tower.GetTree().Root.AddChild(projectile);
			projectile.GlobalPosition = tower.GlobalPosition;
			projectile.InitializeAreaDamage(target, tower.Damage * 0.7f, _damageRadius);
		}
		else
		{
			GD.PrintErr("Error al crear proyectil de área");
		}
	}
}

public class SlowingAttack : IAttackStrategy
{
	private float _slowAmount = 0.5f;
	private float _slowDuration = 3.0f;

	public void Attack(Tower tower, Enemy target)
	{
		var projectile = Tower.ProjectileScene.Instantiate<Projectile>();
		if (projectile != null)
		{
			GD.Print($"Creando proyectil ralentizador: {_slowAmount * 100}% durante {_slowDuration}s");
			tower.GetTree().Root.AddChild(projectile);
			projectile.GlobalPosition = tower.GlobalPosition;
			projectile.InitializeSlowing(target, tower.Damage * 0.6f, _slowAmount, _slowDuration);
		}
		else
		{
			GD.PrintErr("Error al crear proyectil ralentizador");
		}
	}
}

public class MultiTargetAttack : IAttackStrategy
{
	private int _targetCount = 3;

	public void Attack(Tower tower, Enemy mainTarget)
	{
		var enemies = tower.GetNearestEnemies(_targetCount);
		GD.Print($"Ataque múltiple a {enemies.Count} objetivos");

		foreach (var enemy in enemies)
		{
			var projectile = Tower.ProjectileScene.Instantiate<Projectile>();
			if (projectile != null)
			{
				tower.GetTree().Root.AddChild(projectile);
				projectile.GlobalPosition = tower.GlobalPosition;
				projectile.Initialize(enemy, tower.Damage * 0.8f);
			}
		}
	}
}
