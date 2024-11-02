using Godot;

public interface IEnemyState
{
	void Enter(Enemy enemy);
	void Exit(Enemy enemy);
	void Update(Enemy enemy, double delta);
}
