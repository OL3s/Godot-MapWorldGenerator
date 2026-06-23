using Godot;

public partial class Main : Node2D
{
	private World world;

	public override void _Ready()
	{
		world = GetNode<World>("World");

		bindGenerateButton("ControllerUI/TopUI/MapSizeButtons/Generate32Button", 32);
		bindGenerateButton("ControllerUI/TopUI/MapSizeButtons/Generate64Button", 64);
		bindGenerateButton("ControllerUI/TopUI/MapSizeButtons/Generate128Button", 128);

		GetNode<Button>("ControllerUI/TopUI/ClearMapButton").Pressed += () => world.ClearMap();
	}

	private void bindGenerateButton(string path, int size)
	{
		GetNode<Button>(path).Pressed += () => world.GenerateMap(size, size);
	}
}
