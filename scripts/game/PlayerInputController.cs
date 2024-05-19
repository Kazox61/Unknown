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

	private bool _shoot;
	private Vector2 _shootDirection;

	public ActorClient ActorClient;

	public override void _Ready() {
		Instance = this;

		JoystickAim.OnJoystickReleased += direction => {
			_shoot = true;
			GD.Print("Shoot from Release");
			_shootDirection = direction.Normalized();
		};

		JoystickAim.OnClick += () => {
			_shoot = true;
			GD.Print("Shoot from Click");
			_shootDirection = Vector2.Zero;
		};
	}

	public override void _Process(double delta) {
		ClientMessageManager.SendMessage(new PlayerInputMessage(
			JoystickMove.Value, _shoot, _shootDirection
		)).Forget();
		
		if (_shoot) {
			_shoot = false;
		}

		if (JoystickAim.Value == Vector2.Zero) {
			ActorClient?.AttackIndicator.Hide();
			return;
		}
		
		ActorClient?.AttackIndicator.Show();

		ActorClient?.AttackIndicator.Rotate(JoystickAim.Value);
	}
}