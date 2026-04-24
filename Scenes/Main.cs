using Game.Manager;
using Game.Resources.Building;
using Game.UI;
using Godot;


namespace Game;

public partial class Main : Node
{
	//REFRENCES
	private GridManager gridManager;
	private Sprite2D cursor;
	
	private Vector2I? hoveredGridCell;
	private Node2D ySortRoot;
	private GameUI gameUI;
	
	private BuildingResource toPlaceBuidingResource;
	
//READY
	public override void _Ready()
	{
		cursor = GetNode<Sprite2D>("Cursor");
		gridManager = GetNode<GridManager>("GridManager");
		ySortRoot = GetNode<Node2D>("YSortRoot");
		gameUI = GetNode<GameUI>("GameUI");

		cursor.Visible = false;

		gridManager.ResourceTilesUpdated += OnResouceTilesUpdated;
		gameUI.BuildingResourceSelected += OnBuildingResourceSelected;

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


	private void OnBuildingResourceSelected(BuildingResource buildingResource)
	{
		toPlaceBuidingResource = buildingResource;
		cursor.Visible = true;
		gridManager.HighlightBuildableTiles();
	}

	private void OnResouceTilesUpdated(int resourceCount)
	{
		GD.Print(resourceCount);
	}
}
