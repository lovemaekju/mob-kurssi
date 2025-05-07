using Godot;
using System.Linq;
using Shooter;

public partial class EnemySpawner : Node2D
{
	[Export] public PackedScene EnemyScene;
	[Export] public float SpawnInterval = 3.0f;
	[Export] public int MaxEnemies = 10;
	
	private float _timer;
	private Player _player;

	public override void _Ready()
	{
		// Safer player lookup
		RefreshPlayerReference();
	}

	public override void _Process(double delta)
	{
		// 1. Refresh player reference if needed
		if (!IsInstanceValid(_player))
		{
			RefreshPlayerReference();
			if (_player == null) return; // No player exists
		}

		// 2. Other safety checks
		if (EnemyScene == null) return;
		
		_timer += (float)delta;
		
		// 3. Safer enemy counting
		int currentEnemies = GetTree().GetNodesInGroup("Enemies")
			.Count(IsInstanceValid);

		if (_timer >= SpawnInterval && currentEnemies < MaxEnemies)
		{
			SpawnEnemy();
			_timer = 0;
		}
	}

private void RefreshPlayerReference()
{
	// Try multiple ways to find player
	_player = GetTree().GetFirstNodeInGroup("Player") as Player;
	
	if (_player == null)
	{
		_player = GetNodeOrNull<Player>("/root/GameScene/Player");
	}
	
	GD.Print("Player reference refreshed: " + (_player != null));
	
	// Debug: Print all nodes in "Player" group
	GD.Print("Nodes in 'Player' group:", 
		string.Join(", ", GetTree().GetNodesInGroup("Player").Select(n => n.Name)));
}

	private void SpawnEnemy()
	{   
		if (!IsInstanceValid(_player)) return;
		
		var enemy = EnemyScene.Instantiate<Enemy>();
		GetParent().AddChild(enemy);
		enemy.GlobalPosition = GetRandomPosition();
		
		// Pass player reference safely
		if (IsInstanceValid(_player))
		{
			enemy.Initialize(_player);
		}
		GD.Print($"Spawned enemy at {enemy.GlobalPosition}");
	}

	private Vector2 GetRandomPosition()
	{
		if (!IsInstanceValid(_player)) return Vector2.Zero;
		
		float randomAngle = GD.Randf() * Mathf.Pi * 2;
		float distance = GD.Randf() * 200f + 300f;
		return _player.GlobalPosition + new Vector2(
			Mathf.Cos(randomAngle) * distance,
			Mathf.Sin(randomAngle) * distance
		);
	}
}
