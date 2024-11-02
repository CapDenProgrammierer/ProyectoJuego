using Godot;
using System;

public partial class AdvancedTower : Tower
{
	protected override void InitializeTower()
	{
		_damage = 100;
		_range = 150;
		_attackSpeed = 0.8f;
		_cost = 150;
		// Color azul para el rango de la torre avanzada
		_rangeColor = new Color(0, 0.5f, 1, 0.1f);
		
		GD.Print($"Advanced Tower created - Damage: {_damage}, Range: {_range}");
	}
}
