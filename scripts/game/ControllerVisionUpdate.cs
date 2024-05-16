using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Godot;
using GodotTask;
using Unknown.Utilities;

namespace Unknown.Game; 

public partial class ControllerVisionUpdate : Node {
	public const float VisionUpdateTickDuration = 0.05f;
	
	
	[Export] public ServerMessageManager ServerMessageManager;
	[Export] public PackedScene PrefabActorServer;
	
	private double _time;
	private readonly List<ActorServer> _actorServers = new();
	
	public override void _Process(double delta) {
		_time += delta;
		if (_time < VisionUpdateTickDuration) {
			return;
		}

		OnTick();
		_time -= VisionUpdateTickDuration;
	}

	public override void _EnterTree() {
		ServerMessageManager.OnClientConnect += OnClientConnect;
		ServerMessageManager.OnInputMessage += OnInputMessage;
	}

	public override void _ExitTree() {
		ServerMessageManager.OnClientConnect -= OnClientConnect;
		ServerMessageManager.OnInputMessage -= OnInputMessage;
	}

	private void OnTick() {
		SendPositions().Forget();
	}

	private async GDTask SendPositions() {
		var stream = new MemoryStream();
		foreach (var actorServer in _actorServers) {
			await stream.WriteInt(actorServer.Tick);
			await stream.WriteSingle(actorServer.Position.X);
			await stream.WriteSingle(actorServer.Position.Z);
		}
		await ServerMessageManager.Broadcast(stream.GetBuffer());
	}

	private void OnClientConnect(IPEndPoint ipEndPoint) {
		var player = PrefabActorServer.Instantiate<ActorServer>();
		AddChild(player);
		player.Position = new Vector3(30, 1, 0);
		player.IpEndPoint = ipEndPoint;
		_actorServers.Add(player);
	}

	private void OnInputMessage(IPEndPoint ipEndPoint, int tick, float x, float y) {
		var actor = _actorServers.First(actor => Equals(actor.IpEndPoint, ipEndPoint));
		actor.Input = new Vector2(x, y);
		actor.Tick = tick;
	}
}