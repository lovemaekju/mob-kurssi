using Godot;
using Shooter;

public partial class Enemy : CharacterBody2D
{
	[Export] public float Speed = 80f;
	[Export] public int Health = 3;
	[Export] public float Damage = 1;
	
	private Player _target;
	private Area2D _attackArea;

	public void Initialize(Player player)
	{
		_target = player;
		_attackArea = GetNode<Area2D>("AttackArea");
		_attackArea.BodyEntered += OnAttackAreaEntered;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_target != null)
		{
			var direction = (_target.GlobalPosition - GlobalPosition).Normalized();
			Velocity = direction * Speed;
			MoveAndSlide();
			
			if (direction != Vector2.Zero)
			{
				Rotation = direction.Angle();
			}
		}
	}

	public void TakeDamage(int amount)
	{
		Health -= amount;
		
		var sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.Modulate = Colors.Red;
		GetTree().CreateTimer(0.1).Timeout += () => sprite.Modulate = Colors.White;
		
		if (Health <= 0) Die();
	}

	private void Die()
	{
		var explosion = GD.Load<PackedScene>("res://Effects/Explosion.tscn").Instantiate();
		GetParent().AddChild(explosion);
		
		// Safe position setting
		if (explosion is Node2D explosionNode)
		{
			explosionNode.GlobalPosition = GlobalPosition;
		}
		else
		{
			GD.PrintErr("Explosion scene root must be Node2D-derived!");
		}
		
		QueueFree();
	}

	private void OnAttackAreaEntered(Node body)
	{
		if (body is Player player)
		{
			player.TakeDamage((int)Damage);
		}
	}
}
