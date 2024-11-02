using Godot;
using System;

public partial class GameManager : Node
{
	private static GameManager _instance;
	public static GameManager Instance
	{
		get { return _instance; }
	}

	private int _gold = 100;
	private int _lives = 20;
	private GameUI _gameUI;

	public int Gold { get { return _gold; } }
	public int Lives { get { return _lives; } }

	public override void _EnterTree()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	public override void _Ready()
	{
		_gameUI = GetNode<GameUI>("/root/Main/UI/GameUI");
	}

	public bool SpendGold(int amount)
	{
		if (_gold >= amount)
		{
			_gold -= amount;
			ShowMessage($"Oro gastado: -{amount}");
			return true;
		}
		ShowMessage("No hay suficiente oro");
		return false;
	}

	public void AddGold(int amount)
	{
		_gold += amount;
		ShowMessage($"¡+{amount} oro!");
	}

	public void ShowMessage(string message)
	{
		if (_gameUI != null)
		{
			_gameUI.ShowMessage(message);
		}
	}
}
