using Godot;
using System;

public partial class VisualEffectSystem : Node
{
	static VisualEffectSystem instance;
	public static VisualEffectSystem Instance => instance;

	PackedScene vfxScene;

	public override void _EnterTree()
	{
		if (instance == null)
			instance = this;
	}

	public override void _Ready()
	{
		vfxScene = GD.Load<PackedScene>("res://Scenes/VisualEffect.tscn");
	}

	public void CreateEffect(Vector2 pos, EffectType type)
	{
		if (vfxScene == null) return;

		var effect = vfxScene.Instantiate<VisualEffect>();
		GetTree().Root.AddChild(effect);
		effect.GlobalPosition = pos;
		effect.Initialize(type);
	}

	public void CreateDamageNumber(Vector2 pos, float amount, Color color)
	{
		var txt = new Label();
		GetTree().Root.AddChild(txt);
		txt.Text = amount.ToString("F0");
		txt.GlobalPosition = pos;
		txt.AddThemeColorOverride("font_color", color);
		txt.AddThemeFontSizeOverride("font_size", 32);
		txt.HorizontalAlignment = HorizontalAlignment.Center;

		var bgPanel = new PanelContainer();
		txt.AddChild(bgPanel);
		bgPanel.ZIndex = -1;
		bgPanel.Modulate = new Color(0, 0, 0, 0.5f);
		
		var tween = CreateTween();
		tween.SetParallel(true);

		tween.TweenProperty(txt, "global_position", 
			pos + new Vector2(0, -120),
			2.0f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);

		tween.TweenProperty(txt, "modulate:a", 0.0f, 2.0f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.InOut);

		tween.TweenCallback(Callable.From(() => txt.QueueFree()));
	}

public void CreateTowerPlacementEffect(Vector2 pos)
	{
		CreateTowerPlacedEffect(pos);
	}

	public void CreateTowerPlacedEffect(Vector2 pos)
	{
		CreateEffect(pos, EffectType.TowerPlaced);
	}

	public void CreateTowerSoldEffect(Vector2 pos)
	{
		CreateEffect(pos, EffectType.TowerSold);
	}

	public void CreateEnemyDeathEffect(Vector2 pos)
	{
		CreateEffect(pos, EffectType.EnemyDeath);
	}

	public void CreateBaseHitEffect(Vector2 pos)
	{
		if (vfxScene == null) return;

		var boom = vfxScene.Instantiate<VisualEffect>();
		GetTree().Root.AddChild(boom);
		boom.GlobalPosition = pos;

		var visual = new ColorRect();
		boom.AddChild(visual);
		
		visual.Size = new Vector2(32, 32);
		visual.Position = new Vector2(-16, -16); 
		visual.Color = new Color(1, 0, 0, 0.9f);
		
		var tween = boom.CreateTween();
		tween.SetParallel(true);
		
		tween.TweenProperty(visual, "scale", new Vector2(1.5f, 1.5f), 0.2f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);
		
		tween.TweenProperty(visual, "modulate:a", 0.0f, 0.3f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.In);
		
		tween.TweenCallback(Callable.From(() => boom.QueueFree()));
	}
}
