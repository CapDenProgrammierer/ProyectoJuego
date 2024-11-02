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
		// Color verde para el rango de la torre básica
		_rangeColor = new Color(0, 1, 0, 0.1f);
		
		GD.Print($"Basic Tower created - Damage: {_damage}, Range: {_range}, Cost: {_cost}");
	}
}
