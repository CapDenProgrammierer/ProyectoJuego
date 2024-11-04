using Godot;
using System;

public partial class AdvancedTower : Tower
{
	protected override void InitializeTower()
	{
		dmg = 75;
		range = 125;
		atkSpeed = 0.75f;
		cost = 150;
		rangeCol = new Color(0, 0.5f, 1, 0.1f);
		atkStrat = new AreaDamageAttack();
	}
}
