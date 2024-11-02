using Godot;
using System;

public class TowerFactory
{
	public enum TowerType
	{
		Basic,
		Advanced,
		Elite,
		Master,
		Ultimate
	}

	private PackedScene _basicTowerScene;
	private PackedScene _advancedTowerScene;
	private PackedScene _eliteTowerScene;
	private PackedScene _masterTowerScene;
	private PackedScene _ultimateTowerScene;

	public TowerFactory()
	{
		_basicTowerScene = GD.Load<PackedScene>("res://Scenes/BasicTower.tscn");
		_advancedTowerScene = GD.Load<PackedScene>("res://Scenes/AdvancedTower.tscn");
		_eliteTowerScene = GD.Load<PackedScene>("res://Scenes/EliteTower.tscn");
		_masterTowerScene = GD.Load<PackedScene>("res://Scenes/MasterTower.tscn");
		_ultimateTowerScene = GD.Load<PackedScene>("res://Scenes/UltimateTower.tscn");
	}

	public Tower CreateTower(TowerType type)
	{
		PackedScene towerScene = type switch
		{
			TowerType.Basic => _basicTowerScene,
			TowerType.Advanced => _advancedTowerScene,
			TowerType.Elite => _eliteTowerScene,
			TowerType.Master => _masterTowerScene,
			TowerType.Ultimate => _ultimateTowerScene,
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
