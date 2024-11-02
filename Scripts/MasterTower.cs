using Godot;
using System;

public partial class MasterTower : Tower
{
	protected override void InitializeTower()
	{
		_damage = 400;         // Doble del daño de Elite (200)
		_range = 250;          // Mayor rango
		_attackSpeed = 0.4f;   // Más rápido que Elite (0.6f)
		_cost = 600;          // Doble del costo de Elite (300)
		_rangeColor = new Color(1, 0.5f, 0, 0.1f);  // Color naranja para el rango
		
		GD.Print($"Master Tower created - Damage: {_damage}, Range: {_range}, Cost: {_cost}");
	}
}
