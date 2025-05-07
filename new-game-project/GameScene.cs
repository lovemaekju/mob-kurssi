using Godot;
using Shooter;

public partial class GameScene : Node
{
	[Export] public Player Player;
	[Export] public Label HealthLabel;
	[Export] public Label ScoreLabel;
	[Export] public Vector2 PlayerSpawnPosition = new(300, 300);
	
	private int _score = 0;
	private PackedScene _coinScene;
	private PackedScene _playerScene;
	private Timer _spawnTimer;
	//private AudioStreamPlayer2D _asp2;
	private AudioStreamPlayer2D _asp3;
	private AudioStreamPlayer2D _asp4;
	
		public override void _Ready()
	{
		_coinScene = ResourceLoader.Load<PackedScene>("res://Coin.tscn");
		_playerScene = ResourceLoader.Load<PackedScene>("res://player.tscn");
		GetNode<Button>("Control/Button").Pressed += OnButtonPressed;
		GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Finished += OnASPFinish;
		//_asp2 = GetNode<AudioStreamPlayer2D>("ASP2")
		_asp3 = GetNode<AudioStreamPlayer2D>("ASP3");
		_asp4 = GetNode<AudioStreamPlayer2D>("ASP4");
		
		
		InitializeGame();
	}

	private void InitializeGame()
{
	
	// 1. Clear all enemies first
	GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Play();
	GetNode<Control>("Control").Visible = false;
	foreach (Node node in GetTree().GetNodesInGroup("Enemies"))
	{
		if (IsInstanceValid(node)) node.QueueFree();
	}

	// 2. Clear coins
	foreach (Node child in GetChildren())
	{
		if (child is Coin) child.QueueFree();
	}

	// 3. Handle player reset
	if (IsInstanceValid(Player))
	{
		Player.DamageTaken -= OnPlayerDamageTaken;
		Player.QueueFree();
	}

	// 4. Create new player
	Player = _playerScene.Instantiate<Player>();
	AddChild(Player);
	
	// Deferred initialization
	Callable.From(InitializePlayerDeferred).CallDeferred();

	// 5. Reset other state
	_score = 0;
	UpdateUI();

	// 6. Reset timer
	_spawnTimer?.QueueFree();
	_spawnTimer = new Timer();
	AddChild(_spawnTimer);
	_spawnTimer.Timeout += SpawnCoin;
	_spawnTimer.Start(2.0f);
}
private void OnASPFinish()
{
	GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Play();
}

private void InitializePlayerDeferred()
{
	Player.DamageTaken += OnPlayerDamageTaken;
	
	// Reconnect joystick
	var joystick = GetNode<VirtualJoystick>("Joy");;
	if (joystick != null)
	{
		Player.Joystick = joystick;
		GD.Print("Joystick reconnected to player");
	}
	
	Player.Health = 5;
	Player.GlobalPosition = PlayerSpawnPosition;
}

	private void OnPlayerDamageTaken(int currentHealth)
	{
		_asp3.Play();
		HealthLabel.Text = $"HP: {currentHealth}";
		
		if (currentHealth <= 0)
		{
			//HealthLabel.Text = "Game Over";
			// Add auto-reset after delay
			//GetTree().CreateTimer(2.0).Timeout += InitializeGame;
			GetNode<Control>("Control").Visible = true;
			GetNode<Label>("Control/ScoreLabel").Text = $"Score: {_score}";
		}
	}
	
	private void SpawnCoin()
	{
		var coin = _coinScene.Instantiate<Coin>();
		AddChild(coin);
		
		coin.CoinCollected += OnCoinCollected;
		var viewportSize = GetViewport().GetVisibleRect().Size;
		coin.Position = new Vector2(
			(float)GD.RandRange(50, viewportSize.X - 50),
			(float)GD.RandRange(50, viewportSize.Y - 50)
		);
	}
	
	private void OnCoinCollected()
	{
		_asp4.Play();
		_score += 10;
		UpdateUI();
	}
	
	//private void OnPlayerDamageTaken(int currentHealth)
	//{
		//HealthLabel.Text = $"HP: {currentHealth}";
		//
		//if (currentHealth <= 0)
		//{
			//HealthLabel.Text = "Game Over";
		//}
	//}
	
	private void UpdateUI()
	{
		HealthLabel.Text = $"HP: {Player.Health}";
		ScoreLabel.Text = $"Score: {_score}";
	}
	private void OnButtonPressed()
	{
		ResetGame();
	}
	
	// Call this from anywhere to reset the game
	public void ResetGame()
	{
		InitializeGame();
	}

}
