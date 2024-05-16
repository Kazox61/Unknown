using System;
using System.Threading.Tasks;
using Godot;
using GodotTask;
using Unknown.UI;

namespace Unknown.Game; 

public partial class ControllerClient : Node {
	[Export] public ClientMessageManager ClientMessageManager;
	[Export] public Joystick JoystickMove;
	[Export] public PackedScene PrefabActorClient;

	private ActorClient _actorClient;
	
	public int Tick;
	
	public override void _EnterTree() {
		ClientMessageManager.OnMessage += OnMessage;
	}

	public override void _ExitTree() {
		ClientMessageManager.OnMessage -= OnMessage;
	}

	public override void _Ready() {
		//Test().Forget();
	}

	private void OnMessage(int tick, float x, float y) {
		if (_actorClient == null) {
			_actorClient = PrefabActorClient.Instantiate<ActorClient>();
			AddChild(_actorClient);
		}

		GD.Print($"[Client] {tick} received " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
		_actorClient.StartLerpPosition(new Vector3(x, 1, y));
	}
	public override void _Process(double delta) {
		ClientMessageManager.SendInput(Tick, JoystickMove.Value).Forget();
		GD.Print($"[Client] {Tick} " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
		Tick += 1;
	}

	public async GDTask Test() {
		GD.Print("Start Test");
		await GDTask.Delay(TimeSpan.FromSeconds(10));
		GD.Print("[Client] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
		ClientMessageManager.SendInput(Tick, new Vector2(0.69f, 0.15f)).Forget();
	}
}