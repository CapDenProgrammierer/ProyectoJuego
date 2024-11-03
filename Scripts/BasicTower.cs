using Godot;
using System;

	public partial class BasicTower : Tower
{
	protected override void InitializeTower()
	{
		_damage = 50;
		_range = 100;
			_attackSpeed = 1.0f;
		_cost = 50;
		_rangeColor = new Color(0, 1, 0, 0.1f);
		_attackStrategy = new SingleTargetAttack();
	}
}
