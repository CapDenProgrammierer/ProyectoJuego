using Godot;
using System;

public partial class GameMain : Node2D
{
	private Node _gameMap;
	private Node2D _towersContainer;
	private Node2D _enemiesContainer;
	private CanvasLayer _ui;
	private TowerFactory _towerFactory;
	private TowerFactory.TowerType _selectedTowerType = TowerFactory.TowerType.Basic;
	
	// Nuevas variables para enemigos
	private PackedScene _enemyScene;
	private float _enemySpawnTime = 2.0f;
	private float _timeSinceLastSpawn = 0;

	public override void _Ready()
	{
		_gameMap = GetNode("TileMap");
		_towersContainer = GetNode<Node2D>("TowersContainer");
		_enemiesContainer = GetNode<Node2D>("EnemiesContainer");
		_ui = GetNode<CanvasLayer>("UI");

		_towerFactory = new TowerFactory();
		_enemyScene = GD.Load<PackedScene>("res://Scenes/Enemy.tscn");

		InitializeGame();
	}

	private void InitializeGame()
	{
		GD.Print($"Game Started! Gold: {GameManager.Instance.Gold}, Lives: {GameManager.Instance.Lives}");
		GD.Print("Presiona 1 para Torre Básica (50 oro)");
		GD.Print("Presiona 2 para Torre Avanzada (150 oro)");
	}

	public override void _Process(double delta)
	{
		_timeSinceLastSpawn += (float)delta;
		
		if (_timeSinceLastSpawn >= _enemySpawnTime)
		{
			SpawnEnemy();
			_timeSinceLastSpawn = 0;
		}
	}

	private void SpawnEnemy()
	{
		if (_enemyScene == null) 
		{
			GD.Print("Error: Enemy scene not loaded");
			return;
		}

		Enemy enemy = _enemyScene.Instantiate<Enemy>();
		if (enemy != null)
		{
			_enemiesContainer.AddChild(enemy);
			enemy.Position = new Vector2(0, 300);
			GD.Print("Enemigo generado");
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed)
			{
				if (eventKey.Keycode == Key.Key1)
				{
					_selectedTowerType = TowerFactory.TowerType.Basic;
					GD.Print("Torre Básica seleccionada");
				}
				else if (eventKey.Keycode == Key.Key2)
				{
					_selectedTowerType = TowerFactory.TowerType.Advanced;
					GD.Print("Torre Avanzada seleccionada");
				}
			}
		}

		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
			{
				TryPlaceTower(GetGlobalMousePosition());
			}
		}
	}

	private void TryPlaceTower(Vector2 position)
	{
		Tower tower = _towerFactory.CreateTower(_selectedTowerType);
		if (tower == null) return;

		GD.Print($"Intentando colocar torre. Costo: {tower.Cost}, Oro disponible: {GameManager.Instance.Gold}");

		if (GameManager.Instance.SpendGold(tower.Cost))
		{
			_towersContainer.AddChild(tower);
			tower.Position = position;
			GD.Print($"Torre colocada en {position}. Oro restante: {GameManager.Instance.Gold}");
		}
		else
		{
			GD.Print($"No hay suficiente oro. Necesitas: {tower.Cost}, Tienes: {GameManager.Instance.Gold}");
			tower.QueueFree();
		}
	}
}
