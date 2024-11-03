using Godot;
using System;

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
	private bool _isFast;
	protected ColorRect _healthBar;
	private float _slowMultiplier = 1f;
	private IEnemyState _currentState;
	private float _endXPosition = 1100f;
	protected Vector2 _startPosition = new Vector2(-50, 300);
	private bool _hasStartedMoving = false;
	protected IMovementStrategy _movementStrategy;
	protected Sprite2D _sprite;
	private bool _isDying = false;
	
	public bool IsDying => _isDying;
	public float BaseSpeed => _baseSpeed;
		public float CurrentSpeed 
	{ 
		get => _currentSpeed;
		set => _currentSpeed = value;
	}
	public float EndXPosition => _endXPosition;
		public int Damage => _damage;
	public int GoldReward => _goldReward;
	public bool IsAlive => _health > 0 && !_isDying;
	public float SlowMultiplier
	{
		get => _slowMultiplier;
			set => _slowMultiplier = value;
	}

	public void Initialize(int health, int damage, float speed, int goldReward, bool isElite, bool isFast = false, IMovementStrategy movementStrategy = null)
	{
		_health = health;
		_maxHealth = health;
			_damage = damage;
		_baseSpeed = speed;
		_currentSpeed = speed;
		_goldReward = goldReward;
		_isElite = isElite;
		_isFast = isFast;
		_hasStartedMoving = false;
		_isDying = false;
			_movementStrategy = movementStrategy ?? new StraightLineMovement();
		ChangeState(new MovingState());
	}

	public void TakeDamage(float damage)
	{
		if (_isDying) return;
		
		_health -= (int)damage;
			UpdateHealthBar();
		
		if (_health <= 0 && _currentState is not DyingState)
		{
			_isDying = true;
				RemoveFromGroup("Enemies");
			ChangeState(new DyingState());
		}
	}

	public void ApplySlow(float slowAmount, float duration)
	{
		if (_isDying) return;
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
		_sprite = new Sprite2D();
		_sprite.ZIndex = 0;
		AddChild(_sprite);


			_healthBar = new ColorRect();
		_healthBar.Size = new Vector2(_isElite || !_isFast ? 32 : 24, 4);
		_healthBar.Position = new Vector2(_isElite || !_isFast ? -16 : -12, _isElite || !_isFast ? -24 : -20);
			_healthBar.Color = Colors.Green;
		_healthBar.ZIndex = 1;
		AddChild(_healthBar);

		if (_isElite)
		{
			var eliteIndicator = new ColorRect();
			eliteIndicator.Size = new Vector2(16, 8);
				eliteIndicator.Position = new Vector2(-8, -28);
			eliteIndicator.Color = Colors.Yellow;
			eliteIndicator.ZIndex = 2;
			AddChild(eliteIndicator);
		}

		if (_isElite)
		{
			CreateEliteSprite();
		}
		else if (_isFast)
		{
			CreateFastSprite();
		}
		else
		{
			CreateNormalSprite();
		}
	}

	private void CreateNormalSprite()
	{
			var texture = GD.Load<Texture2D>("res://Assets/Imagenes/Enemigo1.png");
		if (texture != null)
		{
			_sprite.Texture = texture;
			float scale = 32.0f / texture.GetWidth();
				_sprite.Scale = new Vector2(scale, scale);
		}
		else
		{
			GD.PrintErr("No se pudo cargar la imagen del enemigo normal");
		}
	}

	private void CreateFastSprite()
	{
			var texture = GD.Load<Texture2D>("res://Assets/Imagenes/Enemigo2.png");
		if (texture != null)
		{
			_sprite.Texture = texture;
			float scale = 24.0f / texture.GetWidth();
			 	_sprite.Scale = new Vector2(scale, scale);
		}
		else
		{
			GD.PrintErr("No se pudo cargar la imagen del enemigo rápido");

		}
	}

	private void CreateEliteSprite()
	{
		  var texture = GD.Load<Texture2D>("res://Assets/Imagenes/Enemigo3.png");
			if (texture != null)
		{
			_sprite.Texture = texture;
				float scale = 32.0f / texture.GetWidth();
			_sprite.Scale = new Vector2(scale, scale);
		}
		else
		{
			GD.PrintErr("No se pudo cargar la imagen del enemigo elite");
	
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
			float baseWidth = _isElite || !_isFast ? 32 : 24;
			_healthBar.Size = new Vector2(baseWidth * healthPercentage, 4);
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
		 if (_movementStrategy != null && !_isDying)
		{
			Vector2 movement = _movementStrategy.GetMovement(this, (float)delta);
			Position += movement;
		}
		 _currentState?.Update(this, delta);
	}

	public void ApplyEffect(IEffect effect)
	{
		 if (!_isDying)
		{
			effect.Apply(this);
		}
	}
}
