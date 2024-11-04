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

	private PackedScene basicTower;
	private PackedScene advancedTower;
	private PackedScene eliteTower;
	private PackedScene masterTower;
	private PackedScene ultimateTower;

	public TowerFactory()
	{
		basicTower = GD.Load<PackedScene>("res://Scenes/BasicTower.tscn");
		advancedTower = GD.Load<PackedScene>("res://Scenes/AdvancedTower.tscn");
		eliteTower = GD.Load<PackedScene>("res://Scenes/EliteTower.tscn");
		masterTower = GD.Load<PackedScene>("res://Scenes/MasterTower.tscn");
		ultimateTower = GD.Load<PackedScene>("res://Scenes/UltimateTower.tscn");
	}

	public Tower CreateTower(TowerType type)
	{
		PackedScene towerScene = type switch
		{
			TowerType.Basic => basicTower,
			TowerType.Advanced => advancedTower,
			TowerType.Elite => eliteTower,
			TowerType.Master => masterTower,
			TowerType.Ultimate => ultimateTower,
			_ => null
		};

		return towerScene?.Instantiate<Tower>();
	}
}
