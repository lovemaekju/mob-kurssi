using Godot;

namespace Shooter
{
	public partial class Player : Node2D
	{
		// Configuration
		//public event Action<int> HealthChanged;
		[Export] public float Speed = 300f;
		[Export] public PackedScene BulletScene;
		[Export] public VirtualJoystick Joystick;
		[Export] public float ShootCooldown = 0.3f;
		[Export] public int Health = 100;
		[Export] public float InvincibilityTime = 0.5f;
		
		
		// State
		private Vector2 _currentVelocity;
		private Vector2 _lastMovementDirection = Vector2.Right;
		private float _currentCooldown = 0f;
		private Sprite2D _playerSprite;
		private Area2D _hitbox;
		private bool _invincible = false;
		private bool _alive = true;

		public override void _Ready()
		{
			_playerSprite = GetNode<Sprite2D>("Sprite2D");
			_hitbox = GetNode<Area2D>("Hitbox");
			_hitbox.BodyEntered += OnHitboxBodyEntered;
		}

		private void OnHitboxBodyEntered(Node body)
		{
			if (!_alive || _invincible) return;
			
			if (body is Enemy enemy)
			{
				TakeDamage(1);
				_invincible = true;
				GetTree().CreateTimer(InvincibilityTime).Timeout += () => _invincible = false;
				StartInvincibilityEffect();
			}
		}

		private void StartInvincibilityEffect()
		{
			var timer = GetTree().CreateTimer(0.1f);
			timer.Timeout += () => {
				if (!_invincible) 
				{
					_playerSprite.Visible = true;
					return;
				}
				_playerSprite.Visible = !_playerSprite.Visible;
				StartInvincibilityEffect();
			};
		}

		public void TakeDamage(int damage)
		{
			if (!_alive || _invincible) return;
			
			Health -= damage;
			GD.Print($"Player took {damage} damage! Health: {Health}");
			//HealthChanged?.Invoke(Health); // This notifies the GameManager
			_playerSprite.Modulate = Colors.Red;
			GetTree().CreateTimer(0.1f).Timeout += () => _playerSprite.Modulate = Colors.White;
			
			if (Health <= 0) Die();
		}

		private void Die()
		{
			if (!_alive) return;
			
			_alive = false;
			GD.Print("Player died!");
			
			var explosion = GD.Load<PackedScene>("res://Effects/Explosion.tscn").Instantiate();
			GetParent().AddChild(explosion);
			
			if (explosion is Node2D explosionNode)
			{
				explosionNode.GlobalPosition = GlobalPosition;
			}
			
			_playerSprite.Visible = false;
			GetTree().CreateTimer(1.0f).Timeout += () => QueueFree();
		}

		public override void _Input(InputEvent @event)
		{
			if (!_alive) return;
			
			if (@event is InputEventScreenTouch touchEvent && touchEvent.Pressed)
			{
				if (Joystick == null || !Joystick.GetGlobalRect().HasPoint(touchEvent.Position))
				{
					Shoot();
				}
			}
		}

		public override void _Process(double delta)
		{
			if (!_alive) return;
			
			float deltaFloat = (float)delta;
			_currentCooldown = Mathf.Max(0, _currentCooldown - deltaFloat);
			
			Vector2 input = GetMovementInput();
			if (input != Vector2.Zero)
			{
				_currentVelocity = input * Speed;
				Position += _currentVelocity * deltaFloat;
				_lastMovementDirection = input.Normalized();
			}
			
			if (Input.IsActionJustPressed("shoot")) Shoot();
		}

		private void Shoot()
		{
			if (!_alive || _currentCooldown > 0) return;
			
			if (BulletScene == null)
			{
				GD.PrintErr("BulletScene is not assigned!");
				return;
			}

			_currentCooldown = ShootCooldown;
			Vector2 shootDirection = GetShootDirection();

			var bullet = BulletScene.Instantiate() as Bullet;
			if (bullet == null)
			{
				GD.PrintErr("Instantiated object is not a Bullet!");
				return;
			}

			GetParent().AddChild(bullet);
			bullet.GlobalPosition = GlobalPosition;
			bullet.Direction = shootDirection;
			
			GD.Print($"Bullet spawned at {GlobalPosition} with direction {shootDirection}");
			
			_playerSprite.Modulate = Colors.Yellow;
			GetTree().CreateTimer(0.1f).Timeout += () => _playerSprite.Modulate = Colors.White;
		}

		private Vector2 GetShootDirection()
		{
			if (Joystick != null && Joystick.IsActive && Joystick.Output != Vector2.Zero)
				return Joystick.Output.Normalized();
			
			return _lastMovementDirection != Vector2.Zero ? _lastMovementDirection : Vector2.Right;
		}

		private Vector2 GetMovementInput()
		{
			return (Joystick != null && Joystick.IsActive) 
				? Joystick.Output 
				: Input.GetVector("move_left", "move_right", "move_up", "move_down");
		}
	}
}
