using Game.Autoload;
using Game.Resources.Building;
using Godot;

namespace Game.Component;

public partial class BuildingComponent : Node2D
{
	[Export(PropertyHint.File, "*.tres")]
	public string buildingResourcePath;

	public BuildingResource buildingResource {get; private set;}

	// READY
	public override void _Ready()
	{

		if(buildingResourcePath != null)
		{
			buildingResource = GD.Load<BuildingResource>(buildingResourcePath);
		}
		AddToGroup(nameof(BuildingComponent));
		Callable.From(() => GameEvents.EmitBuildingPlaced(this)).CallDeferred();
	}

	public Vector2I GetGridCellPosition()
	{
		var gridPosition =  GlobalPosition / 64;
		gridPosition = gridPosition.Floor();
		return new Vector2I((int)gridPosition.X, (int)gridPosition.Y);
	}

	public void Destroy()
	{
		GameEvents.EmitBuildingDestroyed(this);
		Owner.QueueFree();
	}
}
