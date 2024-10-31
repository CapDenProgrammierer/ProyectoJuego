using Godot;
using System;

public partial class BasicTower : Tower
{
	protected override void InitializeTower()
	{
		_damage = 10;
		_range = 100;
		_attackSpeed = 1.0f;
		_cost = 50;
		
		GD.Print($"Basic Tower created - Damage: {_damage}, Range: {_range}, Cost: {_cost}");
	}
}
