using System.Collections.Generic;
using System.Linq;
using System.Net;
using Godot;
using GodotTask;
using Unknown.Game.Protocol;

namespace Unknown.Game; 

public partial class ControllerVisionUpdate : Node {
	public const float VisionUpdateTickDuration = 0.05f;

	public static ControllerVisionUpdate Instance;
	
	[Export] public ServerMessageManager ServerMessageManager;
	[Export] public PackedScene PrefabActorServer;

	private int _nextMatchPlayerId;
	private double _time;
	private readonly List<ActorServer> _actorServers = new();

	public override void _Ready() {
		Instance = this;
	}

	public override void _Process(double delta) {
		_time += delta;
		if (_time < VisionUpdateTickDuration) {
			return;
		}

		OnTick();
		_time -= VisionUpdateTickDuration;
	}

	private void OnTick() {
		var message = new VisionUpdateMessage(_actorServers);
		
		ServerMessageManager.BroadcastMessage(message).Forget();
	}

	public ActorServer AddActor(ActorServer actor) {
		_actorServers.Add(actor);
		
		actor.MatchPlayerId = _nextMatchPlayerId;
		_nextMatchPlayerId++;

		return actor;
	}

	public ActorServer GetActor(IPEndPoint ipEndPoint) {
		return _actorServers.FirstOrDefault(actor => Equals(actor.IpEndPoint, ipEndPoint));
	}
}