using Godot;
using System.Collections.Generic;
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
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		GD.Print("Player found: " + (_player != null)); // Debug

		if (EnemyScene == null)
			GD.PrintErr("EnemyScene not assigned in inspector!");
	}

	public override void _Process(double delta)
	{
		if (_player == null || EnemyScene == null) 
			return;
		
		_timer += (float)delta;
		int currentEnemies = GetTree().GetNodesInGroup("Enemies").Count;
		
		GD.Print($"Timer: {_timer}, Enemies: {currentEnemies}"); // Debug

		if (_timer >= SpawnInterval && currentEnemies < MaxEnemies)
		{
			SpawnEnemy();
			_timer = 0;
		}
	}

	private void SpawnEnemy()
	{   
		var enemy = EnemyScene.Instantiate<Enemy>();
		GetParent().AddChild(enemy);
		enemy.GlobalPosition = GetRandomPosition();
		enemy.Initialize(_player);
		GD.Print($"Spawned enemy at {enemy.GlobalPosition}"); // Debug
	}

	private Vector2 GetRandomPosition()
	{
		float randomAngle = GD.Randf() * Mathf.Pi * 2;
		float distance = GD.Randf() * 200f + 300f;
		return _player.GlobalPosition + new Vector2(
			Mathf.Cos(randomAngle) * distance,
			Mathf.Sin(randomAngle) * distance
		);
	}
}
