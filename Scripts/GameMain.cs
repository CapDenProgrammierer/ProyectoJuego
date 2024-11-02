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
	private ColorRect _endZone;
	
	public override void _Ready()
	{
		_gameMap = GetNode("TileMap");
		_towersContainer = GetNode<Node2D>("TowersContainer");
		_enemiesContainer = GetNode<Node2D>("EnemiesContainer");
		_ui = GetNode<CanvasLayer>("UI");

		_towerFactory = new TowerFactory();

		InitializeGame();
		CreateEndZone();
	}

	private void CreateEndZone()
	{
		_endZone = new ColorRect();
		_endZone.Size = new Vector2(20, 600); // Ajusta según el tamaño de tu mapa
		_endZone.Position = new Vector2(800, 0); // Misma X que _endXPosition en Enemy
		_endZone.Color = new Color(1, 0, 0, 0.3f); // Rojo semitransparente
		AddChild(_endZone);
	}

	private void InitializeGame()
	{
		GD.Print("=== Torres Disponibles ===");
		GD.Print("1: Torre Básica (50 oro) - Daño: 50, Rango: 100");
		GD.Print("2: Torre Avanzada (150 oro) - Daño: 100, Rango: 150");
		GD.Print("3: Torre Elite (300 oro) - Daño: 200, Rango: 200");
		GD.Print("4: Torre Master (600 oro) - Daño: 400, Rango: 250");
		GD.Print("5: Torre Ultimate (1200 oro) - Daño: 800, Rango: 300");
		GD.Print("=====================");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			Vector2 clickPosition = GetGlobalMousePosition();
			
			if (mouseButton.Pressed)
			{
				switch (mouseButton.ButtonIndex)
				{
					case MouseButton.Left:
						TryPlaceTower(clickPosition);
						break;
					case MouseButton.Right:
						TrySellTower(clickPosition);
						break;
				}
			}
		}
		else if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed)
			{
				_selectedTowerType = eventKey.Keycode switch
				{
					Key.Key1 => TowerFactory.TowerType.Basic,
					Key.Key2 => TowerFactory.TowerType.Advanced,
					Key.Key3 => TowerFactory.TowerType.Elite,
					Key.Key4 => TowerFactory.TowerType.Master,
					Key.Key5 => TowerFactory.TowerType.Ultimate,
					_ => _selectedTowerType
				};

				string towerName = _selectedTowerType switch
				{
					TowerFactory.TowerType.Basic => "Básica",
					TowerFactory.TowerType.Advanced => "Avanzada",
					TowerFactory.TowerType.Elite => "Elite",
					TowerFactory.TowerType.Master => "Master",
					TowerFactory.TowerType.Ultimate => "Ultimate",
					_ => ""
				};

				if (towerName != "")
				{
					GameManager.Instance.ShowMessage($"Torre {towerName} seleccionada");
				}
			}
		}
	}

	private void TryPlaceTower(Vector2 position)
	{
		Tower tower = _towerFactory.CreateTower(_selectedTowerType);
		if (tower == null) return;

		if (GameManager.Instance.SpendGold(tower.Cost))
		{
			_towersContainer.AddChild(tower);
			tower.Position = position;
		}
		else
		{
			tower.QueueFree();
		}
	}

	private void TrySellTower(Vector2 position)
	{
		var towers = _towersContainer.GetChildren();
		
		foreach (Node node in towers)
		{
			if (node is Tower tower)
			{
				float distance = position.DistanceTo(tower.Position);
				if (distance < 32) // Radio de 32 pixels para detectar el click
				{
					int sellValue = tower.GetSellValue();
					GameManager.Instance.AddGold(sellValue);
					tower.QueueFree();
					GameManager.Instance.ShowMessage($"Torre vendida por {sellValue} oro");
					return;
				}
			}
		}
	}
}
