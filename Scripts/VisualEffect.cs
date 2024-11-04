using Godot;
using System;

public partial class VisualEffect : Node2D
{
	float defDuration = 2.0f;
	float boomDuration = 1.0f;
	ColorRect vfx;

	public void Initialize(EffectType type)
	{
		MakeVisual(type);
		PlayAnim(type);
	}

	void MakeVisual(EffectType type)
	{
		vfx = new ColorRect();
		AddChild(vfx);

		switch (type)
		{
			case EffectType.TowerPlaced:
				vfx.Size = new Vector2(100, 100);
				vfx.Position = new Vector2(-50, -50);
				vfx.Color = new Color(0, 1, 0, 0.8f);
				break;

			case EffectType.TowerSold:
				vfx.Size = new Vector2(100, 100);
				vfx.Position = new Vector2(-50, -50);
				vfx.Color = new Color(1, 0, 0, 0.8f);
				break;

			case EffectType.EnemyDeath:
				vfx.Size = new Vector2(32, 32);
				vfx.Position = new Vector2(-16, -16);
				vfx.Color = new Color(1, 0.7f, 0, 0.9f);
				break;

			case EffectType.BaseHit:
				vfx.Size = new Vector2(150, 150);
				vfx.Position = new Vector2(-75, -75);
				vfx.Color = new Color(1, 0, 0, 0.9f);
				break;
		}
	}

	void PlayAnim(EffectType type)
	{
		var tween = CreateTween();
		tween.SetParallel(true);

		switch (type)
		{
			case EffectType.TowerPlaced:
				tween.TweenProperty(vfx, "scale", new Vector2(2.5f, 2.5f), defDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.Out);
				
				tween.TweenProperty(vfx, "rotation", Mathf.Pi * 0.5f, defDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.InOut);
				
				tween.TweenProperty(vfx, "modulate:a", 0.0f, defDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.InOut);
				break;

			case EffectType.TowerSold:
				tween.TweenProperty(vfx, "scale", new Vector2(0.1f, 0.1f), defDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.In);
				
				tween.TweenProperty(vfx, "rotation", Mathf.Pi * 2, defDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.InOut);
				
				tween.TweenProperty(vfx, "modulate:a", 0.0f, defDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.In);
				break;

			case EffectType.EnemyDeath:
				var halfBoom = boomDuration / 2;

				tween.TweenProperty(vfx, "scale", new Vector2(1.5f, 1.5f), halfBoom)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.Out);

				tween.Chain()
					.TweenProperty(vfx, "scale", new Vector2(2.0f, 2.0f), halfBoom)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.In);

				tween.TweenProperty(vfx, "rotation", Mathf.Pi, boomDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.InOut);

				tween.TweenProperty(vfx, "modulate:a", 0.0f, boomDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.InOut);
				break;

			case EffectType.BaseHit:
				var quartDur = defDuration / 4;
				
				tween.TweenProperty(vfx, "scale", new Vector2(1.5f, 1.5f), quartDur)
					.SetTrans(Tween.TransitionType.Bounce);
				
				tween.Chain()
					.TweenProperty(vfx, "scale", new Vector2(2.0f, 2.0f), quartDur)
					.SetTrans(Tween.TransitionType.Bounce);
				
				tween.TweenProperty(vfx, "modulate:a", 0.0f, defDuration)
					.SetTrans(Tween.TransitionType.Cubic)
					.SetEase(Tween.EaseType.InOut);
				break;
		}

		tween.TweenCallback(Callable.From(() => QueueFree()));
	}
}
