using Godot;
using Shooter;

public partial class GameScene : Node
{
	// Assign these in the inspector!
	[Export] public Player Player;
	[Export] public Label HealthLabel;

	public override void _Process(double delta)
	{
		if (Player != null && HealthLabel != null)
		{
			HealthLabel.Text = $"HP: {Player.Health}";
			
			// Optional color coding
			if (Player.Health < 30) HealthLabel.Modulate = Colors.Red;
			else if (Player.Health < 70) HealthLabel.Modulate = Colors.Yellow;
			else HealthLabel.Modulate = Colors.Green;
		}
	}
}
