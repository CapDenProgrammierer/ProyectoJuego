using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
	// Singleton
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


	// Estado del juego
	private int _gold = 100;
	private int _lives = 20;
	private int _score = 0;
	private bool _isGameOver = false;

	// Propiedades públicas
	public int Gold { get { return _gold; } }
	public int Lives { get { return _lives; } }
	public int Score { get { return _score; } }
	public bool IsGameOver { get { return _isGameOver; } }

	// Sistema de eventos
	public delegate void GameStateChangedHandler();
	public event GameStateChangedHandler OnGoldChanged;
	public event GameStateChangedHandler OnLivesChanged;
	public event GameStateChangedHandler OnGameOver;

	public override void _EnterTree()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	public override void _Ready()
	{
		// Obtener referencias a nodos necesarios
		_gameUI = GetNode<GameUI>("/root/Main/UI/GameUI");
		_waveManager = GetNode<WaveManager>("/root/Main/WaveManager");
		
		// Crear y configurar la pantalla de Game Over
		_gameOverScreen = new GameOverScreen();
		GetNode<CanvasLayer>("/root/Main/UI").AddChild(_gameOverScreen);

		GD.Print("=== Nuevo Juego Iniciado ===");
		GD.Print($"Oro inicial: {_gold}");
		GD.Print($"Vidas iniciales: {_lives}");
	}

 public override void _UnhandledInput(InputEvent @event)
	{
		if (_isGameOver && @event.IsPressed())
		{
			// Si el juego terminó y se presiona cualquier tecla o botón
			if (@event is InputEventKey || @event is InputEventMouseButton)
			{
				RestartGame();
			}
		}
	}

	// Métodos para gestionar el oro
	public bool SpendGold(int amount)
	{
		if (_isGameOver) return false;

		if (_gold >= amount)
		{
			_gold -= amount;
			OnGoldChanged?.Invoke();
			ShowMessage($"Oro gastado: -{amount}");
			return true;
		}
		
		ShowMessage("No hay suficiente oro");
		return false;
	}

	public void AddGold(int amount)
	{
		if (_isGameOver) return;

		_gold += amount;
		_score += amount; // El oro ganado suma al puntaje
		OnGoldChanged?.Invoke();
		ShowMessage($"¡+{amount} oro!");
	}

	// Métodos para gestionar las vidas
	public void TakeDamage(int damage)
	{
		if (_isGameOver) return;

		_lives -= damage;
		OnLivesChanged?.Invoke();
		
		ShowMessage($"¡Base dañada! -{damage} vidas");
		GD.Print($"Daño recibido: {damage}. Vidas restantes: {_lives}");

		if (_lives <= 0)
		{
			_lives = 0;
			GameOver();
		}
	}

	public void AddLives(int amount)
	{
		if (_isGameOver) return;

		_lives += amount;
		OnLivesChanged?.Invoke();
		ShowMessage($"¡+{amount} vidas!");
	}

	// Método para mostrar mensajes
	public void ShowMessage(string message)
	{
		if (_gameUI != null)
		{
			_gameUI.ShowMessage(message);
		}
	}

	// Gestión del Game Over
	 private void GameOver()
	{
		if (_isGameOver) return;

		_isGameOver = true;
		ShowMessage("¡Game Over!");
		
		// Disparar evento de Game Over
		OnGameOver?.Invoke();
		
		// Mostrar pantalla de Game Over antes de pausar
		if (_gameOverScreen != null)
		{
			int currentWave = _waveManager != null ? _waveManager.CurrentWave : 0;
			_gameOverScreen.Show(currentWave, _gold);
			
			// Usar CallDeferred para pausar en el próximo frame
			CallDeferred("SetGamePaused", true);
		}
		
		// Registrar estadísticas finales
		GD.Print("=== GAME OVER ===");
		GD.Print($"Oleada alcanzada: {_waveManager?.CurrentWave}");
		GD.Print($"Oro final: {_gold}");
		GD.Print($"Puntuación final: {_score}");
	}

 private void SetGamePaused(bool paused)
	{
		_isPaused = paused;
		GetTree().Paused = paused;
		
		// Asegurarnos de que la UI y la pantalla de Game Over no se vean afectadas por la pausa
		if (_gameUI != null)
		{
			_gameUI.ProcessMode = paused ? Node.ProcessModeEnum.Always : Node.ProcessModeEnum.Inherit;
		}
		
		if (_gameOverScreen != null)
		{
			_gameOverScreen.ProcessMode = Node.ProcessModeEnum.Always;
		}
	}

	// Método para reiniciar el juego
	 public void RestartGame()
	{
		// Primero desactivamos la pausa
		SetGamePaused(false);
		
		_isGameOver = false;
		_gold = 100;
		_lives = 20;
		_score = 0;
		
		// Recargar la escena actual
		GetTree().ReloadCurrentScene();
		
		GD.Print("=== Juego Reiniciado ===");
	}

	// Método para salir del juego
	public void QuitGame()
	{
		GetTree().Quit();
	}
	 public override void _Notification(int what)
	{
		if (what == NotificationApplicationResumed)
		{
			// Si la aplicación vuelve al primer plano, asegurarse de que la UI sea visible
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
