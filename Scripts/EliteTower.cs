using Godot;
using System;

public partial class EliteTower : Tower
{
	protected override void InitializeTower()
	{
		dmg = 125;
		range = 200;
		atkSpeed = 0.6f;
		cost = 300;
		rangeCol = new Color(1, 0, 1, 0.1f);
		atkStrat = new MultiTargetAttack();
	}
}
