using Game.Manager;
using Game.Resources.Building;
using Godot;


namespace Game;

public partial class Main : Node
{
	//REFRENCES
	private GridManager gridManager;
	private Sprite2D cursor;
	private Button placeTowerButton;
	private Button placeVillageButton;
	
	private Vector2I? hoveredGridCell;
	private Node2D ySortRoot;
	
	private BuildingResource towerResource;
	private BuildingResource toPlaceBuidingResource;
	private BuildingResource villageresource;
	
//READY
	public override void _Ready()
	{
		towerResource = GD.Load<BuildingResource>("res://Resources/Building/Tower.tres");
		villageresource = GD.Load<BuildingResource>("res://Resources/Building/Village.tres");
		cursor = GetNode<Sprite2D>("Cursor");
		gridManager = GetNode<GridManager>("GridManager");
		placeTowerButton = GetNode<Button>("PlaceTowerButton");
		placeVillageButton = GetNode<Button>("PlaceVillageButton");
		ySortRoot = GetNode<Node2D>("YSortRoot");

		cursor.Visible = false;

		placeTowerButton.Pressed += OnPlaceTowerButtonPressed;
		placeVillageButton.Pressed += OnPlaceVillageButtonPressed;
		gridManager.ResourceTilesUpdated += OnResouceTilesUpdated;


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
		if (toPlaceBuidingResource != null && cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition))
		{
			hoveredGridCell = gridPosition;
			gridManager.ClearHighlightedTiles();
			gridManager.HighlightExpandedBuildableTiles(hoveredGridCell.Value, toPlaceBuidingResource.BuildableRadius);
			gridManager.HighlightResourceTiles(hoveredGridCell.Value, toPlaceBuidingResource.ResourceRadius);
		}
	}

	private void PlaceBuildingAtHoveredCellPosition()
	{
		if (!hoveredGridCell.HasValue) return;

		var building = toPlaceBuidingResource.BuildingScene.Instantiate<Node2D>();
		
		ySortRoot.AddChild(building);

		building.GlobalPosition = hoveredGridCell.Value * 64;

		hoveredGridCell = null;
		gridManager.ClearHighlightedTiles();
	}


	private void OnPlaceTowerButtonPressed()
	{
		toPlaceBuidingResource = towerResource;
		cursor.Visible = true;
		gridManager.HighlightBuildableTiles();
	}

	private void OnPlaceVillageButtonPressed()
	{
		toPlaceBuidingResource = villageresource;
		cursor.Visible = true;
		gridManager.HighlightBuildableTiles();
	}

	private void OnResouceTilesUpdated(int resourceCount)
	{
		GD.Print(resourceCount);
	}
}
