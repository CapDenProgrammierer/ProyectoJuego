using Godot;
using System;

public partial class VisualEffect : Node2D
{
	private float _defaultDuration = 2.0f;
	private float _explosionDuration = 1.0f;
	private ColorRect _visual;

	public void Initialize(EffectType type)
	{
		CreateVisual(type);
		StartAnimation(type);
	}

	private void CreateVisual(EffectType type)
	{
		_visual = new ColorRect();
		AddChild(_visual);

		switch (type)
		{
			case EffectType.TowerPlaced:
				_visual.Size = new Vector2(100, 100);
				_visual.Position = new Vector2(-50, -50);
				_visual.Color = new Color(0, 1, 0, 0.8f);
				break;

			case EffectType.TowerSold:
				_visual.Size = new Vector2(100, 100);
				_visual.Position = new Vector2(-50, -50);
				_visual.Color = new Color(1, 0, 0, 0.8f);
				break;

			case EffectType.EnemyDeath:

				_visual.Size = new Vector2(32, 32);
				_visual.Position = new Vector2(-16, -16);
				_visual.Color = new Color(1, 0.7f, 0, 0.9f);
				break;

			case EffectType.BaseHit:
				_visual.Size = new Vector2(150, 150);
				_visual.Position = new Vector2(-75, -75);
				_visual.Color = new Color(1, 0, 0, 0.9f);
				break;
		}
	}

	private void StartAnimation(EffectType type)
	{
		var tween = CreateTween();
		tween.SetParallel(true);

		switch (type)
		{
			case EffectType.TowerPlaced:
				tween.TweenProperty(_visual, "scale", new Vector2(2.5f, 2.5f), _defaultDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.Out);
				
				tween.TweenProperty(_visual, "rotation", Mathf.Pi * 0.5f, _defaultDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.InOut);
				
				tween.TweenProperty(_visual, "modulate:a", 0.0f, _defaultDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.InOut);
				break;

			case EffectType.TowerSold:
				tween.TweenProperty(_visual, "scale", new Vector2(0.1f, 0.1f), _defaultDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.In);
				
				tween.TweenProperty(_visual, "rotation", Mathf.Pi * 2, _defaultDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.InOut);
				
				tween.TweenProperty(_visual, "modulate:a", 0.0f, _defaultDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.In);
				break;

			case EffectType.EnemyDeath:
				var halfExplosion = _explosionDuration / 2;

				tween.TweenProperty(_visual, "scale", new Vector2(1.5f, 1.5f), halfExplosion)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.Out);

				tween.Chain()
					.TweenProperty(_visual, "scale", new Vector2(2.0f, 2.0f), halfExplosion)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.In);

				tween.TweenProperty(_visual, "rotation", Mathf.Pi, _explosionDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.InOut);

				tween.TweenProperty(_visual, "modulate:a", 0.0f, _explosionDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.InOut);
				break;

			case EffectType.BaseHit:
				var quarterDuration = _defaultDuration / 4;
				
				tween.TweenProperty(_visual, "scale", new Vector2(1.5f, 1.5f), quarterDuration)
					.SetTrans(Tween.TransitionType.Bounce);
				
				tween.Chain()
					.TweenProperty(_visual, "scale", new Vector2(2.0f, 2.0f), quarterDuration)
					.SetTrans(Tween.TransitionType.Bounce);
				
				tween.TweenProperty(_visual, "modulate:a", 0.0f, _defaultDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.InOut);
				break;
		}

		tween.TweenCallback(Callable.From(() => QueueFree()));
	}
}
