using Godot;

namespace Game.Manager;

public partial class GridManager : Node
{
	[Export]
	private TileMapLayer highlightTilemapLayer;
	[Export]
	private TileMapLayer baseTerrainTilemapLayer;



	// READY
	public override void _Ready()
	{
		
	}


}
