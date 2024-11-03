using Godot;
using System;

public partial class GameOverScreen : Control
{
	private Label _gameOverLabel;
	private Label _waveLabel;
	private Label _scoreLabel;
	private Panel _backgroundPanel;

	public override void _Ready()
	{

		 SetAnchorsPreset(LayoutPreset.FullRect);
		CreateBackground();
		CreateLayout();
		Hide();
		
		
		ProcessMode = ProcessModeEnum.Always;
	}

	private void CreateBackground()
	{
		 _backgroundPanel = new Panel();
		_backgroundPanel.SetAnchorsPreset(LayoutPreset.FullRect);
		_backgroundPanel.Modulate = new Color(0, 0, 0, 0.9f);
		AddChild(_backgroundPanel);
	}

	private void CreateLayout()
	{
		var container = new VBoxContainer();
		container.SetAnchorsPreset(LayoutPreset.Center);
		container.GrowHorizontal = GrowDirection.Both;
		container.GrowVertical = GrowDirection.Both;
		container.Size = new Vector2(600, 400);
		container.Position = new Vector2(-300, -200); 

		_gameOverLabel = new Label
		{
			Text = "¡GAME OVER!",
			HorizontalAlignment = HorizontalAlignment.Center
		};
		_gameOverLabel.AddThemeFontSizeOverride("font_size", 64);
		_gameOverLabel.AddThemeColorOverride("font_color", Colors.Red);
		container.AddChild(_gameOverLabel);


		container.AddChild(new Control { CustomMinimumSize = new Vector2(0, 50) });

		_waveLabel = new Label
		{
			HorizontalAlignment = HorizontalAlignment.Center
		};
		_waveLabel.AddThemeFontSizeOverride("font_size", 32);
		_waveLabel.AddThemeColorOverride("font_color", Colors.White);
		container.AddChild(_waveLabel);

		container.AddChild(new Control { CustomMinimumSize = new Vector2(0, 30) });

		_scoreLabel = new Label
		{
			HorizontalAlignment = HorizontalAlignment.Center
		};
		_scoreLabel.AddThemeFontSizeOverride("font_size", 32);
		_scoreLabel.AddThemeColorOverride("font_color", Colors.Yellow);
		container.AddChild(_scoreLabel);

		container.AddChild(new Control { CustomMinimumSize = new Vector2(0, 50) });

		var restartLabel = new Label
		{
			Text = "Presiona cualquier tecla para reiniciar",
			HorizontalAlignment = HorizontalAlignment.Center
		};
		restartLabel.AddThemeFontSizeOverride("font_size", 24);
		restartLabel.AddThemeColorOverride("font_color", Colors.White);
		container.AddChild(restartLabel);

		AddChild(container);
	}

	public new void Show(int waveNumber, int gold)
	{
		GD.Print("Mostrando pantalla de Game Over");
		
		_waveLabel.Text = $"Llegaste hasta la oleada {waveNumber}";
		_scoreLabel.Text = $"Oro acumulado: {gold}";

		Visible = true;
		ZIndex = 1000;
	
		Modulate = new Color(1, 1, 1, 0);
		var tween = CreateTween();
		tween.TweenProperty(this, "modulate:a", 1.0f, 0.5f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);
	}
}
