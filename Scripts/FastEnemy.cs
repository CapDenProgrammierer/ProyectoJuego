using Godot;
using System;

public partial class FastEnemy : Enemy
{
	protected override void InitializeVisuals()
	{
		var body = new ColorRect();
		body.Size = new Vector2(24, 24);
		body.Position = new Vector2(-12, -12);
		body.Color = new Color(1, 0.5f, 0, 1);
		AddChild(body);

		_healthBar = new ColorRect();
		_healthBar.Size = new Vector2(24, 4);
		_healthBar.Position = new Vector2(-12, -18);
		_healthBar.Color = Colors.Green;
		AddChild(_healthBar);
	}
}
