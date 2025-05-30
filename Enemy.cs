using Godot;
using Shooter;

public partial class Enemy : CharacterBody2D
{
	[Export] public float Speed = 80f;
	private Player _target;
	private Area2D _attackArea;

	public void Initialize(Player player)
	{
		_target = player;
		_attackArea = GetNode<Area2D>("AttackArea");
		_attackArea.BodyEntered += OnAttackAreaEntered;
		player.TreeExiting += () => _target = null; 
		GD.Print("Enemy spawned at: ", GlobalPosition);
	}

	public override void _PhysicsProcess(double delta)
	{
				if (!IsInstanceValid(_target))
		{
			// Handle missing target (e.g., wander or despawn)
			return;
		}
		if (_target != null)
		{
			var direction = (_target.GlobalPosition - GlobalPosition).Normalized();
			Velocity = direction * Speed;
			MoveAndSlide();
			Rotation = direction.Angle();
		}
	}

public void TakeDamage(int damageAmount = 1)  // Now accepts parameter but ignores it
{
	GD.Print($"Enemy took damage (always dies in one hit)");
	Die();
}

	private void Die()
	{
		
		QueueFree();
	}

	private void OnAttackAreaEntered(Node body)
	{
		if (body is Player player)
		{
			player.TakeDamage(1); // Player takes damage when touching enemy
		}
	}
}
