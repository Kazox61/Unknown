using System.Net;
using Godot;
using GodotTask;
using Unknown.Utilities;

namespace Unknown.Game.Protocol; 

public class PlayerInputMessage : ProtocolMessage {
	public override MessageId MessageId => MessageId.PlayerInput;

	public Vector2 MoveInput;
	public Vector2 AimInput;

	public PlayerInputMessage(Vector2 moveInput, Vector2 aimInput) {
		MoveInput = moveInput;
		AimInput = aimInput;
	}
	
	public PlayerInputMessage(IPEndPoint ipEndPoint = null, Reader reader = null) : base(ipEndPoint, reader) { }

	public override async GDTask Encode() {
		await Stream.WriteSingle(MoveInput.X);
		await Stream.WriteSingle(MoveInput.Y);
		
		await Stream.WriteSingle(AimInput.X);
		await Stream.WriteSingle(AimInput.Y);
	}

	public override void Decode() {
		MoveInput = new Vector2(Reader.ReadSingle(), Reader.ReadSingle());
		AimInput = new Vector2(Reader.ReadSingle(), Reader.ReadSingle());
	}

	public override async GDTask Process() {
		var actor = ControllerVisionUpdate.Instance.GetActor(IpEndPoint);

		if (actor == null) {
			return;
		}
		
		actor.Input = MoveInput;
	}
}