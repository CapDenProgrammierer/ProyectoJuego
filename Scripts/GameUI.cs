using Godot;
using System;
using System.Collections.Generic;

public partial class GameUI : Control, IGameEventObserver
{
	Label goldLabel;
	Label livesLabel;
	Label scoreLabel;
	VBoxContainer messageContainer;
	readonly List<Label> messageLabels = new();
	const int MAX_MESSAGES = 5;
	
	public override void _Ready()
	{
		SetupHUD();
		SetupMessageSystem();
		GameEventSystem.Instance?.AddObserver(this);
	}

	public override void _ExitTree()
	{
		GameEventSystem.Instance?.RemoveObserver(this);
	}

	void SetupHUD()
	{
		var panel = new PanelContainer();
		panel.SetAnchorsPreset(LayoutPreset.TopWide);
		AddChild(panel);

		var container = new HBoxContainer();
		container.CustomMinimumSize = new Vector2(0, 40);
		panel.AddChild(container);

		var goldBox = new HBoxContainer();
		goldLabel = new Label();
		goldLabel.AddThemeColorOverride("font_color", Colors.Yellow);
		goldLabel.AddThemeFontSizeOverride("font_size", 24);
		goldBox.AddChild(goldLabel);
		container.AddChild(goldBox);

		container.AddChild(new Control { SizeFlagsHorizontal = SizeFlags.Expand });

		var livesBox = new HBoxContainer();
		livesLabel = new Label();
		livesLabel.AddThemeColorOverride("font_color", Colors.Red);
		livesLabel.AddThemeFontSizeOverride("font_size", 24);
		livesBox.AddChild(livesLabel);
		container.AddChild(livesBox);

		container.AddChild(new Control { SizeFlagsHorizontal = SizeFlags.Expand });

		var scoreBox = new HBoxContainer();
		scoreLabel = new Label();
		scoreLabel.AddThemeColorOverride("font_color", Colors.White);
		scoreLabel.AddThemeFontSizeOverride("font_size", 24);
		scoreBox.AddChild(scoreLabel);
		container.AddChild(scoreBox);

		container.AddThemeConstantOverride("margin_left", 20);
		container.AddThemeConstantOverride("margin_right", 20);
		container.AddThemeConstantOverride("margin_top", 10);
		container.AddThemeConstantOverride("margin_bottom", 10);
	}

	void SetupMessageSystem()
	{
		messageContainer = new VBoxContainer();
		messageContainer.SetAnchorsPreset(LayoutPreset.CenterRight);
		messageContainer.Position = new Vector2(-400, -200);
		messageContainer.CustomMinimumSize = new Vector2(300, 0);
		AddChild(messageContainer);

		for (int i = 0; i < MAX_MESSAGES; i++)
		{
			var lbl = new Label();
			lbl.HorizontalAlignment = HorizontalAlignment.Center;
			lbl.AddThemeFontSizeOverride("font_size", 20);
			lbl.Visible = false;
			messageLabels.Add(lbl);
			messageContainer.AddChild(lbl);
		}
	}

	public void OnGameEvent(GameEvent evt)
	{
		switch (evt)
		{
			case ResourceEvent rEvt:
				HandleResource(rEvt);
				break;
			case WaveEvent wEvt:
				HandleWave(wEvt);
				break;
			case TowerEvent tEvt:
				HandleTower(tEvt);
				break;
			case EnemyEvent eEvt:
				HandleEnemy(eEvt);
				break;
			case MessageEvent mEvt:
				ShowMessage(mEvt.Message);
				break;
		}
	}

	void HandleResource(ResourceEvent evt)
	{
		switch (evt.Type)
		{
			case ResourceType.Gold:
				goldLabel.Text = $"Oro: {evt.NewAmount}";
				if (evt.NewAmount != evt.PreviousAmount)
				{
					int diff = evt.NewAmount - evt.PreviousAmount;
					if (diff < 0)
						ShowMessage($"Oro gastado: {diff}");
					else if (diff > 0)
						ShowMessage($"+{diff} oro");
				}
				break;

			case ResourceType.Lives:
				livesLabel.Text = $"Vidas: {evt.NewAmount}";
				if (evt.NewAmount != evt.PreviousAmount)
				{
					int diff = evt.NewAmount - evt.PreviousAmount;
					if (diff < 0)
						ShowMessage($"¡Perdiste {-diff} vidas!");
					else
						ShowMessage($"+{diff} vidas");
				}
				break;

			case ResourceType.Score:
				scoreLabel.Text = $"Puntos: {evt.NewAmount}";
				break;
		}
	}

	void HandleWave(WaveEvent evt)
	{
		var msg = evt.State switch
		{
			WaveState.Starting => $"¡Oleada {evt.WaveNumber}! ({evt.EnemyCount} enemigos)",
			WaveState.Complete => $"¡Oleada {evt.WaveNumber} completada!",
			_ => $"Oleada {evt.WaveNumber}"
		};
		ShowMessage(msg);
	}

	void HandleTower(TowerEvent evt)
	{
		var msg = evt.Type switch
		{
			TowerEventType.Placed => $"Torre {evt.TowerType} construida (-{evt.Cost})",
			TowerEventType.Sold => $"Torre vendida (+{evt.Cost})",
			TowerEventType.Upgraded => $"Torre mejorada (-{evt.Cost})",
			_ => ""
		};
		
		if (!string.IsNullOrEmpty(msg))
			ShowMessage(msg);
	}

	void HandleEnemy(EnemyEvent evt)
	{
		if (evt.Type == EnemyEventType.Killed)
			ShowMessage($"¡Enemigo eliminado! +{evt.Reward}");
	}

	public void ShowMessage(string msg)
	{
		for (int i = 0; i < messageLabels.Count - 1; i++)
		{
			if (messageLabels[i + 1].Visible)
			{
				messageLabels[i].Text = messageLabels[i + 1].Text;
				messageLabels[i].Modulate = messageLabels[i + 1].Modulate;
				messageLabels[i].Visible = true;
			}
			else
				messageLabels[i].Visible = false;
		}

		var lastMsg = messageLabels[messageLabels.Count - 1];
		lastMsg.Text = msg;
		lastMsg.Modulate = Colors.White;
		lastMsg.Visible = true;

		var timer = GetTree().CreateTimer(2.0);
		timer.Timeout += () => FadeOutMessage(lastMsg);
	}

	void FadeOutMessage(Label lbl)
	{
		var tween = CreateTween();
		tween.TweenProperty(lbl, "modulate:a", 0.0, 1.0);
		tween.TweenCallback(Callable.From(() => lbl.Visible = false));
	}
}
