using Godot;
using System;

public class MovingState : IEnemyState
{
	public void Enter(Enemy enemy)
	{
		enemy.CurrentSpeed = enemy.BaseSpeed * enemy.SlowMultiplier;
	}
	
	public void Exit(Enemy enemy) { }
	
	public void Update(Enemy enemy, double dt)
	{
		if (!enemy.IsAlive) 
		{
			enemy.ChangeState(new DyingState());
			return;
		}
		
		if (enemy.Position.X >= enemy.EndXPosition)
			enemy.ChangeState(new ReachedEndState());
	}
}

public class DyingState : IEnemyState
{
	float fadeTime = 1.0f;
	float timer = 0;
	
	public void Enter(Enemy enemy)
	{
		enemy.CurrentSpeed = 0;
		
		GameEventSystem.Instance?.NotifyObservers(new EnemyEvent(
			EnemyEventType.Killed,
			enemy.GlobalPosition,
			enemy.GoldReward
		));

		GameEventSystem.Instance?.NotifyObservers(new MessageEvent(
			$"¡Enemigo eliminado! +{enemy.GoldReward} oro"
		));

		var tween = enemy.CreateTween();
		tween.SetParallel(true);
		
		tween.TweenProperty(enemy, "rotation", Mathf.Pi * 2, fadeTime)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.In);
			
		tween.TweenProperty(enemy, "scale", Vector2.Zero, fadeTime)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.In);

		tween.TweenProperty(enemy, "modulate:a", 0.0f, fadeTime)
			.SetTrans(Tween.TransitionType.Cubic);
	}
	
	public void Exit(Enemy enemy) { }
	
	public void Update(Enemy enemy, double dt)
	{
		timer += (float)dt;
		if (timer >= fadeTime)
			enemy.QueueFree();
	}
}

public class ReachedEndState : IEnemyState
{
	float animTime = 0.5f;
	bool animStarted = false;
	bool dmgDealt = false;

	public void Enter(Enemy enemy)
	{
		enemy.CurrentSpeed = 0;
		animStarted = false;
		dmgDealt = false;
		StartHitAnimation(enemy);
	}
	
	public void Exit(Enemy enemy) { }
	
	void StartHitAnimation(Enemy enemy)
	{
		if (animStarted) return;
		animStarted = true;

		VisualEffectSystem.Instance?.CreateBaseHitEffect(new Vector2(1100, 300));
		
		var tween = enemy.CreateTween();
		tween.SetParallel(true);

		tween.TweenProperty(enemy, "scale", enemy.Scale * 1.5f, animTime * 0.2f)
			.SetTrans(Tween.TransitionType.Back);
			
		tween.TweenProperty(enemy, "position", 
			enemy.Position - new Vector2(20, 0), 
			animTime * 0.2f)
			.SetTrans(Tween.TransitionType.Back);

		tween.Chain();
		tween.TweenProperty(enemy, "position", 
			enemy.Position + new Vector2(50, 0), 
			animTime * 0.3f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.In);

		tween.TweenCallback(Callable.From(() => 
		{
			if (!dmgDealt && GameManager.Instance != null && !GameManager.Instance.IsGameOver)
			{
				dmgDealt = true;
				GameManager.Instance.TakeDamage(enemy.Damage);
			}
		}));

		tween.TweenProperty(enemy, "scale", Vector2.Zero, animTime * 0.3f);
		tween.TweenProperty(enemy, "modulate:a", 0.0f, animTime * 0.3f);
		tween.TweenCallback(Callable.From(() => enemy.QueueFree()));
	}
	
	public void Update(Enemy enemy, double dt) { }
}

public class SlowedState : IEnemyState
{
	IEnemyState prevState;
	double slowTimer;
	
	public SlowedState(IEnemyState prevState, float duration)
	{
		this.prevState = prevState;
		slowTimer = duration;
	}
	
	public void Enter(Enemy enemy)
	{
		enemy.CurrentSpeed = enemy.BaseSpeed * enemy.SlowMultiplier;

		var slowVFX = new ColorRect
		{
			Size = new Vector2(32, 32),
			Position = new Vector2(-16, -16),
			Color = new Color(0, 0, 1, 0.2f),
			ZIndex = -1
		};
		enemy.AddChild(slowVFX);
		
		var tween = enemy.CreateTween();
		tween.TweenProperty(slowVFX, "modulate:a", 0.0f, (float)slowTimer)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.InOut);
		
		tween.TweenCallback(Callable.From(() => slowVFX.QueueFree()));
	}
	
	public void Exit(Enemy enemy)
	{
		enemy.SlowMultiplier = 1.0f;
		enemy.CurrentSpeed = enemy.BaseSpeed;
	}
	
	public void Update(Enemy enemy, double dt)
	{
		slowTimer -= dt;
		
		if (slowTimer <= 0)
		{
			enemy.ChangeState(prevState);
			return;
		}
		
		prevState.Update(enemy, dt);
	}
}
