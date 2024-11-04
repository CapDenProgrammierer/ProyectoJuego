using Godot;

public partial class RangeIndicator : Node2D
{
	float radius;
	Color rangeColor;

	public RangeIndicator(float range, Color color)
	{
		radius = range;
		rangeColor = color;
	}

	public override void _Draw()
	{
		DrawCircle(Vector2.Zero, radius, rangeColor);
	}
}
