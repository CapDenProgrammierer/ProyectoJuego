using Godot;
using System;

public partial class BasicTower : Tower
{
	protected override void InitializeTower()
	{
		dmg = 50;
		range = 100;
		atkSpeed = 1.0f;
		cost = 50;
		rangeCol = new Color(0, 1, 0, 0.1f);
		atkStrat = new SingleTargetAttack();
	}
}
