using Godot;
using System.Collections.Generic;

public partial class StatsPanel : CanvasLayer
{
	private static readonly StyleBox SectionStyle = GD.Load<StyleBox>("res://assets/ui/styles/stats_section.tres");
	private VBoxContainer sections;
	private Label emptyLabel;
	private ScrollContainer scroll;

	public override void _Ready()
	{
		sections = GetNode<VBoxContainer>("Panel/PanelMargin/PanelLayout/Content/Scroll/Sections");
		emptyLabel = GetNode<Label>("Panel/PanelMargin/PanelLayout/Content/EmptyLabel");
		scroll = GetNode<ScrollContainer>("Panel/PanelMargin/PanelLayout/Content/Scroll");
		GetNode<Button>("Panel/PanelMargin/PanelLayout/HeaderBar/HeaderMargin/Header/CloseButton").Pressed += Hide;
		Hide();
	}

	public void ShowStats(GenerationStats stats)
	{
		ClearSections();
		emptyLabel.Visible = stats == null;
		scroll.Visible = stats != null;

		if(stats != null)
		{
			AddDetailsSection(stats);
			AddSummarySection("Terrain", stats.HeightStats);
			AddSummarySection("Resources", stats.ResourceStats);
			AddNoiseSection(stats);
		}

		Show();
	}

	private void ClearSections()
	{
		foreach(Node child in sections.GetChildren())
		{
			sections.RemoveChild(child);
			child.QueueFree();
		}
	}

	private void AddDetailsSection(GenerationStats stats)
	{
		VBoxContainer section = CreateSection("Map Details");
		GridContainer details = new GridContainer();
		details.Columns = 2;
		details.AddThemeConstantOverride("h_separation", 24);
		details.AddThemeConstantOverride("v_separation", 6);
		details.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

		AddDetail(details, "Size", stats.Width + " x " + stats.Height);
		AddDetail(details, "Total tiles", (stats.Width * stats.Height).ToString());
		AddDetail(details, "Chunk size", stats.ChunkSize + " x " + stats.ChunkSize);
		AddDetail(details, "Noise chunks", stats.NoiseChunkAverages.Count.ToString());

		section.AddChild(details);
		sections.AddChild(section.GetParent().GetParent());
	}

	private void AddSummarySection(string title, List<GenerationStatEntry> entries)
	{
		VBoxContainer section = CreateSection(title);
		foreach(GenerationStatEntry entry in entries)
			section.AddChild(CreateStatRow(entry));

		sections.AddChild(section.GetParent().GetParent());
	}

	private void AddNoiseSection(GenerationStats stats)
	{
		VBoxContainer section = CreateSection("Height Noise Overview");
		GridContainer grid = new GridContainer();
		grid.Columns = Mathf.Max(1, stats.NoiseColumns);
		grid.AddThemeConstantOverride("h_separation", 3);
		grid.AddThemeConstantOverride("v_separation", 3);

		foreach(float value in stats.NoiseChunkAverages)
		{
			ColorRect cell = new ColorRect();
			cell.CustomMinimumSize = new Vector2(16, 16);
			float intensity = Mathf.Clamp(value, 0.0f, 1.0f);
			cell.Color = new Color(0.08f + intensity * 0.50f, 0.18f + intensity * 0.55f, 0.28f + intensity * 0.45f, 1.0f);
			grid.AddChild(cell);
		}

		section.AddChild(grid);
		sections.AddChild(section.GetParent().GetParent());
	}

	private VBoxContainer CreateSection(string title)
	{
		PanelContainer panel = new PanelContainer();
		panel.AddThemeStyleboxOverride("panel", SectionStyle);

		MarginContainer margin = new MarginContainer();
		margin.AddThemeConstantOverride("margin_left", 12);
		margin.AddThemeConstantOverride("margin_top", 10);
		margin.AddThemeConstantOverride("margin_right", 12);
		margin.AddThemeConstantOverride("margin_bottom", 12);
		panel.AddChild(margin);

		VBoxContainer layout = new VBoxContainer();
		layout.AddThemeConstantOverride("separation", 8);
		margin.AddChild(layout);

		Label label = new Label();
		label.Text = title;
		label.AddThemeColorOverride("font_color", new Color(0.90f, 0.95f, 1.0f));
		label.AddThemeFontSizeOverride("font_size", 17);
		layout.AddChild(label);

		return layout;
	}

	private Control CreateStatRow(GenerationStatEntry entry)
	{
		VBoxContainer row = new VBoxContainer();
		row.AddThemeConstantOverride("separation", 3);

		HBoxContainer labels = new HBoxContainer();
		Label name = new Label();
		name.Text = entry.Name;
		name.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		Label value = new Label();
		value.Text = entry.Count + " tiles  " + entry.Percent.ToString("0.0") + "%";
		value.HorizontalAlignment = HorizontalAlignment.Right;
		labels.AddChild(name);
		labels.AddChild(value);

		ProgressBar bar = new ProgressBar();
		bar.MinValue = 0.0;
		bar.MaxValue = 100.0;
		bar.Value = entry.Percent;
		bar.ShowPercentage = false;
		bar.CustomMinimumSize = new Vector2(0, 8);

		row.AddChild(labels);
		row.AddChild(bar);
		return row;
	}

	private void AddDetail(GridContainer details, string label, string value)
	{
		Label labelNode = new Label();
		labelNode.Text = label;
		labelNode.AddThemeColorOverride("font_color", new Color(0.62f, 0.70f, 0.82f));

		Label valueNode = new Label();
		valueNode.Text = value;
		valueNode.HorizontalAlignment = HorizontalAlignment.Right;
		valueNode.AddThemeColorOverride("font_color", new Color(0.92f, 0.96f, 1.0f));

		details.AddChild(labelNode);
		details.AddChild(valueNode);
	}

}
