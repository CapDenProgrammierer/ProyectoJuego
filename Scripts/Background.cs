using Godot;
using System;

public partial class Background : Node2D
{
	TextureRect bgImage;
	ColorRect bgColor;
	const string BG_PATH = "res://Assets/Imagenes/background.png";

	public override void _Ready()
	{
		SetupBackground();
	}

	void SetupBackground()
	{
		var screenSize = GetViewport().GetVisibleRect().Size;

		bgColor = new ColorRect
		{
			Position = Vector2.Zero,
			Size = screenSize,
			Color = new Color(0.1f, 0.1f, 0.15f)
		};
		AddChild(bgColor);

		bgImage = new TextureRect
		{
			Position = Vector2.Zero,
			Size = screenSize
		};

		bgImage.StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered;

		var tex = GD.Load<Texture2D>(BG_PATH);
		if (tex != null)
		{
			bgImage.Texture = tex;
			AddChild(bgImage);
		}
	}

	public override void _Process(double delta)
	{
		var screenSize = GetViewport().GetVisibleRect().Size;
		
		if (bgColor != null && bgColor.Size != screenSize)
			bgColor.Size = screenSize;

		if (bgImage != null && bgImage.Size != screenSize)
			bgImage.Size = screenSize;
	}
}
