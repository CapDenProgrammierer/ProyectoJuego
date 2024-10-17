using Godot;
using System;

public partial class Enemigo1 : CharacterBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		$AnimationPlayer.play(AnimacionPrueba);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var direccion = Input.get_vector("ui_left", "")
	}
}
