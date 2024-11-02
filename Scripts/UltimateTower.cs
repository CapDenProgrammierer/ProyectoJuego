using Godot;
using System;

public partial class UltimateTower : Tower
{
	protected override void InitializeTower()
	{
		_damage = 800;         // Doble del daño de Master (400)
		_range = 300;          // Mayor rango
		_attackSpeed = 0.2f;   // Más rápido que Master (0.4f)
		_cost = 1200;         // Doble del costo de Master (600)
		_rangeColor = new Color(1, 0, 0, 0.1f);  // Color rojo para el rango
		
		GD.Print($"Ultimate Tower created - Damage: {_damage}, Range: {_range}, Cost: {_cost}");
	}
}
