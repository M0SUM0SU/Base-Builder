using Godot;
using Game.UI;
using Game.Resources.Building;
using Game.Building;

namespace Game.Manager;

public partial class BuildingManager : Node
{
	private readonly StringName ACTION_LEFT_CLICK = "left_click";
	private readonly StringName ACTION_CANCEL = "Cancel";
	private readonly StringName ACTION_RIGHT_CLICK = "right_click";

	[Export]
	private GridManager gridManager;
	[Export]
	private GameUI gameUI;
	[Export]
	private Node2D ySortRoot;
	[Export]
	private PackedScene buildingGhostScene;

	private enum State
	{
		Normal,
		PlacingBuilding
	}

	private Vector2I hoveredGridCell;
	private BuildingResource toPlaceBuidingResource;
	private int currentlyUsedResourceCount;
	private int currentResourceCount;
	private int startingReosurceCount = 4;
	private BuildingGhost buildingGhost;
	private State currentState;


	private int avaiableResourceCount => startingReosurceCount + currentResourceCount - currentlyUsedResourceCount;

	public override void _Ready()
	{
		gridManager.ResourceTilesUpdated += OnresourceTilesUpdated;
		gameUI.BuildingResourceSelected += OnBuildingResourceSelected;
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		switch (currentState)
		{
			case State.Normal:
				if (evt.IsActionPressed(ACTION_RIGHT_CLICK))
				{
					DestroyBuildingAtHoveredCellPosition();
				}
				break;

			case State.PlacingBuilding:
				if (evt.IsActionPressed(ACTION_CANCEL))
				{
					ChangeState(State.Normal);
				}
				else if (
					toPlaceBuidingResource != null &&
					evt.IsActionPressed(ACTION_LEFT_CLICK) &&
					IsBuildingPlacableAtTile(hoveredGridCell)
					)
				{
					PlaceBuildingAtHoveredCellPosition();
				}
				break;

			default:
				break;
		}
		
	}

	public override void _Process(double delta)
	{
		var gridPosition = gridManager.GetMouseGridVectorPosition();
		if (hoveredGridCell != gridPosition)
		{
			hoveredGridCell = gridPosition;
			UpdateHoveredGridCell();
		}

		switch(currentState)
		{
			case State.Normal:
				break;
			case State.PlacingBuilding:
				buildingGhost.GlobalPosition = gridPosition * 64;
				break;
		}

	}

	private void UpdateGridDisplay()
	{
		gridManager.ClearHighlightedTiles();
		gridManager.HighlightBuildableTiles();
		if (IsBuildingPlacableAtTile(hoveredGridCell))
		{
			gridManager.HighlightExpandedBuildableTiles(hoveredGridCell, toPlaceBuidingResource.BuildableRadius);
			gridManager.HighlightResourceTiles(hoveredGridCell, toPlaceBuidingResource.ResourceRadius);
			buildingGhost.SetValid();
		}
		else
		{
			buildingGhost.SetInvalid();
		}
	}

	private void DestroyBuildingAtHoveredCellPosition()
	{
		
	}

	private void PlaceBuildingAtHoveredCellPosition()
	{
		var building = toPlaceBuidingResource.BuildingScene.Instantiate<Node2D>();
		
		ySortRoot.AddChild(building);

		building.GlobalPosition = hoveredGridCell * 64;
		
		currentlyUsedResourceCount += toPlaceBuidingResource.ResourceCost;

		ChangeState(State.Normal);
	}

	private void ClearBuildingGhost()
	{
		gridManager.ClearHighlightedTiles();

		if (IsInstanceValid(buildingGhost))
		{	
			buildingGhost.QueueFree();
		}
		buildingGhost = null;
	}

	private bool IsBuildingPlacableAtTile(Vector2I tilePosition)
	{
		return gridManager.IsTilePositionBuildable(tilePosition) && avaiableResourceCount >= toPlaceBuidingResource.ResourceCost;
	}

	private void UpdateHoveredGridCell()
	{
		switch (currentState)
		{
			case State.Normal:
				break;
			case State.PlacingBuilding:
				UpdateGridDisplay();
				break;
			default:
				break;
		}
	}

	private void ChangeState(State toState)
	{
		switch (currentState)
		{
			case State.Normal:
				break;
			case State.PlacingBuilding:
				ClearBuildingGhost();
				toPlaceBuidingResource = null;
				break;
		}

		currentState = toState;

		switch (currentState)
		{
			case State.Normal:
				break;
			case State.PlacingBuilding:
				buildingGhost = buildingGhostScene.Instantiate<BuildingGhost>();
				ySortRoot.AddChild(buildingGhost);
				break;
		}
	}

	private void OnresourceTilesUpdated(int resourceCount)
	{
		currentResourceCount += resourceCount;
	}

	private void OnBuildingResourceSelected(BuildingResource buildingResource)
	{
		ChangeState(State.PlacingBuilding);
		var buildingSprite = buildingResource.SpriteScene.Instantiate<Sprite2D>();
		buildingGhost.AddChild(buildingSprite);
		toPlaceBuidingResource = buildingResource;
		UpdateGridDisplay();
	}
}
