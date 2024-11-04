using Godot;
using System;

public partial class MasterTower : Tower
{
	protected override void InitializeTower()
	{
		dmg = 200;
		range = 250;
		atkSpeed = 0.4f;
		cost = 600;
		rangeCol = new Color(1, 0.5f, 0, 0.1f);
		atkStrat = new SlowingAttack();
	}
}
