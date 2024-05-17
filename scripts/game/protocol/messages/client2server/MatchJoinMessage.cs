using System.Net;
using Godot;
using GodotTask;
using Unknown.Utilities;

namespace Unknown.Game.Protocol;

public class MatchJoinMessage : ProtocolMessage {
	public override MessageId MessageId => MessageId.MatchJoin;

	public MatchJoinMessage(IPEndPoint ipEndPoint = null, Reader reader = null) : base(ipEndPoint, reader) { }

	public override async GDTask Process() {
		ServerMessageManager.Instance.AddClient(IpEndPoint);

		var actor = ActorServer.Create(IpEndPoint);

		var spawnPosition = ActorSpawnManager.Instance.GetSpawnPosition();

		actor.GlobalPosition = spawnPosition;

		GD.Print($"[Server] New Client connected -> MatchPlayerId: {actor.MatchPlayerId}, Position: {spawnPosition}");

		await ServerMessageManager.Instance.SendMessage(actor.IpEndPoint,
			new MatchJoinOkMessage(actor.MatchPlayerId, new Vector2(spawnPosition.X, spawnPosition.Z)));
	}
}