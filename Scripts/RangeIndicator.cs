using Godot;

public partial class RangeIndicator : Node2D
{
	private float _range;
	private Color _color;

	public RangeIndicator(float range, Color color)
	{
		_range = range;
		_color = color;
	}

	public override void _Draw()
	{
		DrawCircle(Vector2.Zero, _range, _color);
	}
}
