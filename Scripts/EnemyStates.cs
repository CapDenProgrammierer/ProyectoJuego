using Godot;
using System;

public class MovingState : IEnemyState
{
	public void Enter(Enemy enemy)
	{
		enemy.CurrentSpeed = enemy.BaseSpeed * enemy.SlowMultiplier;
	}
	
	public void Exit(Enemy enemy) { }
	
	public void Update(Enemy enemy, double delta)
	{
		if (!enemy.IsAlive) 
		{
			enemy.ChangeState(new DyingState());
			return;
		}
		
		if (enemy.Position.X >= enemy.EndXPosition)
		{
			enemy.ChangeState(new ReachedEndState());
		}
	}
}

public class DyingState : IEnemyState
{
	private float _fadeTime = 1.0f;
	private float _currentTime = 0;
	
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
		
		tween.TweenProperty(enemy, "rotation", Mathf.Pi * 2, _fadeTime)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.In);
			
		tween.TweenProperty(enemy, "scale", Vector2.Zero, _fadeTime)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.In);

		tween.TweenProperty(enemy, "modulate:a", 0.0f, _fadeTime)
			.SetTrans(Tween.TransitionType.Cubic);
	}
	
	public void Exit(Enemy enemy) { }
	
	public void Update(Enemy enemy, double delta)
	{
		_currentTime += (float)delta;
		if (_currentTime >= _fadeTime)
		{
			enemy.QueueFree();
		}
	}
}

public class ReachedEndState : IEnemyState
{
	private float _animationTime = 0.5f;
	private bool _hasStartedAnimation = false;
	private bool _hasDamaged = false;

	public void Enter(Enemy enemy)
	{
		enemy.CurrentSpeed = 0;
		_hasStartedAnimation = false;
		_hasDamaged = false;
		StartDamageAnimation(enemy);
	}
	
	public void Exit(Enemy enemy) { }
	
	private void StartDamageAnimation(Enemy enemy)
	{
		if (_hasStartedAnimation) return;
		_hasStartedAnimation = true;

		VisualEffectSystem.Instance?.CreateBaseHitEffect(new Vector2(1100, 300));
		
		var tween = enemy.CreateTween();
		tween.SetParallel(true);

		tween.TweenProperty(enemy, "scale", enemy.Scale * 1.5f, _animationTime * 0.2f)
			.SetTrans(Tween.TransitionType.Back);
			
		tween.TweenProperty(enemy, "position", 
			enemy.Position - new Vector2(20, 0), 
			_animationTime * 0.2f)
			.SetTrans(Tween.TransitionType.Back);

		tween.Chain();
		tween.TweenProperty(enemy, "position", 
			enemy.Position + new Vector2(50, 0), 
			_animationTime * 0.3f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.In);

		tween.TweenCallback(Callable.From(() => 
		{
			if (!_hasDamaged && GameManager.Instance != null && !GameManager.Instance.IsGameOver)
			{
				_hasDamaged = true;
				GameManager.Instance.TakeDamage(enemy.Damage);
			}
		}));

		tween.TweenProperty(enemy, "scale", Vector2.Zero, _animationTime * 0.3f);
		tween.TweenProperty(enemy, "modulate:a", 0.0f, _animationTime * 0.3f);
		tween.TweenCallback(Callable.From(() => enemy.QueueFree()));
	}
	
	public void Update(Enemy enemy, double delta) { }
}

public class SlowedState : IEnemyState
{
	private IEnemyState _previousState;
	private double _slowTimeRemaining;
	
	public SlowedState(IEnemyState previousState, float duration)
	{
		_previousState = previousState;
		_slowTimeRemaining = duration;
	}
	
	public void Enter(Enemy enemy)
	{
		enemy.CurrentSpeed = enemy.BaseSpeed * enemy.SlowMultiplier;

		var slowEffect = new ColorRect
		{
			Size = new Vector2(32, 32),
			Position = new Vector2(-16, -16),
			Color = new Color(0, 0, 1, 0.2f),
			ZIndex = -1
		};
		enemy.AddChild(slowEffect);
		
		var tween = enemy.CreateTween();
		tween.TweenProperty(slowEffect, "modulate:a", 0.0f, (float)_slowTimeRemaining)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.InOut);
		
		tween.TweenCallback(Callable.From(() => slowEffect.QueueFree()));
	}
	
	public void Exit(Enemy enemy)
	{
		enemy.SlowMultiplier = 1.0f;
		enemy.CurrentSpeed = enemy.BaseSpeed;
	}
	
	public void Update(Enemy enemy, double delta)
	{
		_slowTimeRemaining -= delta;
		
		if (_slowTimeRemaining <= 0)
		{
			enemy.ChangeState(_previousState);
			return;
		}
		
		_previousState.Update(enemy, delta);
	}
}
