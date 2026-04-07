using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;


namespace Game;

public partial class Main : Node
{

	//REFRENCES
	private Sprite2D cursor;
	private PackedScene buildingscene;
	private Button placeBuildingButton;
	private TileMapLayer highlightTilemapLayer;
	private Vector2? hoveredGridCell;
	private HashSet<Vector2> occupiedCells = new();


//READY
	public override void _Ready()
	{
		buildingscene = GD.Load<PackedScene>("res://Scenes/Building/Building.tscn");
		cursor = GetNode<Sprite2D>("Cursor");
		highlightTilemapLayer = GetNode<TileMapLayer>("HighlightTilemapLayer");
		placeBuildingButton = GetNode<Button>("PlaceBuildingButton");

		placeBuildingButton.Pressed += OnButtonPressed;

		cursor.Visible = false;

	}
//CLICK CLICK
	public override void _UnhandledInput(InputEvent evt)
	{
		if (hoveredGridCell.HasValue && evt.IsActionPressed("left_click") && !occupiedCells.Contains(hoveredGridCell.Value))
		{
			PlaceBuildingAtHoveredCellPosition();
			cursor.Visible = false;
		}
	}
//PROCESS
	public override void _Process(double delta)
	{
		var gridPosition = GetMouseGridVectorPosition();
		cursor.GlobalPosition = gridPosition * 64;
		if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition))
		{
			hoveredGridCell = gridPosition;
			UpdateHighlightTileLayer();
		}
	}

	private Vector2 GetMouseGridVectorPosition()
	{
		var mousePosition = highlightTilemapLayer.GetGlobalMousePosition();
		var gridPosition =  mousePosition / 64;
		gridPosition = gridPosition.Floor();
		return gridPosition;
	}


	private void PlaceBuildingAtHoveredCellPosition()
	{
		if (!hoveredGridCell.HasValue) return;

		var building = buildingscene.Instantiate<Node2D>();
		AddChild(building);

		building.GlobalPosition = hoveredGridCell.Value * 64;
		occupiedCells.Add(hoveredGridCell.Value);

		hoveredGridCell = null;
		UpdateHighlightTileLayer();
	}

	private void UpdateHighlightTileLayer()
	{
		highlightTilemapLayer.Clear();
		if (!hoveredGridCell.HasValue)
		{
			return;
		}

		for (var x = hoveredGridCell.Value.X - 3; x <= hoveredGridCell.Value.X + 3; x++)
		{
			for (var y = hoveredGridCell.Value.Y - 3; y <= hoveredGridCell.Value.Y + 3; y++)
			{
				highlightTilemapLayer.SetCell(new Vector2I((int)x,(int)y),0, Vector2I.Zero);
			}
		}
	}

	private void OnButtonPressed()
	{
		cursor.Visible = true;
	}
}
