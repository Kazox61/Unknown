using System.Collections.Generic;
using System.Net;
using Godot;
using GodotTask;
using Unknown.Utilities;

namespace Unknown.Game.Protocol;

public class VisionUpdateMessage : ProtocolMessage {
	public override MessageId MessageId => MessageId.VisionUpdate;

	private List<ActorServer> _actorServers;
	private List<DataActorServer> _dataActorServers = new();

	public VisionUpdateMessage(List<ActorServer> actorServers) {
		_actorServers = actorServers;
	}

	public VisionUpdateMessage(IPEndPoint ipEndPoint = null, Reader reader = null) : base(ipEndPoint, reader) { }

	public override async GDTask Encode() {
		await Stream.WriteInt(_actorServers.Count);

		foreach (var actorServer in _actorServers) {
			await Stream.WriteInt(actorServer.MatchPlayerId);
			await Stream.WriteSingle(actorServer.GlobalPosition.X);
			await Stream.WriteSingle(actorServer.GlobalPosition.Z);
		}
	}

	public override void Decode() {
		var count = Reader.ReadInt32();

		for (var i = 0; i < count; i++) {
			_dataActorServers.Add(new DataActorServer {
				MatchPlayerId = Reader.ReadInt32(),
				Position = new Vector2(Reader.ReadSingle(), Reader.ReadSingle())
			});
		}
	}

	public override GDTask Process() {
		foreach (var dataActorServer in _dataActorServers) {
			var actor = ControllerClient.Instance.GetActor(dataActorServer.MatchPlayerId)
			            ?? ActorClient.Create(dataActorServer.MatchPlayerId, true);

			actor.StartLerpPosition(new Vector3(dataActorServer.Position.X, 1, dataActorServer.Position.Y));
		}

		return GDTask.CompletedTask;
	}

	private struct DataActorServer {
		public int MatchPlayerId;
		public Vector2 Position;
	}
}