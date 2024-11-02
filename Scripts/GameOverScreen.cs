using Godot;
using System;

public partial class GameOverScreen : Control
{
	private Label _gameOverLabel;
	private Label _scoreLabel;
	private Label _waveLabel;
	private Panel _backgroundPanel;

	public override void _Ready()
	{
		CreateBackground();
		CreateLayout();
		Hide(); // Inicialmente oculta
	}

	private void CreateBackground()
	{
		_backgroundPanel = new Panel();
		_backgroundPanel.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		_backgroundPanel.Modulate = new Color(0, 0, 0, 0.8f); // Panel semitransparente
		AddChild(_backgroundPanel);
	}

	private void CreateLayout()
	{
		// Contenedor principal centrado
		var container = new VBoxContainer();
		container.SetAnchorsPreset(Control.LayoutPreset.Center);
		container.GrowHorizontal = Control.GrowDirection.Both;
		container.GrowVertical = Control.GrowDirection.Both;
		container.CustomMinimumSize = new Vector2(300, 300);
		
		// Game Over Label
		_gameOverLabel = new Label();
		_gameOverLabel.Text = "GAME OVER";
		_gameOverLabel.HorizontalAlignment = HorizontalAlignment.Center;
		_gameOverLabel.AddThemeFontSizeOverride("font_size", 48);
		_gameOverLabel.AddThemeColorOverride("font_color", Colors.Red);
		container.AddChild(_gameOverLabel);

		// Espaciador
		container.AddChild(new Control { CustomMinimumSize = new Vector2(0, 40) });

		// Wave Label
		_waveLabel = new Label();
		_waveLabel.HorizontalAlignment = HorizontalAlignment.Center;
		_waveLabel.AddThemeFontSizeOverride("font_size", 24);
		_waveLabel.AddThemeColorOverride("font_color", Colors.White);
		container.AddChild(_waveLabel);

		// Espaciador
		container.AddChild(new Control { CustomMinimumSize = new Vector2(0, 20) });

		// Score Label
		_scoreLabel = new Label();
		_scoreLabel.HorizontalAlignment = HorizontalAlignment.Center;
		_scoreLabel.AddThemeFontSizeOverride("font_size", 24);
		_scoreLabel.AddThemeColorOverride("font_color", Colors.Yellow);
		container.AddChild(_scoreLabel);

		AddChild(container);
	}

	public void Show(int waveNumber, int gold)
	{
		_waveLabel.Text = $"Llegaste hasta la oleada {waveNumber}";
		_scoreLabel.Text = $"Oro acumulado: {gold}";
		Visible = true;
	}
}
