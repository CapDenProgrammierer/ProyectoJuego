using Godot;
using System;

public partial class EliteTower : Tower
{
	protected override void InitializeTower()
	{
		_damage = 200;
		_range = 200;
		_attackSpeed = 0.6f;
		_cost = 300;
		_rangeColor = new Color(1, 0, 1, 0.1f);
		_attackStrategy = new MultiTargetAttack();
	}
}
