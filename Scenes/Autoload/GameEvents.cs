using Game.Component;
using Godot;

namespace Game.Autoload;

public partial class GameEvents : Node
{

	public static GameEvents Instance { get; private set; }

	[Signal]
	public delegate void BuildingPlacedEventHandler(BuildingComponent buildingcomponent);

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
		{
			Instance = this;
		}
    }

	public static void EmitBuildingPlaced(BuildingComponent buildingcomponent)
	{
		Instance.EmitSignal(SignalName.BuildingPlaced, buildingcomponent);
	}
}
