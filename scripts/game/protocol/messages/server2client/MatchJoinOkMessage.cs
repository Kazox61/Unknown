using System.Net;
using Godot;
using GodotTask;
using Unknown.Utilities;

namespace Unknown.Game.Protocol; 

public class MatchJoinOkMessage : ProtocolMessage {
	public override MessageId MessageId => MessageId.MatchJoinOk;

	public int MatchPlayerId;
	public Vector2 SpawnPosition;

	public MatchJoinOkMessage(int matchPlayerId, Vector2 spawnPosition) {
		MatchPlayerId = matchPlayerId;
		SpawnPosition = spawnPosition;
	}
	
	public MatchJoinOkMessage(IPEndPoint ipEndPoint = null, Reader reader = null) : base(ipEndPoint, reader) { }

	public override async GDTask Encode() {
		await Stream.WriteInt(MatchPlayerId);
		await Stream.WriteSingle(SpawnPosition.X);
		await Stream.WriteSingle(SpawnPosition.Y);
	}

	public override void Decode() {
		MatchPlayerId = Reader.ReadInt32();

		var x = Reader.ReadSingle();
		var y = Reader.ReadSingle();
		SpawnPosition = new Vector2(x, y);
	}

	public override GDTask Process() {
		var actor = ActorClient.Create(MatchPlayerId,false);
		actor.GlobalPosition = new Vector3(SpawnPosition.X, 0, SpawnPosition.Y);
		PlayerInputController.Instance.ActorClient = actor;

		GD.Print($"Match Joined with MatchPlayerId: {MatchPlayerId}");
		return GDTask.CompletedTask;
	}
}