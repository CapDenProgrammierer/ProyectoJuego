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
	public int Gold { get { return _gold; } }
	public int Lives { get { return _lives; } }

	public override void _EnterTree()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	public bool SpendGold(int amount)
	{
		if (_gold >= amount)
		{
			_gold -= amount;
			return true;
		}
		return false;
	}
}
