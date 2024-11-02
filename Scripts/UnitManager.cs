using Godot;
using System.Collections.Generic;

public partial class UnitManager : Node
{
	private List<Enemy> _enemies = new List<Enemy>();
	private List<Tower> _towers = new List<Tower>();
	private static UnitManager _instance;

	public static UnitManager Instance
	{
		get { return _instance; }
	}

	public override void _EnterTree()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	public void RegisterEnemy(Enemy enemy)
	{
		if (!_enemies.Contains(enemy))
		{
			_enemies.Add(enemy);
		}
	}

	public void UnregisterEnemy(Enemy enemy)
	{
		_enemies.Remove(enemy);
	}

	public void RegisterTower(Tower tower)
	{
		if (!_towers.Contains(tower))
		{
			_towers.Add(tower);
		}
	}

	public void UnregisterTower(Tower tower)
	{
		_towers.Remove(tower);
	}

	public List<Enemy> GetEnemiesInRange(Vector2 position, float range)
	{
		return _enemies.FindAll(enemy => 
			enemy.GlobalPosition.DistanceTo(position) <= range);
	}

	public List<Tower> GetTowersInRange(Vector2 position, float range)
	{
		return _towers.FindAll(tower => 
			tower.GlobalPosition.DistanceTo(position) <= range);
	}

	public void ApplyEffectToArea(Vector2 position, float range, IEffect effect)
	{
		var affectedEnemies = GetEnemiesInRange(position, range);
		foreach (var enemy in affectedEnemies)
		{
			effect.Apply(enemy);
		}
	}
}
