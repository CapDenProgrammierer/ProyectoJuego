using Godot;
using System;

public partial class GameMain : Node2D
{
	Node levelMap;
	Node2D towerCont;
	Node2D enemyCont;
	CanvasLayer uiLayer;
	TowerFactory towerFactory;
	TowerFactory.TowerType currentTower = TowerFactory.TowerType.Basic;
	ColorRect endMarker;
	
	public override void _Ready()
	{
		levelMap = GetNode("TileMap");
		towerCont = GetNode<Node2D>("TowersContainer");
		enemyCont = GetNode<Node2D>("EnemiesContainer");
		uiLayer = GetNode<CanvasLayer>("UI");

		towerFactory = new TowerFactory();

		ShowHelp();
		CreateEndZone();
	}

	void CreateEndZone()
	{
		endMarker = new ColorRect
		{
			Size = new Vector2(20, 600),
			Position = new Vector2(1100, 0),
			Color = new Color(1, 0, 0, 0.3f)
		};
		AddChild(endMarker);
	}

	void ShowHelp()
	{
		GD.Print("\n=== CONTROLES DEL JUEGO ===");
		GD.Print("1: Torre Básica (50 oro)");
		GD.Print("2: Torre Avanzada (150 oro)");
		GD.Print("3: Torre Elite (300 oro)");
		GD.Print("4: Torre Master (600 oro)");
		GD.Print("5: Torre Ultimate (1200 oro)");
		GD.Print("Click Izq: Construir torre");
		GD.Print("Click Der: Vender torre");
		GD.Print("======================\n");
	}

	public override void _Input(InputEvent evt)
	{
		if (evt is InputEventMouseButton mouseBtn && mouseBtn.Pressed)
		{
			var pos = GetGlobalMousePosition();
			
			switch (mouseBtn.ButtonIndex)
			{
				case MouseButton.Left:
					PlaceTower(pos);
					break;
				case MouseButton.Right:
					SellTower(pos);
					break;
			}
		}
		else if (evt is InputEventKey keyEvt && keyEvt.Pressed)
		{
			HandleTowerSelection(keyEvt.Keycode);
		}
	}

	void HandleTowerSelection(Key key)
	{
		var oldTower = currentTower;
		currentTower = key switch
		{
			Key.Key1 => TowerFactory.TowerType.Basic,
			Key.Key2 => TowerFactory.TowerType.Advanced,
			Key.Key3 => TowerFactory.TowerType.Elite,
			Key.Key4 => TowerFactory.TowerType.Master,
			Key.Key5 => TowerFactory.TowerType.Ultimate,
			_ => currentTower
		};

		if (oldTower != currentTower)
		{
			string towerName = GetTowerName(currentTower);
			if (towerName != "")
				GameManager.Instance?.ShowMessage($"Torre {towerName} seleccionada");
		}
	}

	string GetTowerName(TowerFactory.TowerType type) => type switch
	{
		TowerFactory.TowerType.Basic => "Básica",
		TowerFactory.TowerType.Advanced => "Avanzada",
		TowerFactory.TowerType.Elite => "Elite",
		TowerFactory.TowerType.Master => "Master",
		TowerFactory.TowerType.Ultimate => "Ultimate",
		_ => ""
	};

	void PlaceTower(Vector2 pos)
	{
		var tower = towerFactory.CreateTower(currentTower);
		if (tower == null) return;

		if (GameManager.Instance?.SpendGold(tower.Cost) ?? false)
		{
			towerCont.AddChild(tower);
			tower.Position = pos;
			VisualEffectSystem.Instance?.CreateTowerPlacementEffect(pos);
		}
		else
		{
			tower.QueueFree();
		}
	}

	void SellTower(Vector2 pos)
	{
		foreach (Node node in towerCont.GetChildren())
		{
			if (node is Tower t && pos.DistanceTo(t.Position) < 32)
			{
				int val = t.GetSellValue();
				
				GameEventSystem.Instance?.NotifyObservers(
					new TowerEvent(
						TowerEventType.Sold,
						t.Position,
						val,
						t.GetType().Name
					)
				);
				
				t.QueueFree();
				return;
			}
		}
	}
}
