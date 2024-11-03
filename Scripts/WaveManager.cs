using Godot;
using System;
using System.Collections.Generic;

public partial class WaveManager : Node
{
	private int _currentWave = 0;
	private int _enemiesInWave = 0;
	private int _enemiesSpawned = 0;
	private float _spawnTimer = 0;
	private float _spawnDelay = 1.0f;
	private float _waveCooldown = 5.0f;
	private float _waveTimer = 0;
	private bool _isWaveActive = false;
	private bool _isWaitingForNextWave = false;
	private Node2D _enemiesContainer;
	private PackedScene _enemyScene;
	private Random _random = new Random();
	private float _initialDelay = 3.0f;
	private float _initialTimer = 0f;
	private bool _gameStarted = false;
	public int CurrentWave => _currentWave;

	public override void _Ready()
	{
		_enemyScene = GD.Load<PackedScene>("res://Scenes/Enemy.tscn");
		_enemiesContainer = GetNode<Node2D>("/root/Main/EnemiesContainer");
		_gameStarted = false;
		_initialTimer = 0f;
		GameEventSystem.Instance?.NotifyObservers(new MessageEvent("¡Prepárate! La primera oleada comenzará en 3 segundos"));
	}

	public override void _Process(double delta)
	{
		if (!_gameStarted)
		{
			_initialTimer += (float)delta;
			if (_initialTimer >= _initialDelay)
			{
				_gameStarted = true;
				StartNextWave();
			}
			return;
		}

		if (_isWaitingForNextWave)
		{
			_waveTimer += (float)delta;
			if (_waveTimer >= _waveCooldown)
			{
				_isWaitingForNextWave = false;
				GiveWaveCompletion();
				StartNextWave();
			}
			return;
		}

		if (_isWaveActive)
		{
			if (_enemiesSpawned < _enemiesInWave)
			{
				_spawnTimer += (float)delta;
				if (_spawnTimer >= _spawnDelay)
				{
					SpawnEnemy();
					_spawnTimer = 0;
				}
			}
			else if (GetTree().GetNodesInGroup("Enemies").Count == 0)
			{
				_isWaveActive = false;
				_isWaitingForNextWave = true;
				_waveTimer = 0;
				GameEventSystem.Instance?.NotifyObservers(new WaveEvent(_currentWave, WaveState.Complete, _enemiesInWave));
			}
		}
	}
	
	private void GiveWaveCompletion()
	{
		GameEventSystem.Instance?.NotifyObservers(new ResourceEvent(ResourceType.Lives, GameManager.Instance.Lives, GameManager.Instance.Lives + 20));
		if (GameManager.Instance != null)
		{
			GameManager.Instance.AddLives(20);
		}
		GameEventSystem.Instance?.NotifyObservers(new MessageEvent($"¡Oleada completada! +20 vidas"));
	}
	
	private void StartNextWave()
	{
		_currentWave++;
		_enemiesInWave = 5 + _currentWave;
		_enemiesSpawned = 0;
		_isWaveActive = true;
		_waveTimer = 0;
		_spawnDelay = Mathf.Max(0.5f, 1.0f - (_currentWave * 0.05f));
		GameEventSystem.Instance?.NotifyObservers(new WaveEvent(_currentWave, WaveState.Starting, _enemiesInWave));
		GameEventSystem.Instance?.NotifyObservers(new MessageEvent($"¡Oleada {_currentWave} comenzando! Enemigos: {_enemiesInWave}"));
	}

	private void SpawnEnemy()
	{
		float difficultyMultiplier = 1 + ((_currentWave - 1) * 0.25f);
		bool isElite = _currentWave >= 2 && _random.NextDouble() < 0.3;
		bool isFast = !isElite && _random.NextDouble() < 0.4;
		IMovementStrategy movementStrategy = _random.NextDouble() < 0.5 ? 
			new ZigZagMovement() : new StraightLineMovement();

		Enemy enemy = _enemyScene.Instantiate<Enemy>();
		
		if (isElite)
		{
			enemy.Initialize(
				health: (int)(200 * difficultyMultiplier),
				damage: (int)(20 * difficultyMultiplier),
				speed: 60,
				goldReward: 75,
				isElite: true,
				isFast: false,
				movementStrategy: new ZigZagMovement()
			);
		}
		else if (isFast)
		{
			enemy.Initialize(
				health: (int)(80 * difficultyMultiplier),
				damage: (int)(15 * difficultyMultiplier),
				speed: 120,
				goldReward: 50,
				isElite: false,
				isFast: true,
				movementStrategy: new StraightLineMovement()
			);
		}
		else
		{
			enemy.Initialize(
				health: (int)(100 * difficultyMultiplier),
				damage: (int)(10 * difficultyMultiplier),
				speed: 80,
				goldReward: 25,
				isElite: false,
				isFast: false,
				movementStrategy: movementStrategy
			);
		}

		_enemiesContainer.AddChild(enemy);
		_enemiesSpawned++;
	}
}
