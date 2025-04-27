using Godot;

namespace Shooter
{
public partial class Player : Node2D
{
	// Configuration
	[Export] public float Speed = 300f;
	[Export] public float BulletSpeed = 600f;
	[Export] public PackedScene BulletScene;
	[Export] public VirtualJoystick Joystick;
	[Export] public float ShootCooldown = 0.3f;
	
	// State
	private Vector2 _currentVelocity;
	private Vector2 _lastMovementDirection = Vector2.Right;
	private float _currentCooldown = 0f;
	private Sprite2D _playerSprite;
	[Export] public int Health = 100;
	
	public void TakeDamage(int damage)
	{
		Health -= damage;
		GD.Print($"Player took {damage} damage! Health: {Health}");
		
		// Visual feedback
		GetNode<Sprite2D>("Sprite2D").Modulate = Colors.Red;
		GetTree().CreateTimer(0.1).Timeout += () => GetNode<Sprite2D>("Sprite2D").Modulate = Colors.White;
		
		if (Health <= 0)
		{
			Die();
		}
	}
	
	private void Die()
	{
		GD.Print("Player died!");
		// Add your death handling here
	}

	public override void _Ready()
	{
		_playerSprite = GetNode<Sprite2D>("Sprite2D");
	}

	public override void _Input(InputEvent @event)
	{
		// Handle touch shooting
		if (@event is InputEventScreenTouch touchEvent && touchEvent.Pressed)
		{
			// Only shoot if touch is outside joystick area
			if (Joystick == null || !Joystick.GetGlobalRect().HasPoint(touchEvent.Position))
			{
				Shoot();
			}
		}
	}

	public override void _Process(double delta)
	{
		float deltaFloat = (float)delta;
		
		// Handle cooldown
		_currentCooldown = Mathf.Max(0, _currentCooldown - deltaFloat);
		
		// Movement
		Vector2 input = GetMovementInput();
		if (input != Vector2.Zero)
		{
			_currentVelocity = input * Speed;
			Position += _currentVelocity * deltaFloat;
			_lastMovementDirection = input.Normalized();
		}
		//GD.Print("Player position: ", GlobalPosition);
//GD.Print("Last movement dir: ", _lastMovementDirection);
if (Joystick != null) 
	//GD.Print("Joystick output: ", Joystick.Output);
		
		// Keyboard shooting
		if (Input.IsActionJustPressed("shoot"))
		{
			Shoot();
		}
	}

private void Shoot()
{
	// Debug current state
	GD.Print("--- SHOOTING DEBUG ---");
	GD.Print($"Player Position: {GlobalPosition}");
	GD.Print($"Last Movement: {_lastMovementDirection}");
	if (Joystick != null)
	{
		GD.Print($"Joystick Active: {Joystick.IsActive}");
		GD.Print($"Joystick Output: {Joystick.Output}");
	}

	if (BulletScene == null)
	{
		GD.PrintErr("BulletScene is not assigned!");
		return;
	}

	if (_currentCooldown > 0) return;
	_currentCooldown = ShootCooldown;

	// Get final direction
	Vector2 shootDirection = GetShootDirection();
	GD.Print($"Final Shoot Direction: {shootDirection}");

	// Spawn bullet
	var bullet = BulletScene.Instantiate();
	GetParent().AddChild(bullet);

	if (bullet is Bullet bulletScript)
	{
		// CRITICAL POSITION FIX
		bulletScript.GlobalPosition = GlobalPosition;
		bulletScript.Direction = shootDirection;
		
		// Immediate debug of bullet state
		GD.Print($"Bullet Spawned At: {bulletScript.GlobalPosition}");
		GD.Print($"Bullet Direction: {bulletScript.Direction}");
		
		// Visual marker for testing
		bulletScript.GetNode<Sprite2D>("Sprite2D").Modulate = Colors.Red;
	}

	// Player feedback
	_playerSprite.Modulate = Colors.Yellow;
	GetTree().CreateTimer(0.1).Timeout += () => _playerSprite.Modulate = Colors.White;
}

private Vector2 GetShootDirection()
{
	// 1. Priority to joystick direction
	if (Joystick != null && Joystick.IsActive && Joystick.Output != Vector2.Zero)
	{
		return Joystick.Output.Normalized();
	}
	
	// 2. Use last movement direction
	if (_lastMovementDirection != Vector2.Zero)
	{
		return _lastMovementDirection;
	}
	
	// 3. Default to right
	return Vector2.Right;
}

	private Vector2 GetMovementInput()
	{
		// Priority to virtual joystick
		if (Joystick != null && Joystick.IsActive)
		{
			return Joystick.Output;
		}
		
		// Fallback to keyboard input
		return Input.GetVector("move_left", "move_right", "move_up", "move_down");
	}

}
}
