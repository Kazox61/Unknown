using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotTask;
using Unknown.Game.Protocol;
using Unknown.UI;

namespace Unknown.Game; 

public partial class ControllerClient : Node {
	public static ControllerClient Instance;
	
	[Export] public ClientMessageManager ClientMessageManager;
	[Export] public Joystick JoystickMove;
	[Export] public PackedScene PrefabActorClient;
	[Export] public PackedScene PrefabActorClientEnemy;

	
	private readonly List<ActorClient> _actorClients = new();
	
	public int Tick;

	public override void _Ready() {
		Instance = this;
	}

	public override void _Process(double delta) {
		ClientMessageManager.SendMessage(new PlayerInputMessage(
			JoystickMove.Value, Vector2.Zero
		)).Forget();
		//ClientMessageManager.SendInput(Tick, JoystickMove.Value).Forget();
		//GD.Print($"[Client] {Tick} " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
		Tick += 1;
	}
	
	public ActorClient AddActor(ActorClient actor ) {
		_actorClients.Add(actor);
		return actor;
	}

	public ActorClient GetActor(int matchPlayerId) {
		return _actorClients.FirstOrDefault(actor => actor.MatchPlayerId == matchPlayerId);
	}
}