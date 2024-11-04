using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
	static GameManager instance;
	public static GameManager Instance => instance;

	GameUI gameUi;
	GameOverScreen goScreen;
	WaveManager waveMan;
	bool gamePaused;
	public bool IsPaused => gamePaused;

	int playerGold = 100;
	int playerLives = 20;
	int playerScore = 0;
	bool isGameOver;

	public int Gold => playerGold;
	public int Lives => playerLives;
	public int Score => playerScore;
	public bool IsGameOver => isGameOver;

	public override void _EnterTree()
	{
		if (instance == null)
			instance = this;
	}

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;

		gameUi = GetNode<GameUI>("/root/Main/UI/GameUI");
		waveMan = GetNode<WaveManager>("/root/Main/WaveManager");
		
		goScreen = new GameOverScreen();
		GetNode<CanvasLayer>("/root/Main/UI").AddChild(goScreen);

		GameEventSystem.Instance?.AddObserver(new GameManagerObserver(this));
		
		var eventSys = GameEventSystem.Instance;
		if (eventSys != null)
		{
			eventSys.NotifyObservers(new ResourceEvent(ResourceType.Gold, 0, playerGold));
			eventSys.NotifyObservers(new ResourceEvent(ResourceType.Lives, 0, playerLives));
		}
	}

	private class GameManagerObserver : IGameEventObserver
	{
		readonly GameManager gm;

		public GameManagerObserver(GameManager gameManager)
		{
			gm = gameManager;
		}

		public void OnGameEvent(GameEvent gameEvent)
		{
			switch (gameEvent)
			{
				case EnemyEvent enemyEvt:
					if (enemyEvt.Type == EnemyEventType.Killed)
						gm.AddGold(enemyEvt.Reward);
					break;

				case TowerEvent towerEvt:
					if (towerEvt.Type == TowerEventType.Sold)
						gm.AddGold(towerEvt.Cost);
					break;
			}
		}
	}

	public void ShowMessage(string msg) 
		=> GameEventSystem.Instance?.NotifyObservers(new MessageEvent(msg));

	void SetGamePaused(bool paused)
	{
		gamePaused = paused;
		GetTree().Paused = paused;
		
		if (gameUi != null)
			gameUi.ProcessMode = paused ? ProcessModeEnum.Always : ProcessModeEnum.Inherit;
		
		if (goScreen != null)
			goScreen.ProcessMode = ProcessModeEnum.Always;
	}

	public bool SpendGold(int amount)
	{
		if (isGameOver) return false;

		if (playerGold >= amount)
		{
			int prevGold = playerGold;
			playerGold -= amount;
			GameEventSystem.Instance?.NotifyObservers(new ResourceEvent(ResourceType.Gold, prevGold, playerGold));
			return true;
		}
		
		ShowMessage("No hay suficiente oro");
		return false;
	}

	public void TakeDamage(int dmg)
	{
		if (isGameOver) return;

		int prevLives = playerLives;

		if (dmg >= playerLives)
		{
			playerLives = 0;
			isGameOver = true;
			
			GameEventSystem.Instance?.NotifyObservers(new ResourceEvent(ResourceType.Lives, prevLives, playerLives));
			GameEventSystem.Instance?.NotifyObservers(new MessageEvent("¡La base ha sido destruida!"));

			GetTree().CallGroup("Enemies", "queue_free");
			CallDeferred(nameof(InitiateGameOver));
			return;
		}

		playerLives -= dmg;
		
		VisualEffectSystem.Instance?.CreateBaseHitEffect(new Vector2(1100, 300));
		
		var eventSys = GameEventSystem.Instance;
		if (eventSys != null)
		{
			eventSys.NotifyObservers(new ResourceEvent(ResourceType.Lives, prevLives, playerLives));
			eventSys.NotifyObservers(new MessageEvent($"¡La base recibió {dmg} de daño! Vidas: {playerLives}"));
		}
	}

	void InitiateGameOver()
	{
		if (goScreen == null)
		{
			goScreen = new GameOverScreen();
			var ui = GetNode<CanvasLayer>("/root/Main/UI");
			ui?.AddChild(goScreen);
		}

		GetTree().Paused = true;
		goScreen.Show(waveMan?.CurrentWave ?? 0, playerGold);
		
		GameEventSystem.Instance?.NotifyObservers(new GameOverEvent(
			waveMan?.CurrentWave ?? 0,
			playerGold,
			playerScore
		));
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (isGameOver && evt.IsPressed())
		{
			if (evt is InputEventKey || evt is InputEventMouseButton)
				RestartGame();
		}
	}

	public void AddGold(int amount)
	{
		if (isGameOver) return;

		int prevGold = playerGold;
		int prevScore = playerScore;
		
		playerGold += amount;
		playerScore += amount;

		var eventSys = GameEventSystem.Instance;
		if (eventSys != null)
		{
			eventSys.NotifyObservers(new ResourceEvent(ResourceType.Gold, prevGold, playerGold));
			eventSys.NotifyObservers(new ResourceEvent(ResourceType.Score, prevScore, playerScore));
		}
	}

	public void AddLives(int amount)
	{
		if (isGameOver) return;
		
		int prevLives = playerLives;
		playerLives += amount;
		
		GameEventSystem.Instance?.NotifyObservers(new ResourceEvent(
			ResourceType.Lives,
			prevLives,
			playerLives
		));
	}

	void RestartGame()
	{
		if (goScreen != null)
		{
			goScreen.QueueFree();
			goScreen = null;
		}

		GetTree().Paused = false;
		
		isGameOver = false;
		playerGold = 100;
		playerLives = 20;
		playerScore = 0;
		
		GetTree().CallGroup("Enemies", "queue_free");
		GetTree().ReloadCurrentScene();
	}

	public void QuitGame() => GetTree().Quit();

	public override void _Input(InputEvent evt)
	{
		if (!isGameOver) return;
		
		if (evt.IsPressed())
			RestartGame();
	}

	public override void _Notification(int what)
	{
		if (what == NotificationApplicationResumed)
		{
			if (gameUi != null)
				gameUi.ProcessMode = ProcessModeEnum.Always;
			
			if (goScreen != null && isGameOver)
				goScreen.ProcessMode = ProcessModeEnum.Always;
		}
	}
}
