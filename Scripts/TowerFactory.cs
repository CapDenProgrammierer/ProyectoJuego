using Godot;
using System;

public class TowerFactory
{
	public enum TowerType
	{
		Basic,
		Advanced
	}

	private PackedScene _basicTowerScene;
	private PackedScene _advancedTowerScene;

	public TowerFactory()
	{
		_basicTowerScene = GD.Load<PackedScene>("res://Scenes/BasicTower.tscn");
		_advancedTowerScene = GD.Load<PackedScene>("res://Scenes/AdvancedTower.tscn");
	}

	public Tower CreateTower(TowerType type)
	{
		PackedScene towerScene = type switch
		{
			TowerType.Basic => _basicTowerScene,
			TowerType.Advanced => _advancedTowerScene,
			_ => null
		};

		if (towerScene == null) 
		{
			GD.Print("No se pudo cargar la escena de la torre");
			return null;
		}

		Tower tower = towerScene.Instantiate<Tower>();
		if (tower != null)
		{
			GD.Print($"Torre creada del tipo {type} con costo {tower.Cost}");
		}
		return tower;
	}
}
