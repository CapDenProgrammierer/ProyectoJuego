using Godot;
using System;

public partial class EliteTower : Tower
{
	protected override void InitializeTower()
	{
		_damage = 200;         // Doble del daño de Advanced (100)
		_range = 200;          // Mayor rango
		_attackSpeed = 0.6f;   // Más rápido que Advanced (0.8f)
		_cost = 300;          // Doble del costo de Advanced (150)
		_rangeColor = new Color(1, 0, 1, 0.1f);  // Color púrpura para el rango
		
		GD.Print($"Elite Tower created - Damage: {_damage}, Range: {_range}, Cost: {_cost}");
	}
}
