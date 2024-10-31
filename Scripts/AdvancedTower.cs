using Godot;
using System;

public partial class AdvancedTower : Tower
{
	protected override void InitializeTower()
	{
		_damage = 25;
		_range = 150;
		_attackSpeed = 0.8f;
		_cost = 150;
		
		GD.Print($"Advanced Tower created - Damage: {_damage}, Range: {_range}");
	}
}
