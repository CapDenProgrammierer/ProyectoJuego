using Godot;
using System;

public partial class Enemy : Node2D, IGameUnit
{
	private float _baseSpeed;
	private float _currentSpeed;
	private int _health;
	private int _maxHealth;
	private int _damage;
	private int _goldReward;
	private bool _isElite;
	protected ColorRect _healthBar;
	private float _slowMultiplier = 1f;
	private IEnemyState _currentState;
	private float _endXPosition = 1100f;
	protected Vector2 _startPosition = new Vector2(-50, 300);
	private bool _hasStartedMoving = false;
	protected IMovementStrategy _movementStrategy;

	public float BaseSpeed => _baseSpeed;
	public float CurrentSpeed 
	{ 
		get => _currentSpeed;
		set => _currentSpeed = value;
	}
	public float EndXPosition => _endXPosition;
	public int Damage => _damage;
	public int GoldReward => _goldReward;
	public bool IsAlive => _health > 0;
	public float SlowMultiplier
	{
		get => _slowMultiplier;
		set => _slowMultiplier = value;
	}

	public void Initialize(int health, int damage, float speed, int goldReward, bool isElite, IMovementStrategy movementStrategy = null)
	{
		_health = health;
		_maxHealth = health;
		_damage = damage;
		_baseSpeed = speed;
		_currentSpeed = speed;
		_goldReward = goldReward;
		_isElite = isElite;
		_hasStartedMoving = false;
		_movementStrategy = movementStrategy ?? new StraightLineMovement();
		ChangeState(new MovingState());
	}

	public void TakeDamage(float damage)
	{
		_health -= (int)damage;
		UpdateHealthBar();
		
		if (_health <= 0 && _currentState is not DyingState)
		{
			ChangeState(new DyingState());
		}
	}

	public void ApplySlow(float slowAmount, float duration)
	{
		_slowMultiplier = Mathf.Min(_slowMultiplier, 1f - slowAmount);
		ChangeState(new SlowedState(_currentState, duration));
	}

	public void ChangeState(IEnemyState newState)
	{
		_currentState?.Exit(this);
		_currentState = newState;
		_currentState.Enter(this);
	}

	public override void _Ready()
	{
		AddToGroup("Enemies");
		InitializeVisuals();
		UnitManager.Instance?.RegisterEnemy(this);
		GlobalPosition = _startPosition;
		_hasStartedMoving = true;
	}

	protected virtual void InitializeVisuals()
	{
		var body = new ColorRect();
		body.Size = new Vector2(32, 32);
		body.Position = new Vector2(-16, -16);
		body.Color = _isElite ? Colors.Purple : Colors.Red;
		AddChild(body);

		_healthBar = new ColorRect();
		_healthBar.Size = new Vector2(32, 4);
		_healthBar.Position = new Vector2(-16, -24);
		_healthBar.Color = Colors.Green;
		AddChild(_healthBar);

		if (_isElite)
		{
			var eliteIndicator = new ColorRect();
			eliteIndicator.Size = new Vector2(16, 8);
			eliteIndicator.Position = new Vector2(-8, -28);
			eliteIndicator.Color = Colors.Yellow;
			AddChild(eliteIndicator);
		}
	}

	public override void _ExitTree()
	{
		UnitManager.Instance?.UnregisterEnemy(this);
	}

	private void UpdateHealthBar()
	{
		if (_healthBar != null)
		{
			float healthPercentage = (float)_health / _maxHealth;
			_healthBar.Size = new Vector2(32 * healthPercentage, 4);
			_healthBar.Color = new Color(1 - healthPercentage, healthPercentage, 0);
		}
	}
	
	public new Vector2 GetPosition()
	{
		return GlobalPosition;
	}
	
	public override void _Process(double delta)
	{
		_currentState?.Update(this, delta);
	}

	public void UpdateUnit(double delta)
	{
		if (_movementStrategy != null)
		{
			Vector2 movement = _movementStrategy.GetMovement(this, (float)delta);
			Position += movement;
		}
		_currentState?.Update(this, delta);
	}

	public void ApplyEffect(IEffect effect)
	{
		effect.Apply(this);
	}
}
