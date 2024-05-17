using Godot;
using GodotTask;
using Unknown.Game.Protocol;
using Unknown.UI;

namespace Unknown.Game; 

public partial class PlayerInputController : Node {
	public static PlayerInputController Instance;
	
	[Export] public Joystick JoystickMove;
	[Export] public Joystick JoystickAim;
	[Export] public ClientMessageManager ClientMessageManager;

	public ActorClient ActorClient;

	public override void _Ready() {
		Instance = this;
	}

	public override void _Process(double delta) {
		ClientMessageManager.SendMessage(new PlayerInputMessage(
			JoystickMove.Value, JoystickAim.Value
		)).Forget();

		if (JoystickAim.Value == Vector2.Zero) {
			return;
		}

		ActorClient?.AttackIndicator.Rotate(JoystickAim.Value);
	}
}