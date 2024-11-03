using Godot;
using System;
using System.Collections.Generic;

public partial class GameUI : Control, IGameEventObserver
{
	private Label _goldLabel;
	private Label _livesLabel;
	private Label _scoreLabel;
	private Label _messageLabel;
	private VBoxContainer _messageContainer;
	private List<Label> _messagePool;
	private const int MESSAGE_POOL_SIZE = 5;
	
	public override void _Ready()
	{
		CreateTopHUD();
		CreateMessageSystem();
		GameEventSystem.Instance.AddObserver(this);
	}

	public override void _ExitTree()
	{
		GameEventSystem.Instance.RemoveObserver(this);
	}

	private void CreateTopHUD()
	{
		var topPanel = new PanelContainer();
		topPanel.SetAnchorsPreset(Control.LayoutPreset.TopWide);
		AddChild(topPanel);

		var topContainer = new HBoxContainer();
		topContainer.CustomMinimumSize = new Vector2(0, 40);
		topPanel.AddChild(topContainer);

		var goldContainer = new HBoxContainer();
		_goldLabel = new Label();
		_goldLabel.AddThemeColorOverride("font_color", Colors.Yellow);
		_goldLabel.AddThemeFontSizeOverride("font_size", 24);
		goldContainer.AddChild(_goldLabel);
		topContainer.AddChild(goldContainer);

		var spacer1 = new Control();
		spacer1.SizeFlagsHorizontal = Control.SizeFlags.Expand;
		topContainer.AddChild(spacer1);

		var livesContainer = new HBoxContainer();
		_livesLabel = new Label();
		_livesLabel.AddThemeColorOverride("font_color", Colors.Red);
		_livesLabel.AddThemeFontSizeOverride("font_size", 24);
		livesContainer.AddChild(_livesLabel);
		topContainer.AddChild(livesContainer);

		var spacer2 = new Control();
		spacer2.SizeFlagsHorizontal = Control.SizeFlags.Expand;
		topContainer.AddChild(spacer2);

		var scoreContainer = new HBoxContainer();
		_scoreLabel = new Label();
		_scoreLabel.AddThemeColorOverride("font_color", Colors.White);
		_scoreLabel.AddThemeFontSizeOverride("font_size", 24);
		scoreContainer.AddChild(_scoreLabel);
		topContainer.AddChild(scoreContainer);

		topContainer.AddThemeConstantOverride("margin_left", 20);
		topContainer.AddThemeConstantOverride("margin_right", 20);
		topContainer.AddThemeConstantOverride("margin_top", 10);
		topContainer.AddThemeConstantOverride("margin_bottom", 10);
	}

	private void CreateMessageSystem()
	{
		_messageContainer = new VBoxContainer();
		_messageContainer.SetAnchorsPreset(Control.LayoutPreset.CenterRight);
		_messageContainer.Position = new Vector2(-400, -200);
		_messageContainer.CustomMinimumSize = new Vector2(300, 0);
		AddChild(_messageContainer);

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

	public void OnGameEvent(GameEvent gameEvent)
	{
		switch (gameEvent)
		{
			case ResourceEvent resourceEvent:
				HandleResourceEvent(resourceEvent);
				break;
			case WaveEvent waveEvent:
				HandleWaveEvent(waveEvent);
				break;
			case TowerEvent towerEvent:
				HandleTowerEvent(towerEvent);
				break;
			case EnemyEvent enemyEvent:
				HandleEnemyEvent(enemyEvent);
				break;
		}
	}

	private void HandleResourceEvent(ResourceEvent evt)
	{
		switch (evt.Type)
		{
			case ResourceType.Gold:
				_goldLabel.Text = $"Oro: {evt.NewAmount}";
				if (evt.NewAmount < evt.PreviousAmount)
				{
					ShowMessage($"Oro gastado: -{evt.PreviousAmount - evt.NewAmount}");
				}
				else if (evt.NewAmount > evt.PreviousAmount)
				{
					ShowMessage($"¡+{evt.NewAmount - evt.PreviousAmount} oro!");
				}
				break;

			case ResourceType.Lives:
				_livesLabel.Text = $"Vidas: {evt.NewAmount}";
				if (evt.NewAmount < evt.PreviousAmount)
				{
					ShowMessage($"¡Base dañada! -{evt.PreviousAmount - evt.NewAmount} vidas");
				}
				else if (evt.NewAmount > evt.PreviousAmount)
				{
					ShowMessage($"¡+{evt.NewAmount - evt.PreviousAmount} vidas!");
				}
				break;

			case ResourceType.Score:
				_scoreLabel.Text = $"Puntuación: {evt.NewAmount}";
				break;
		}
	}

	private void HandleWaveEvent(WaveEvent evt)
	{
		string message = evt.State switch
		{
			WaveState.Starting => $"¡Comienza la oleada {evt.WaveNumber}! ({evt.EnemyCount} enemigos)",
			WaveState.Complete => $"¡Oleada {evt.WaveNumber} completada!",
			_ => $"Oleada {evt.WaveNumber} en progreso"
		};
		ShowMessage(message);
	}

	private void HandleTowerEvent(TowerEvent evt)
	{
		string message = evt.Type switch
		{
			TowerEventType.Placed => $"Torre {evt.TowerType} construida (-{evt.Cost} oro)",
			TowerEventType.Sold => $"Torre vendida (+{evt.Cost} oro)",
			TowerEventType.Upgraded => $"Torre mejorada (-{evt.Cost} oro)",
			_ => ""
		};
		if (!string.IsNullOrEmpty(message))
			ShowMessage(message);
	}

	private void HandleEnemyEvent(EnemyEvent evt)
	{
		switch (evt.Type)
		{
			case EnemyEventType.Killed:
				ShowMessage($"¡Enemigo eliminado! +{evt.Reward} oro");
				break;
			case EnemyEventType.ReachedEnd:
				ShowMessage("¡Un enemigo alcanzó la base!");
				break;
			case EnemyEventType.Damaged:
				if (evt.HealthPercentage <= 0.25f)
				{
					ShowMessage("¡Enemigo casi derrotado!");
				}
				break;
		}
	}

	public void ShowMessage(string message)
	{
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

		var lastLabel = _messagePool[_messagePool.Count - 1];
		lastLabel.Text = message;
		lastLabel.Modulate = Colors.White;
		lastLabel.Visible = true;

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
