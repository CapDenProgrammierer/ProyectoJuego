using Godot;

public interface IGameUnit
{
	void UpdateUnit(double delta);
	void TakeDamage(float damage);
	Vector2 GetPosition();
	void ApplyEffect(IEffect effect);
}

public interface IEffect
{
	void Apply(IGameUnit unit);
}

public class SlowEffect : IEffect
{
	private float _slowAmount;
	private float _duration;

	public SlowEffect(float slowAmount, float duration)
	{
		_slowAmount = slowAmount;
		_duration = duration;
	}

	public void Apply(IGameUnit unit)
	{
		if (unit is Enemy enemy)
		{
			enemy.ApplySlow(_slowAmount, _duration);
		}
	}
}

public class GameUnitComposite : IGameUnit
{
	private System.Collections.Generic.List<IGameUnit> _units = new System.Collections.Generic.List<IGameUnit>();
	private Vector2 _position;

	public void Add(IGameUnit unit)
	{
		_units.Add(unit);
	}

	public void Remove(IGameUnit unit)
	{
		_units.Remove(unit);
	}

	public void Clear()
	{
		_units.Clear();
	}

	public void UpdateUnit(double delta)
	{
		foreach (var unit in _units.ToArray())
		{
			unit.UpdateUnit(delta);
		}
	}

	public void TakeDamage(float damage)
	{
		foreach (var unit in _units.ToArray())
		{
			unit.TakeDamage(damage);
		}
	}

	public Vector2 GetPosition()
	{
		if (_units.Count == 0)
			return _position;

		Vector2 centerPosition = Vector2.Zero;
		foreach (var unit in _units)
		{
			centerPosition += unit.GetPosition();
		}
		return centerPosition / _units.Count;
	}

	public void ApplyEffect(IEffect effect)
	{
		foreach (var unit in _units.ToArray())
		{
			unit.ApplyEffect(effect);
		}
	}

	public System.Collections.Generic.List<IGameUnit> GetUnitsInRange(Vector2 position, float range)
	{
		return _units.FindAll(unit => 
			unit.GetPosition().DistanceTo(position) <= range);
	}
}
