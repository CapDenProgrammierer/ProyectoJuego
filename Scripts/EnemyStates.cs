using Godot;

public class MovingState : IEnemyState
{
	private Vector2 _targetPosition;
	private float _pathProgress = 0;
	
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

		Vector2 movement = new Vector2(enemy.CurrentSpeed * (float)delta, 0);
		enemy.Position += movement;
		
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
	}
	
	public void Exit(Enemy enemy) { }
	
	public void Update(Enemy enemy, double delta)
	{
		_currentTime += (float)delta;
		enemy.Modulate = new Color(1, 1, 1, 1 - (_currentTime / _fadeTime));
		
		if (_currentTime >= _fadeTime)
		{
			enemy.QueueFree();
		}
	}
}

public class ReachedEndState : IEnemyState
{
	private float _damageDelay = 0.5f; 
	private float _currentDelay = 0f;
	private bool _hasDamaged = false;

	public void Enter(Enemy enemy)
	{
		enemy.CurrentSpeed = 0;
		_hasDamaged = false;
	}
	
	public void Exit(Enemy enemy) { }
	
	public void Update(Enemy enemy, double delta)
	{
		if (!_hasDamaged)
		{
			_currentDelay += (float)delta;
			
			if (_currentDelay >= _damageDelay)
			{
				GameEventSystem.Instance?.NotifyObservers(new MessageEvent(
					$"¡Un enemigo alcanzó la base! -{enemy.Damage} vidas"
				));

				if (GameManager.Instance != null)
				{
					GameManager.Instance.TakeDamage(enemy.Damage);
				}
				
				_hasDamaged = true;
				enemy.QueueFree();
			}
		}
	}
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
