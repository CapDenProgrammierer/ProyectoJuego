using Godot;
using System;

public partial class AdvancedTower : Tower
{
	protected override void InitializeTower()
	{
		_damage = 100;
		_range = 150;
		_attackSpeed = 0.8f;
		_cost = 150;
		_rangeColor = new Color(0, 0.5f, 1, 0.1f);
		_attackStrategy = new AreaDamageAttack();
	}
}
