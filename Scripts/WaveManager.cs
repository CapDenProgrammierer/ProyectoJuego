using Godot;
using System;

public partial class WaveManager : Node
{
	int currentWave = 0;
	int enemiesInWave = 0;
	int enemiesSpawned = 0;
	float spawnTimer = 0;
	float spawnDelay = 1.0f;
	float waveCooldown = 5.0f;
	float waveTimer = 0;
	bool waveActive = false;
	bool waitingForNext = false;
	Node2D enemyCont;
	PackedScene enemyScene;
	Random rng = new Random();
	float startDelay = 3.0f;
	float startTimer = 0f;
	bool started = false;
	
	public int CurrentWave => currentWave;

	public override void _Ready()
	{
		enemyScene = GD.Load<PackedScene>("res://Scenes/Enemy.tscn");
		enemyCont = GetNode<Node2D>("/root/Main/EnemiesContainer");
		started = false;
		startTimer = 0f;
		GameEventSystem.Instance?.NotifyObservers(new MessageEvent("¡Prepárate! La primera oleada comenzará en 3 segundos"));
	}

	public override void _Process(double delta)
	{
		if (!started)
		{
			startTimer += (float)delta;
			if (startTimer >= startDelay)
			{
				started = true;
				StartNextWave();
			}
			return;
		}

		if (waitingForNext)
		{
			waveTimer += (float)delta;
			if (waveTimer >= waveCooldown)
			{
				waitingForNext = false;
				GiveWaveCompletion();
				StartNextWave();
			}
			return;
		}

		if (waveActive)
		{
			if (enemiesSpawned < enemiesInWave)
			{
				spawnTimer += (float)delta;
				if (spawnTimer >= spawnDelay)
				{
					SpawnEnemy();
					spawnTimer = 0;
				}
			}
			else if (GetTree().GetNodesInGroup("Enemies").Count == 0)
			{
				waveActive = false;
				waitingForNext = true;
				waveTimer = 0;
				GameEventSystem.Instance?.NotifyObservers(new WaveEvent(currentWave, WaveState.Complete, enemiesInWave));
			}
		}
	}

	void GiveWaveCompletion()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.AddLives(20);
			GameEventSystem.Instance?.NotifyObservers(new ResourceEvent(ResourceType.Lives, GameManager.Instance.Lives - 20, GameManager.Instance.Lives));
			GameEventSystem.Instance?.NotifyObservers(new MessageEvent($"¡Oleada completada! +20 vidas"));
		}
	}

	void StartNextWave()
	{
		currentWave++;
		enemiesInWave = 5 + currentWave;
		enemiesSpawned = 0;
		waveActive = true;
		waveTimer = 0;
		spawnDelay = Mathf.Max(0.5f, 1.0f - (currentWave * 0.05f));
		
		GameEventSystem.Instance?.NotifyObservers(new WaveEvent(currentWave, WaveState.Starting, enemiesInWave));
		GameEventSystem.Instance?.NotifyObservers(new MessageEvent($"¡Oleada {currentWave} comenzando! Enemigos: {enemiesInWave}"));
	}

	void SpawnEnemy()
	{
		float diffMult = 1 + ((currentWave - 1) * 0.25f);
		
		float eliteChance = Mathf.Min(0.3f + (currentWave * 0.02f), 0.5f);
		float fastChance = Mathf.Min(0.4f + (currentWave * 0.03f), 0.6f);
		float zigzagChance = Mathf.Min(0.2f + (currentWave * 0.05f), 0.4f);

		bool isElite = currentWave >= 2 && rng.NextDouble() < eliteChance;
		bool isFast = !isElite && rng.NextDouble() < fastChance;
		bool useZigzag = isElite ? rng.NextDouble() < 0.8f : rng.NextDouble() < zigzagChance;

		IMovementStrategy moveStrat;
		if (useZigzag)
		{
			var zigzag = new ZigZagMovement();
			if (isElite)
			{
				zigzag.Amplitude = 200f;
				zigzag.Frequency = 0.8f;
			}
			else if (isFast)
			{
				zigzag.Amplitude = 100f;
				zigzag.Frequency = 1.5f;
			}
			else
			{
				zigzag.Amplitude = 150f;
				zigzag.Frequency = 1.0f;
			}
			moveStrat = zigzag;
		}
		else
		{
			moveStrat = new StraightLineMovement();
		}

		Enemy enemy = enemyScene.Instantiate<Enemy>();
		
		if (isElite)
		{
			enemy.Initialize(
				health: (int)(200 * diffMult),
				damage: (int)(20 * diffMult),
				speed: 60,
				goldReward: 75,
				eliteFlag: true,
				fastFlag: false,
				movementStrategy: moveStrat
			);
		}
		else if (isFast)
		{
			enemy.Initialize(
				health: (int)(80 * diffMult),
				damage: (int)(15 * diffMult),
				speed: 120,
				goldReward: 50,
				eliteFlag: false,
				fastFlag: true,
				movementStrategy: moveStrat
			);
		}
		else
		{
			enemy.Initialize(
				health: (int)(100 * diffMult),
				damage: (int)(10 * diffMult),
				speed: 80,
				goldReward: 25,
				eliteFlag: false,
				fastFlag: false,
				movementStrategy: moveStrat
			);
		}

		enemyCont.AddChild(enemy);
		enemiesSpawned++;
	}
}
