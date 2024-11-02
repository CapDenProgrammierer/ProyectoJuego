using Godot;
using System;

public partial class UltimateTower : Tower
{
	protected override void InitializeTower()
	{
		_damage = 800;
		_range = 300;
		_attackSpeed = 0.2f;
		_cost = 1200;
		_rangeColor = new Color(1, 0, 0, 0.1f);
		_attackStrategy = new AreaDamageAttack();
	}
}
