using Godot;

public partial class VirtualJoystick : Control
{
	[Export] public float Radius = 100f;
	[Export] public float DeadZone = 0.2f;
	[Export] public Color BaseColor = new Color(1, 1, 1, 0.2f);
	[Export] public Color HandleColor = new Color(1, 1, 1, 0.5f);
	
	public Vector2 Output { get; private set; }
	public bool IsActive { get; private set; }
	
	private Vector2 _center;
	private int? _touchId = null;
	private Vector2 _handlePosition;

	public override void _Ready()
	{
		// Position at middle-left with some margin
		Position = new Vector2(250, GetViewportRect().Size.Y / 2);
		
		_center = Size / 2;
		_handlePosition = _center;
		
		CustomMinimumSize = new Vector2(Radius * 2, Radius * 2);
		ProcessMode = ProcessModeEnum.Always;
		ZIndex = 100;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventScreenTouch touch)
		{
			if (touch.Pressed && !_touchId.HasValue)
			{
				Vector2 localPos = GetLocalMousePosition();
				if (IsInTouchArea(localPos))
				{
					_touchId = touch.Index;
					IsActive = true;
					UpdateHandlePosition(touch.Position);
				}
			}
			else if (!touch.Pressed && touch.Index == _touchId)
			{
				ResetJoystick();
			}
		}
		else if (@event is InputEventScreenDrag drag && drag.Index == _touchId)
		{
			UpdateHandlePosition(drag.Position);
		}
	}

	private bool IsInTouchArea(Vector2 position)
	{
		return _center.DistanceTo(position) <= Radius * 1.5f;
	}

	private void UpdateHandlePosition(Vector2 screenPosition)
	{
		Vector2 localPos = GetLocalMousePosition();
		Vector2 direction = (localPos - _center);
		float distance = Mathf.Min(direction.Length(), Radius);
		
		Output = distance > DeadZone * Radius 
			? direction.Normalized() * ((distance - DeadZone * Radius) / (Radius * (1 - DeadZone)))
			: Vector2.Zero;
			
		_handlePosition = _center + (direction.Normalized() * distance);
		QueueRedraw();
	}

	private void ResetJoystick()
	{
		_touchId = null;
		IsActive = false;
		Output = Vector2.Zero;
		_handlePosition = _center;
		QueueRedraw();
	}

	public override void _Draw()
	{
		// Draw base
		DrawCircle(_center, Radius, BaseColor);
		
		// Draw handle
		DrawCircle(_handlePosition, Radius * 0.4f, HandleColor);
	}
}
