using Godot;
using System;
using Shooter;

public partial class Boundary : Area2D
{
	[Export] public NodePath PlayerPath;
	private Player _player;

	public override void _Ready()
	{
		_player = GetNode<Player>(PlayerPath);
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body == _player)
		{
			Vector2 viewportCenter = GetViewport().GetVisibleRect().Size / 2;
			_player.Position = _player.Position.LimitLength(viewportCenter.Length() * 0.9f);
		}
	}
}
