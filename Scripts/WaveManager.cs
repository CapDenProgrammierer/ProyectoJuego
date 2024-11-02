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
	public int CurrentWave => _currentWave;

	public override void _Ready()
	{
		_enemyScene = GD.Load<PackedScene>("res://Scenes/Enemy.tscn");
		_enemiesContainer = GetNode<Node2D>("/root/Main/EnemiesContainer");
		StartNextWave();
	}

	public override void _Process(double delta)
	{
		if (_isWaitingForNextWave)
		{
			_waveTimer += (float)delta;
			if (_waveTimer >= _waveCooldown)
			{
				_isWaitingForNextWave = false;
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
				GD.Print($"=== Oleada {_currentWave} Completada ===");
				GameManager.Instance.ShowMessage($"Oleada {_currentWave} completada! Siguiente oleada en {_waveCooldown} segundos");
			}
		}
	}

	private void StartNextWave()
	{
		_currentWave++;
		_enemiesInWave = 5 + _currentWave; // Aumenta la cantidad de enemigos por oleada
		_enemiesSpawned = 0;
		_isWaveActive = true;
		_waveTimer = 0;
		_spawnDelay = Mathf.Max(0.5f, 1.0f - (_currentWave * 0.05f)); // Spawn más rápido en oleadas superiores

		GD.Print($"=== Oleada {_currentWave} ===");
		GD.Print($"Enemigos en esta oleada: {_enemiesInWave}");
		GD.Print($"Probabilidad de enemigo élite: {(_currentWave >= 2 ? "30%" : "0%")}");
		GD.Print($"Multiplicador de dificultad: x{1 + ((_currentWave - 1) * 0.25f):F2}");
		GameManager.Instance.ShowMessage($"¡Comienza la oleada {_currentWave}!");
	}

	private void SpawnEnemy()
	{
		if (_enemyScene == null) return;

		Enemy enemy = _enemyScene.Instantiate<Enemy>();
		if (enemy != null)
		{
			float difficultyMultiplier = 1 + ((_currentWave - 1) * 0.25f); // Incremento del 25% por oleada

			// A partir de la oleada 2, hay probabilidad de enemigo élite
			bool isElite = _currentWave >= 2 && _random.NextDouble() < 0.3;

			if (isElite)
			{
				// Enemigo élite
				enemy.Initialize(
					health: (int)(200 * difficultyMultiplier),
					damage: (int)(20 * difficultyMultiplier),
					speed: 80,
					goldReward: 75,
					isElite: true
				);
				GD.Print("Enemigo élite generado");
			}
			else
			{
				// Enemigo normal
				enemy.Initialize(
					health: (int)(100 * difficultyMultiplier),
					damage: (int)(10 * difficultyMultiplier),
					speed: 100,
					goldReward: 25,
					isElite: false
				);
			}

			_enemiesContainer.AddChild(enemy);
			enemy.Position = new Vector2(0, 300);
			_enemiesSpawned++;
		}
	}
}
