using Godot;
using Game.UI;
using Game.Resources.Building;

namespace Game.Manager;

public partial class BuildingManager : Node
{
	[Export]
	private GridManager gridManager;
	[Export]
	private GameUI gameUI;
	[Export]
	private Node2D ySortRoot;
	[Export]
	private PackedScene buildingGhostScene;


	private Vector2I? hoveredGridCell;
	private BuildingResource toPlaceBuidingResource;
	private int currentlyUsedResourceCount;
	private int currentResourceCount;
	private int startingReosurceCount = 4;
	private Node2D buildingGhost;

	private int avaiableResourceCount => startingReosurceCount + currentResourceCount - currentlyUsedResourceCount;

	public override void _Ready()
	{
		gridManager.ResourceTilesUpdated += OnresourceTilesUpdated;
		gameUI.BuildingResourceSelected += OnBuildingResourceSelected;
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (
			hoveredGridCell.HasValue && 
			toPlaceBuidingResource != null &&
			evt.IsActionPressed("left_click") && 
			gridManager.IsTilePositionBuildable(hoveredGridCell.Value) && 
			avaiableResourceCount >= toPlaceBuidingResource.ResourceCost
			)
		{
			PlaceBuildingAtHoveredCellPosition();
		}
	}

	public override void _Process(double delta)
	{
		if (!IsInstanceValid(buildingGhost)) return;


		var gridPosition = gridManager.GetMouseGridVectorPosition();
		buildingGhost.GlobalPosition = gridPosition * 64;
		if (toPlaceBuidingResource != null && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition))
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

		currentlyUsedResourceCount += toPlaceBuidingResource.ResourceCost;
		buildingGhost.QueueFree();
		buildingGhost = null;
	}

	private void OnresourceTilesUpdated(int resourceCount)
	{
		currentResourceCount += resourceCount;
	}

	private void OnBuildingResourceSelected(BuildingResource buildingResource)
	{
		if (IsInstanceValid(buildingGhost))
		{
			buildingGhost.QueueFree();
		}

		buildingGhost = buildingGhostScene.Instantiate<Node2D>();
		ySortRoot.AddChild(buildingGhost);

		var buildingSprite = buildingResource.SpriteScene.Instantiate<Sprite2D>();
		buildingGhost.AddChild(buildingSprite);

		toPlaceBuidingResource = buildingResource;
		gridManager.HighlightBuildableTiles();
	}
}
