using Godot;
using System;
using System.Collections.Generic;

public partial class GameUI : Control
{
	private Label _goldLabel;
	private Label _livesLabel;
	private Label _messageLabel;
	private VBoxContainer _messageContainer;
	private List<Label> _messagePool;
	private const int MESSAGE_POOL_SIZE = 5;
	
	public override void _Ready()
	{
		CreateTopHUD();
		CreateMessageSystem();
	}

	private void CreateTopHUD()
	{
		// Panel superior para recursos
		var topPanel = new PanelContainer();
		topPanel.SetAnchorsPreset(Control.LayoutPreset.TopWide);
		AddChild(topPanel);

		var topContainer = new HBoxContainer();
		topContainer.CustomMinimumSize = new Vector2(0, 40);
		topPanel.AddChild(topContainer);

		// Contenedor para el oro
		var goldContainer = new HBoxContainer();
		_goldLabel = new Label();
		_goldLabel.AddThemeColorOverride("font_color", Colors.Yellow);
		_goldLabel.AddThemeFontSizeOverride("font_size", 24);
		goldContainer.AddChild(_goldLabel);
		topContainer.AddChild(goldContainer);

		// Espaciador
		var spacer = new Control();
		spacer.SizeFlagsHorizontal = Control.SizeFlags.Expand;
		topContainer.AddChild(spacer);

		// Contenedor para las vidas
		var livesContainer = new HBoxContainer();
		_livesLabel = new Label();
		_livesLabel.AddThemeColorOverride("font_color", Colors.Red);
		_livesLabel.AddThemeFontSizeOverride("font_size", 24);
		livesContainer.AddChild(_livesLabel);
		topContainer.AddChild(livesContainer);

		// Agregar padding
		topContainer.AddThemeConstantOverride("margin_left", 20);
		topContainer.AddThemeConstantOverride("margin_right", 20);
		topContainer.AddThemeConstantOverride("margin_top", 10);
		topContainer.AddThemeConstantOverride("margin_bottom", 10);
	}

	private void CreateMessageSystem()
	{
		// Contenedor para mensajes
		_messageContainer = new VBoxContainer();
		_messageContainer.SetAnchorsPreset(Control.LayoutPreset.CenterRight);
		_messageContainer.Position = new Vector2(-400, -200); // Ajusta según necesites
		_messageContainer.CustomMinimumSize = new Vector2(300, 0);
		AddChild(_messageContainer);

		// Inicializar pool de mensajes
		_messagePool = new List<Label>();
		for (int i = 0; i < MESSAGE_POOL_SIZE; i++)
		{
			var label = new Label();
			label.HorizontalAlignment = HorizontalAlignment.Center;
			label.AddThemeFontSizeOverride("font_size", 20);
			label.Visible = false;
			_messagePool.Add(label);
			_messageContainer.AddChild(label);
		}
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
		// Mover mensajes existentes hacia arriba
		for (int i = 0; i < _messagePool.Count - 1; i++)
		{
			if (_messagePool[i + 1].Visible)
			{
				_messagePool[i].Text = _messagePool[i + 1].Text;
				_messagePool[i].Modulate = _messagePool[i + 1].Modulate;
				_messagePool[i].Visible = true;
			}
			else
			{
				_messagePool[i].Visible = false;
			}
		}

		// Mostrar nuevo mensaje en la última posición
		var lastLabel = _messagePool[_messagePool.Count - 1];
		lastLabel.Text = message;
		lastLabel.Modulate = Colors.White;
		lastLabel.Visible = true;

		// Crear y empezar el timer para desvanecer el mensaje
		var timer = GetTree().CreateTimer(2.0);
		timer.Timeout += () => StartFadeOut(lastLabel);
	}

	private void StartFadeOut(Label label)
	{
		var tween = CreateTween();
		tween.TweenProperty(label, "modulate:a", 0.0, 1.0);
		tween.TweenCallback(Callable.From(() => label.Visible = false));
	}
}
