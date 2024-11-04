using Godot;
using System;

public partial class UltimateTower : Tower
{
	protected override void InitializeTower()
	{
		dmg = 150;
		range = 300;
		atkSpeed = 0.2f;
		cost = 1200;
		rangeCol = new Color(1, 0, 0, 0.1f);
		atkStrat = new AreaDamageAttack();
	}
}
