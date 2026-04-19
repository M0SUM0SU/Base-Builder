using Game.Manager;
using Godot;


namespace Game;

public partial class Main : Node
{

	//REFRENCES
	private GridManager gridManager;
	private Sprite2D cursor;
	private PackedScene towerScene;
	private PackedScene villagescene;
	private Button placeTowerButton;
	private Button placeVillageButton;
	private Vector2I? hoveredGridCell;
	private PackedScene toPlaceBuidingScene;
	private Node2D ySortRoot;
	


//READY
	public override void _Ready()
	{
		towerScene = GD.Load<PackedScene>("res://Scenes/Building/Tower.tscn");
		villagescene = GD.Load<PackedScene>("res://Scenes/Building/Village.tscn");
		cursor = GetNode<Sprite2D>("Cursor");
		gridManager = GetNode<GridManager>("GridManager");
		placeTowerButton = GetNode<Button>("PlaceTowerButton");
		placeVillageButton = GetNode<Button>("PlaceVillageButton");
		ySortRoot = GetNode<Node2D>("YSortRoot");

		placeTowerButton.Pressed += OnPlaceTowerButtonPressed;
		placeVillageButton.Pressed += OnPlaceVillageButtonPressed;

		cursor.Visible = false;

	}
//CLICK CLICK
	public override void _UnhandledInput(InputEvent evt)
	{
		if (hoveredGridCell.HasValue && evt.IsActionPressed("left_click") && gridManager.IsTilePositionBuildable(hoveredGridCell.Value))
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
			gridManager.HighlightExpandedBuildableTiles(hoveredGridCell.Value, 3);
		}
	}

	private void PlaceBuildingAtHoveredCellPosition()
	{
		if (!hoveredGridCell.HasValue) return;

		var building = toPlaceBuidingScene.Instantiate<Node2D>();
		
		ySortRoot.AddChild(building);

		building.GlobalPosition = hoveredGridCell.Value * 64;

		hoveredGridCell = null;
		gridManager.ClearHighlightedTiles();
	}


	private void OnPlaceTowerButtonPressed()
	{
		toPlaceBuidingScene = towerScene;
		cursor.Visible = true;
		gridManager.HighlightBuildableTiles();
	}

	private void OnPlaceVillageButtonPressed()
	{
		toPlaceBuidingScene = villagescene;
		cursor.Visible = true;
		gridManager.HighlightBuildableTiles();
	}

}
