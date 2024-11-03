using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
	private static GameManager _instance;
	public static GameManager Instance
	{
		get { return _instance; }
	}

	private GameUI _gameUI;
	private GameOverScreen _gameOverScreen;
	private WaveManager _waveManager;
	private bool _isPaused = false;
	public bool IsPaused => _isPaused;

	private int _gold = 100;
	private int _lives = 20;
	private int _score = 0;
	private bool _isGameOver = false;

	public int Gold { get { return _gold; } }
	public int Lives { get { return _lives; } }
	public int Score { get { return _score; } }
	public bool IsGameOver { get { return _isGameOver; } }

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
		_waveManager = GetNode<WaveManager>("/root/Main/WaveManager");
		
		_gameOverScreen = new GameOverScreen();
		GetNode<CanvasLayer>("/root/Main/UI").AddChild(_gameOverScreen);

		GameEventSystem.Instance?.AddObserver(new GameManagerObserver(this));

		GameEventSystem.Instance?.NotifyObservers(new ResourceEvent(
			ResourceType.Gold,
			0,
			_gold
		));
		
		GameEventSystem.Instance?.NotifyObservers(new ResourceEvent(
			ResourceType.Lives,
			0,
			_lives
		));
	}

	  private class GameManagerObserver : IGameEventObserver
	{
		private readonly GameManager _gameManager;

		public GameManagerObserver(GameManager gameManager)
		{
			_gameManager = gameManager;
		}

		public void OnGameEvent(GameEvent gameEvent)
		{
			switch (gameEvent)
			{
				case EnemyEvent enemyEvent:
					if (enemyEvent.Type == EnemyEventType.Killed)
					{
						_gameManager.AddGold(enemyEvent.Reward);
					}
					else if (enemyEvent.Type == EnemyEventType.ReachedEnd)
					{
						GD.Print("Enemigo alcanzó la base"); 
					}
					break;

				case TowerEvent towerEvent:
					if (towerEvent.Type == TowerEventType.Sold)
					{
						_gameManager.AddGold(towerEvent.Cost);
					}
					break;
			}
		}
	}

	 public override void _UnhandledInput(InputEvent @event)
	{
		if (_isGameOver && @event.IsPressed())
		{
			if (@event is InputEventKey || @event is InputEventMouseButton)
			{
				GD.Print("Input detectado durante Game Over, reiniciando juego"); 
				RestartGame();
			}
		}
	}

	public void ShowMessage(string message)
	{
		GameEventSystem.Instance?.NotifyObservers(new MessageEvent(message));
	}

	 private void SetGamePaused(bool paused)
	{
		GD.Print($"Estableciendo pausa: {paused}"); 
		_isPaused = paused;
		GetTree().Paused = paused;
		
		if (_gameUI != null)
		{
			_gameUI.ProcessMode = paused ? Node.ProcessModeEnum.Always : Node.ProcessModeEnum.Inherit;
		}
		
		if (_gameOverScreen != null)
		{
			_gameOverScreen.ProcessMode = Node.ProcessModeEnum.Always;
		}
	}

	public bool SpendGold(int amount)
	{
		if (_isGameOver) return false;

		if (_gold >= amount)
		{
			int previousGold = _gold;
			_gold -= amount;
			GameEventSystem.Instance?.NotifyObservers(new ResourceEvent(
				ResourceType.Gold, 
				previousGold, 
				_gold
			));
			return true;
		}
		
		ShowMessage("No hay suficiente oro");
		return false;
	}

	 public void TakeDamage(int damage)
{
	if (_isGameOver) return;

	int previousLives = _lives;
	_lives -= damage;

	VisualEffectSystem.Instance?.CreateBaseHitEffect(new Vector2(1100, 300));
	
	GameEventSystem.Instance?.NotifyObservers(new MessageEvent(
		$"¡La base recibió {damage} de daño! Vidas restantes: {_lives}"
	));

	GameEventSystem.Instance?.NotifyObservers(new ResourceEvent(
		ResourceType.Lives,
		previousLives,
		_lives
	));

	if (_lives <= 0)
	{
		_lives = 0;
		CallDeferred(nameof(InitiateGameOver));
	}
}

	private void InitiateGameOver()
	{
		if (_isGameOver) return;

		_isGameOver = true;
		
		if (_gameOverScreen == null)
		{
			_gameOverScreen = new GameOverScreen();
			GetNode<CanvasLayer>("/root/Main/UI").AddChild(_gameOverScreen);
		}

		int currentWave = _waveManager != null ? _waveManager.CurrentWave : 0;
		
		_gameOverScreen.Show(currentWave, _gold);
		_gameOverScreen.Visible = true;
		_gameOverScreen.ZIndex = 999;

		GameEventSystem.Instance?.NotifyObservers(new MessageEvent(
			"¡GAME OVER! Presiona cualquier tecla para reiniciar"
		));

		GameEventSystem.Instance?.NotifyObservers(new GameOverEvent(
			currentWave,
			_gold,
			_score
		));

		SetGamePaused(true);
	}
	public void AddLives(int amount)
	{
		if (_isGameOver) return;

		int previousLives = _lives;
		_lives += amount;
		
		GameEventSystem.Instance.NotifyObservers(new ResourceEvent(
			ResourceType.Lives,
			previousLives,
			_lives
		));
	}

public void AddGold(int amount)
	{
		if (_isGameOver) return;

		int previousGold = _gold;
		int previousScore = _score;
		
		_gold += amount;
		_score += amount;

		GameEventSystem.Instance?.NotifyObservers(new ResourceEvent(
			ResourceType.Gold,
			previousGold,
			_gold
		));

		GameEventSystem.Instance?.NotifyObservers(new ResourceEvent(
			ResourceType.Score,
			previousScore,
			_score
		));
	}

	  private void GameOver()
	{
		if (_isGameOver) return;

		_isGameOver = true;
		GD.Print("Game Over activado"); 
		
		if (_gameOverScreen != null)
		{
			int currentWave = _waveManager != null ? _waveManager.CurrentWave : 0;
			_gameOverScreen.Show(currentWave, _gold);
			
			
			_gameOverScreen.Visible = true;
			_gameOverScreen.ZIndex = 100; 
			
			CallDeferred(nameof(SetGamePaused), true);
		}
		else
		{
			GD.PrintErr("_gameOverScreen es null"); 
		}

		GameEventSystem.Instance?.NotifyObservers(new GameOverEvent(
			_waveManager?.CurrentWave ?? 0,
			_gold,
			_score
		));
		
		GD.Print("=== GAME OVER ===");
		GD.Print($"Oleada alcanzada: {_waveManager?.CurrentWave}");
		GD.Print($"Oro final: {_gold}");
		GD.Print($"Puntuación final: {_score}");
	}
	
	public void RestartGame()
	{
		SetGamePaused(false);
		
		_isGameOver = false;
		_gold = 100;
		_lives = 20;
		_score = 0;
		
		GetTree().ReloadCurrentScene();
		
		GD.Print("=== Juego Reiniciado ===");
	}

	public void QuitGame()
	{
		GetTree().Quit();
	}

	public override void _Notification(int what)
	{
		if (what == NotificationApplicationResumed)
		{
			if (_gameUI != null)
			{
				_gameUI.ProcessMode = Node.ProcessModeEnum.Always;
			}
			if (_gameOverScreen != null && _isGameOver)
			{
				_gameOverScreen.ProcessMode = Node.ProcessModeEnum.Always;
			}
		}
	}
}
