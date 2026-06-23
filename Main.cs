using Godot;

public partial class Main : Node2D
{
	private World world;
	private StatsPanel statsPanel;

	public override void _Ready()
	{
		world = GetNode<World>("World");
		statsPanel = GetNode<StatsPanel>("StatsPanel");

		bindGenerateButton("ControllerUI/TopUI/TopUIMargin/TopUIContent/CreateMapControls/MapSizeButtons/GenerateSmallButton", 32);
		bindGenerateButton("ControllerUI/TopUI/TopUIMargin/TopUIContent/CreateMapControls/MapSizeButtons/GenerateNormalButton", 64);
		bindGenerateButton("ControllerUI/TopUI/TopUIMargin/TopUIContent/CreateMapControls/MapSizeButtons/GenerateLargeButton", 128);

		GetNode<Button>("ControllerUI/TopUI/TopUIMargin/TopUIContent/ClearMapButton").Pressed += clearMap;
		GetNode<Button>("ControllerUI/TopUI/TopUIMargin/TopUIContent/ShowStatisticsButton").Pressed += showStatistics;
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
}
