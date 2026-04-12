using Game.Manager;
using Godot;


namespace Game;

public partial class Main : Node
{

	//REFRENCES
	private GridManager gridManager;
	private Sprite2D cursor;
	private PackedScene buildingscene;
	private Button placeBuildingButton;
	private Vector2I? hoveredGridCell;
	


//READY
	public override void _Ready()
	{
		buildingscene = GD.Load<PackedScene>("res://Scenes/Building/Building.tscn");
		cursor = GetNode<Sprite2D>("Cursor");
		gridManager = GetNode<GridManager>("GridManager");
		placeBuildingButton = GetNode<Button>("PlaceBuildingButton");

		placeBuildingButton.Pressed += OnButtonPressed;

		cursor.Visible = false;

	}
//CLICK CLICK
	public override void _UnhandledInput(InputEvent evt)
	{
		if (hoveredGridCell.HasValue && evt.IsActionPressed("left_click") && gridManager.IsTilePositionValid(hoveredGridCell.Value))
		{
			PlaceBuildingAtHoveredCellPosition();
			cursor.Visible = false;
		}
	}
//PROCESS
	public override void _Process(double delta)
	{
		var gridPosition = gridManager.GetMouseGridVectorPosition();
		cursor.GlobalPosition = gridPosition * 64;
		if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition))
		{
			hoveredGridCell = gridPosition;
			gridManager.HighlightBuildableTiles();
		}
	}

	private void PlaceBuildingAtHoveredCellPosition()
	{
		if (!hoveredGridCell.HasValue) return;

		var building = buildingscene.Instantiate<Node2D>();
		AddChild(building);

		building.GlobalPosition = hoveredGridCell.Value * 64;
		gridManager.MarkTilesAsOccupied(hoveredGridCell.Value);

		hoveredGridCell = null;
		gridManager.ClearHighlightedTiles();
	}


	private void OnButtonPressed()
	{
		cursor.Visible = true;
	}
}
