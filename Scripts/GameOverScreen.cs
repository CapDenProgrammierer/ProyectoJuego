using Godot;
using System;

public partial class GameOverScreen : Control
{
	Label mainText;
	Label waveText;
	Label scoreText;
	Panel darkBg;

	public override void _Ready()
	{
		SetAnchorsPreset(LayoutPreset.FullRect);
		CreateBg();
		SetupUI();
		Hide();
		
		ProcessMode = ProcessModeEnum.Always;
	}

	void CreateBg()
	{
		darkBg = new Panel();
		darkBg.SetAnchorsPreset(LayoutPreset.FullRect);
		darkBg.Modulate = new Color(0, 0, 0, 0.9f);
		AddChild(darkBg);
	}

	void SetupUI()
	{
		var content = new VBoxContainer();
		content.SetAnchorsPreset(LayoutPreset.Center);
		content.GrowHorizontal = GrowDirection.Both;
		content.GrowVertical = GrowDirection.Both;
		content.Size = new Vector2(600, 400);
		content.Position = new Vector2(-300, -200); 

		mainText = new Label
		{
			Text = "¡GAME OVER!",
			HorizontalAlignment = HorizontalAlignment.Center
		};
		mainText.AddThemeFontSizeOverride("font_size", 64);
		mainText.AddThemeColorOverride("font_color", Colors.Red);
		content.AddChild(mainText);

		content.AddChild(new Control { CustomMinimumSize = new Vector2(0, 50) });

		waveText = new Label
		{
			HorizontalAlignment = HorizontalAlignment.Center
		};
		waveText.AddThemeFontSizeOverride("font_size", 32);
		waveText.AddThemeColorOverride("font_color", Colors.White);
		content.AddChild(waveText);

		content.AddChild(new Control { CustomMinimumSize = new Vector2(0, 30) });

		scoreText = new Label
		{
			HorizontalAlignment = HorizontalAlignment.Center
		};
		scoreText.AddThemeFontSizeOverride("font_size", 32);
		scoreText.AddThemeColorOverride("font_color", Colors.Yellow);
		content.AddChild(scoreText);

		content.AddChild(new Control { CustomMinimumSize = new Vector2(0, 50) });

		var restartMsg = new Label
		{
			Text = "Presiona cualquier tecla para reiniciar",
			HorizontalAlignment = HorizontalAlignment.Center
		};
		restartMsg.AddThemeFontSizeOverride("font_size", 24);
		restartMsg.AddThemeColorOverride("font_color", Colors.White);
		content.AddChild(restartMsg);

		AddChild(content);
	}

	public new void Show(int wave, int gold)
	{
		waveText.Text = $"Llegaste hasta la oleada {wave}";
		scoreText.Text = $"Oro acumulado: {gold}";

		Visible = true;
		ZIndex = 1000;
	
		Modulate = new Color(1, 1, 1, 0);
		var tween = CreateTween();
		tween.TweenProperty(this, "modulate:a", 1.0f, 0.5f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);
	}
}
