using Godot;
using System;

public partial class Enemy : Node2D
{
	private float _speed;
	private int _health;
	private int _maxHealth;
	private int _damage;
	private int _goldReward;
	private bool _isElite;
	private ColorRect _healthBar;
	
	public bool IsAlive => _health > 0;

	public void Initialize(int health, int damage, float speed, int goldReward, bool isElite)
	{
		_health = health;
		_maxHealth = health;
		_damage = damage;
		_speed = speed;
		_goldReward = goldReward;
		_isElite = isElite;
	}

	public void TakeDamage(float damage)
	{
		_health -= (int)damage;
		UpdateHealthBar();
		
		if (_health <= 0)
		{
			if (GameManager.Instance != null)
			{
				GameManager.Instance.AddGold(_goldReward);
			}
			QueueFree();
		}
	}

	public override void _Ready()
	{
		AddToGroup("Enemies");
		
		// Cuerpo del enemigo
		var body = new ColorRect();
		body.Size = new Vector2(32, 32);
		body.Position = new Vector2(-16, -16);
		body.Color = _isElite ? Colors.Purple : Colors.Red;
		AddChild(body);

		// Barra de vida
		_healthBar = new ColorRect();
		_healthBar.Size = new Vector2(32, 4);
		_healthBar.Position = new Vector2(-16, -24);
		_healthBar.Color = Colors.Green;
		AddChild(_healthBar);

		if (_isElite)
		{
			// Corona o indicador de élite
			var eliteIndicator = new ColorRect();
			eliteIndicator.Size = new Vector2(16, 8);
			eliteIndicator.Position = new Vector2(-8, -28);
			eliteIndicator.Color = Colors.Yellow;
			AddChild(eliteIndicator);
		}
	}

	private void UpdateHealthBar()
	{
		if (_healthBar != null)
		{
			float healthPercentage = (float)_health / _maxHealth;
			_healthBar.Size = new Vector2(32 * healthPercentage, 4);
			_healthBar.Color = new Color(
				1 - healthPercentage, // Más rojo cuando menos vida
				healthPercentage,     // Más verde cuando más vida
				0
			);
		}
	}

	public override void _Process(double delta)
	{
		Position += Vector2.Right * _speed * (float)delta;
	}
}
