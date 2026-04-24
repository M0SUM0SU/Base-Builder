using Game.Resources.Building;
using Godot;

namespace Game.UI;

public partial class GameUI : MarginContainer
{
	[Signal]
	public delegate void BuildingResourceSelectedEventHandler(BuildingResource buildingResource);

	[Export]
	private BuildingResource[] buildingResources;

	private HBoxContainer hboxcontainer;

	public override void _Ready()
	{
		hboxcontainer = GetNode<HBoxContainer>("HBoxContainer");
		CreateBuildingButtons();
	}

	private void CreateBuildingButtons()
	{
		foreach( var buildingresource in buildingResources)
		{
			var buildingButton = new Button();
			buildingButton.Text = $"Place {buildingresource.DisplayName}";
			hboxcontainer.AddChild(buildingButton);

			buildingButton.Pressed += () =>
			{
				EmitSignal(SignalName.BuildingResourceSelected, buildingresource);	
			};
		}
	}

}
