using Godot;

public partial class Main : Node2D
{
	private World world;
	private CameraController cameraController;
	private StatsPanel statsPanel;
	private Control topUI;

	public override void _Ready()
	{
		world = GetNode<World>("World");
		cameraController = GetNode<CameraController>("Camera2D");
		statsPanel = GetNode<StatsPanel>("StatsPanel");
		topUI = GetNode<Control>("ControllerUI/TopUI");

		bindGenerateButton("ControllerUI/TopUI/TopUIMargin/TopUIContent/CreateMapControls/MapSizeButtons/GenerateSmallButton", 32);
		bindGenerateButton("ControllerUI/TopUI/TopUIMargin/TopUIContent/CreateMapControls/MapSizeButtons/GenerateNormalButton", 64);
		bindGenerateButton("ControllerUI/TopUI/TopUIMargin/TopUIContent/CreateMapControls/MapSizeButtons/GenerateLargeButton", 128);

		GetNode<Button>("ControllerUI/TopUI/TopUIMargin/TopUIContent/ClearMapButton").Pressed += clearMap;
		GetNode<Button>("ControllerUI/TopUI/TopUIMargin/TopUIContent/ResetCameraButton").Pressed += cameraController.ResetCamera;
		GetNode<Button>("ControllerUI/TopUI/TopUIMargin/TopUIContent/ShowStatisticsButton").Pressed += showStatistics;
		GetNode<Button>("ControllerUI/ToggleTopUIButton").Pressed += toggleTopUI;
	}

	private void bindGenerateButton(string path, int size)
	{
		GetNode<Button>(path).Pressed += () => generateMap(size);
	}

	private void generateMap(int size)
	{
		world.GenerateMap(size, size);
		if(statsPanel.Visible)
			showStatistics();
	}

	private void clearMap()
	{
		world.ClearMap();
		if(statsPanel.Visible)
			showStatistics();
	}

	private void showStatistics()
	{
		statsPanel.ShowStats(world.GetGenerationStats());
	}

	private void toggleTopUI()
	{
		topUI.Visible = !topUI.Visible;
	}
}
