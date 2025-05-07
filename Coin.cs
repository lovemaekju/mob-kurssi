using Godot;
using System;
using Shooter;

public partial class Coin : Area2D
{
	[Signal]
	public delegate void CoinCollectedEventHandler();
	[Export] public float PopDuration = 0.3f;
	[Export] public Vector2 PopScale = new Vector2(1.1f, 1.1f); // Slightly larger than normal

	public override void _Ready()
	{
		// Connect the signal in code (more reliable than editor)
		BodyEntered += OnBodyEntered;

			// Set initial state (invisible and scaled down)
		SelfModulate = new Color(SelfModulate, 0f);
		Scale = Vector2.Zero;
		
		// Create the tween
		Tween popTween = CreateTween();
		
		// Animate both properties simultaneously
		popTween.Parallel(); // Start parallel animations
		
		// Fade in
		popTween.TweenProperty(this, "self_modulate:a", 1.0f, PopDuration);
		
		// Scale animation - first grow slightly larger than normal
		popTween.TweenProperty(this, "scale", PopScale, PopDuration * 0.5f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Back);
			
		// Then settle back to normal scale
		popTween.TweenProperty(this, "scale", Vector2.One, PopDuration * 0.5f)
			.SetDelay(PopDuration * 0.5f) // Start after first scale completes
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Bounce);
	}

	private void OnBodyEntered(Node2D body)
	{
		// Check multiple ways the player might be identified
		if (body is Player player)
		{
			GD.Print("Coin collected!");  // Debug output
			EmitSignal(SignalName.CoinCollected);
			QueueFree();
		}
	}
	
}
