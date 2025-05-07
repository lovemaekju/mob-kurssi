using Godot;
using Shooter;

public partial class Bullet : Area2D
{
	[Export] public float Speed = 600f;
	[Export] public int Damage = 1;
	public Vector2 Direction { get; set; } = Vector2.Right; // Add this property
	private AudioStreamPlayer2D _asp;
	
	public override void _Ready()
	{
		_asp = GetNode<AudioStreamPlayer2D>("ASP2");
		_asp.Play();
		BodyEntered += OnBodyEntered;
	}
public override void _PhysicsProcess(double delta)
	{
		Position += Direction * Speed * (float)delta; // Use Direction instead of Transform.X
	}
	
	private void OnBodyEntered(Node body)
	{
		GD.Print($"Bullet hit: {body.Name}");
		
		if (body is Enemy enemy)
		{
			enemy.TakeDamage(Damage);
			GD.Print("Enemy hit confirmed!");
			QueueFree(); // Destroy bullet on any hit
		}
		
		
	}
}
