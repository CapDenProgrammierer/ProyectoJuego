using Godot;
using System;

public partial class GameUI : Control
{
	private Label _goldLabel;
	private Label _livesLabel;
	private Label _statusLabel;
	
	public override void _Ready()
	{
		// Crear contenedor principal
		var container = new VBoxContainer();
		container.Position = new Vector2(10, 10);
		AddChild(container);

		// Crear labels para oro y vidas
		_goldLabel = new Label();
		_goldLabel.AddThemeColorOverride("font_color", Colors.Yellow);
		container.AddChild(_goldLabel);

		_livesLabel = new Label();
		_livesLabel.AddThemeColorOverride("font_color", Colors.Red);
		container.AddChild(_livesLabel);

		// Label para mensajes de estado
		_statusLabel = new Label();
		_statusLabel.Position = new Vector2(10, 600); // Ajusta según tu resolución
		_statusLabel.HorizontalAlignment = HorizontalAlignment.Left;
		AddChild(_statusLabel);

		// Configurar timer para limpiar mensajes de estado
		var timer = new Timer();
		timer.WaitTime = 2.0f;
		timer.OneShot = false;
		timer.Timeout += ClearStatusMessage;
		AddChild(timer);
		timer.Start();
	}

	public override void _Process(double delta)
	{
		if (GameManager.Instance != null)
		{
			_goldLabel.Text = $"Oro: {GameManager.Instance.Gold}";
			_livesLabel.Text = $"Vidas: {GameManager.Instance.Lives}";
		}
	}

	public void ShowMessage(string message)
	{
		_statusLabel.Text = message;
	}

	private void ClearStatusMessage()
	{
		_statusLabel.Text = "";
	}
}
