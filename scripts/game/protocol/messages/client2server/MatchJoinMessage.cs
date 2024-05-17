using System.Net;
using Godot;
using GodotTask;
using Unknown.Utilities;

namespace Unknown.Game.Protocol; 

public class MatchJoinMessage : ProtocolMessage {
	public override MessageId MessageId => MessageId.MatchJoin;

	public MatchJoinMessage(IPEndPoint ipEndPoint = null, Reader reader = null) : base(ipEndPoint, reader) {
		GD.Print("MatchJoinMessage");
	}

	public override async GDTask Process() {
		ServerMessageManager.Instance.AddClient(IpEndPoint);

		var actor = ActorServer.Create(IpEndPoint);

		var spawnPosition = actor.MatchPlayerId == 0 ? new Vector2(30f, 0f) : new Vector2(-30f, 0f);
		
		actor.Position = new Vector3(spawnPosition.X, 1, spawnPosition.Y);
		
		GD.Print($"[Server] New Client connected -> MatchPlayerId: {actor.MatchPlayerId}, Position: {spawnPosition}");

		await ServerMessageManager.Instance.SendMessage(actor.IpEndPoint, new MatchJoinOkMessage(actor.MatchPlayerId, spawnPosition));
	}
}