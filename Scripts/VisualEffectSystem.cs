using Godot;
using System;
using System.Collections.Generic;

public partial class VisualEffectSystem : Node
{
	private static VisualEffectSystem _instance;
		public static VisualEffectSystem Instance => _instance;

	private PackedScene _effectScene;

	public override void _EnterTree()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

		public override void _Ready()
	{
		_effectScene = GD.Load<PackedScene>("res://Scenes/VisualEffect.tscn");
	}

		public void CreateEffect(Vector2 position, EffectType type)
	{
		if (_effectScene == null)
			return;

			var effect = _effectScene.Instantiate<VisualEffect>();
		GetTree().Root.AddChild(effect);
		effect.GlobalPosition = position;
		effect.Initialize(type);
	}

		public void CreateDamageNumber(Vector2 position, float amount, Color color)
	{
			var label = new Label();
		GetTree().Root.AddChild(label);
		label.Text = amount.ToString("F0");
		label.GlobalPosition = position;
		label.AddThemeColorOverride("font_color", color);
		label.AddThemeFontSizeOverride("font_size", 32);
		label.HorizontalAlignment = HorizontalAlignment.Center;

			var panel = new PanelContainer();
		label.AddChild(panel);
		panel.ZIndex = -1;
		panel.Modulate = new Color(0, 0, 0, 0.5f);
		
		var tween = CreateTween();
		tween.SetParallel(true);
		
			tween.TweenProperty(label, "global_position", 
			position + new Vector2(0, -120),
			2.0f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);

			tween.TweenProperty(label, "modulate:a", 0.0f, 2.0f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.InOut);

		tween.TweenCallback(Callable.From(() => label.QueueFree()));
	}

	public void CreateTowerPlacementEffect(Vector2 position)
	{
		CreateEffect(position, EffectType.TowerPlaced);
	}

	public void CreateTowerSoldEffect(Vector2 position)
	{
		CreateEffect(position, EffectType.TowerSold);
	}

	public void CreateEnemyDeathEffect(Vector2 position)
	{
		CreateEffect(position, EffectType.EnemyDeath);
	}

	public void CreateBaseHitEffect(Vector2 position)
{
	if (_effectScene == null)
		return;

		var effect = _effectScene.Instantiate<VisualEffect>();
	GetTree().Root.AddChild(effect);
	effect.GlobalPosition = position;
	

	var visual = new ColorRect();
	effect.AddChild(visual);
	
	visual.Size = new Vector2(32, 32);
	visual.Position = new Vector2(-16, -16); 
		visual.Color = new Color(1, 0, 0, 0.9f);
	
	var tween = effect.CreateTween();
	tween.SetParallel(true);
	
	
	
		tween.TweenProperty(visual, "scale", new Vector2(1.5f, 1.5f), 0.2f)
		.SetTrans(Tween.TransitionType.Cubic)
		.SetEase(Tween.EaseType.Out);
	
	tween.TweenProperty(visual, "modulate:a", 0.0f, 0.3f)
		.SetTrans(Tween.TransitionType.Cubic)
		.SetEase(Tween.EaseType.In);
	
	tween.TweenCallback(Callable.From(() => effect.QueueFree()));
}
}
