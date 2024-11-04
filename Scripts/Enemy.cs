using Godot;
using System;

public partial class Enemy : Node2D, IGameUnit
{
	float baseSpeed;
	float speed;
	int hp;
	int maxHp;
	int dmg;
	int gold;
	bool isElite;
	bool isFast;
	ColorRect hpBar;
	float slowMult = 1f;
	IEnemyState state;
	float endX = 1100f;
	Vector2 startPos = new Vector2(-50, 300);
	bool started = false;
	IMovementStrategy moveStrat;
	Sprite2D sprite;
	bool isDead = false;
	
	public bool IsDying => isDead;
	public float BaseSpeed => baseSpeed;
	public float CurrentSpeed 
	{ 
		get => speed;
		set => speed = value;
	}
	public float EndXPosition => endX;
	public int Damage => dmg;
	public int GoldReward => gold;
	public bool IsAlive => hp > 0 && !isDead;
	public float SlowMultiplier
	{
		get => slowMult;
		set => slowMult = value;
	}

	public void Initialize(int health, int damage, float speed, int goldReward, bool eliteFlag, bool fastFlag = false, IMovementStrategy movementStrategy = null)
	{
		hp = health;
		maxHp = health;
		dmg = damage;
		baseSpeed = speed;
		this.speed = speed;
		gold = goldReward;
		isElite = eliteFlag;
		isFast = fastFlag;
		started = false;
		isDead = false;
		moveStrat = movementStrategy ?? new StraightLineMovement();
		ChangeState(new MovingState());
	}

	public void TakeDamage(float damage)
	{
		if (isDead) return;
		
		hp -= (int)damage;
		UpdateHPBar();
		
		if (hp <= 0 && state is not DyingState)
		{
			isDead = true;
			RemoveFromGroup("Enemies");
			ChangeState(new DyingState());
		}
	}

	public void ApplySlow(float amount, float duration)
	{
		if (isDead) return;
		slowMult = Mathf.Min(slowMult, 1f - amount);
		ChangeState(new SlowedState(state, duration));
	}

	public void ChangeState(IEnemyState newState)
	{
		state?.Exit(this);
		state = newState;
		state.Enter(this);
	}

	public override void _Ready()
	{
		AddToGroup("Enemies");
		InitSprite();
		UnitManager.Instance?.RegisterEnemy(this);
		GlobalPosition = startPos;
		started = true;
	}

	void InitSprite()
	{
		sprite = new Sprite2D();
		sprite.ZIndex = 0;
		AddChild(sprite);

		hpBar = new ColorRect();
		hpBar.Size = new Vector2(isElite || !isFast ? 32 : 24, 4);
		hpBar.Position = new Vector2(isElite || !isFast ? -16 : -12, isElite || !isFast ? -24 : -20);
		hpBar.Color = Colors.Green;
		hpBar.ZIndex = 1;
		AddChild(hpBar);

		if (isElite)
		{
			var eliteMark = new ColorRect();
			eliteMark.Size = new Vector2(16, 8);
			eliteMark.Position = new Vector2(-8, -28);
			eliteMark.Color = Colors.Yellow;
			eliteMark.ZIndex = 2;
			AddChild(eliteMark);
		}

		if (isElite)
			MakeEliteSprite();
		else if (isFast)
			MakeFastSprite();
		else
			MakeNormalSprite();
	}

	void MakeNormalSprite()
	{
		var tex = GD.Load<Texture2D>("res://Assets/Imagenes/Enemigo1.png");
		if (tex != null)
		{
			sprite.Texture = tex;
			float scl = 32.0f / tex.GetWidth();
			sprite.Scale = new Vector2(scl, scl);
		}
	}

	void MakeFastSprite()
	{
		var tex = GD.Load<Texture2D>("res://Assets/Imagenes/Enemigo2.png");
		if (tex != null)
		{
			sprite.Texture = tex;
			float scl = 24.0f / tex.GetWidth();
			sprite.Scale = new Vector2(scl, scl);
		}
	}

	void MakeEliteSprite()
	{
		var tex = GD.Load<Texture2D>("res://Assets/Imagenes/Enemigo3.png");
		if (tex != null)
		{
			sprite.Texture = tex;
			float scl = 32.0f / tex.GetWidth();
			sprite.Scale = new Vector2(scl, scl);
		}
	}

	public override void _ExitTree()
	{
		UnitManager.Instance?.UnregisterEnemy(this);
	}

	void UpdateHPBar()
	{
		if (hpBar != null)
		{
			float hpPercent = (float)hp / maxHp;
			float w = isElite || !isFast ? 32 : 24;
			hpBar.Size = new Vector2(w * hpPercent, 4);
			hpBar.Color = new Color(1 - hpPercent, hpPercent, 0);
		}
	}
	
	public Vector2 GetPosition()
	{
		return GlobalPosition;
	}
	
	public override void _Process(double delta)
	{
		state?.Update(this, delta);
	}
	
	public void ApplyEffect(IEffect effect)
	{
		if (!isDead)
			effect.Apply(this);
	}

	public void UpdateUnit(double delta)
	{
		if (moveStrat != null && !isDead)
		{
			Vector2 movement = moveStrat.GetMovement(this, (float)delta);
			Position += movement;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateUnit(delta);
	}
}
