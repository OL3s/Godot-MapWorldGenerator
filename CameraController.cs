using Godot;

public partial class CameraController : Camera2D
{
	[Export] public float MoveSpeed = 512.0f;
	[Export] public float ZoomStep = 1.1f;
	[Export] public float MinZoom = 0.25f;
	[Export] public float MaxZoom = 4.0f;

	private Vector2 startPosition;
	private Vector2 startZoom;

	public override void _Ready()
	{
		startPosition = Position;
		startZoom = Zoom;
	}

	public override void _Process(double delta)
	{
		Vector2 direction = Vector2.Zero;

		if(Input.IsActionPressed("ui_left") || Input.IsActionPressed("camera_left"))
			direction.X -= 1;
		if(Input.IsActionPressed("ui_right") || Input.IsActionPressed("camera_right"))
			direction.X += 1;
		if(Input.IsActionPressed("ui_up") || Input.IsActionPressed("camera_up"))
			direction.Y -= 1;
		if(Input.IsActionPressed("ui_down") || Input.IsActionPressed("camera_down"))
			direction.Y += 1;

		if(direction != Vector2.Zero)
			Position += direction.Normalized() * MoveSpeed * (float)delta / Zoom.X;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event is not InputEventMouseButton mouseButton || !mouseButton.Pressed)
			return;

		if(mouseButton.ButtonIndex == MouseButton.WheelUp)
			setZoom(Zoom.X * ZoomStep);
		else if(mouseButton.ButtonIndex == MouseButton.WheelDown)
			setZoom(Zoom.X / ZoomStep);
	}

	private void setZoom(float zoom)
	{
		float clampedZoom = Mathf.Clamp(zoom, MinZoom, MaxZoom);
		Zoom = new Vector2(clampedZoom, clampedZoom);
	}

	public void ResetCamera()
	{
		Position = startPosition;
		Zoom = startZoom;
	}
}
