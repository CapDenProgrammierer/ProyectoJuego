using Godot;
using System;

public partial class MasterTower : Tower
{
	protected override void InitializeTower()
	{
		_damage = 400;
		_range = 250;
		_attackSpeed = 0.4f;
		_cost = 600;
		_rangeColor = new Color(1, 0.5f, 0, 0.1f);
		_attackStrategy = new SlowingAttack();
	}
}
