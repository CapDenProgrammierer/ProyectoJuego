using Godot;
using System;

public partial class Background : Node2D
{
	private TextureRect _backgroundTexture;
	private ColorRect _backgroundColor;
	private const string BACKGROUND_PATH = "res://Assets/Imagenes/background.png";

	public override void _Ready()
	{
		CreateBackground();
	}

	private void CreateBackground()
	{
		var viewportSize = GetViewport().GetVisibleRect().Size;

		_backgroundColor = new ColorRect
		{
			Position = Vector2.Zero,
			Size = viewportSize,
			Color = new Color(0.1f, 0.1f, 0.15f)
		};
		AddChild(_backgroundColor);

		_backgroundTexture = new TextureRect
		{
			Position = Vector2.Zero,
			Size = viewportSize
		};

		_backgroundTexture.StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered;

		var texture = GD.Load<Texture2D>(BACKGROUND_PATH);
		if (texture != null)
		{
			_backgroundTexture.Texture = texture;
			AddChild(_backgroundTexture);
		}
	}

	public override void _Process(double delta)
	{
		var viewportSize = GetViewport().GetVisibleRect().Size;
		
		if (_backgroundColor != null && _backgroundColor.Size != viewportSize)
		{
			_backgroundColor.Size = viewportSize;
		}

		if (_backgroundTexture != null && _backgroundTexture.Size != viewportSize)
		{
			_backgroundTexture.Size = viewportSize;
		}
	}
}
