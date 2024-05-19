using System.Net;
using Godot;
using GodotTask;
using Unknown.Utilities;

namespace Unknown.Game.Protocol; 

public class PlayerInputMessage : ProtocolMessage {
	public override MessageId MessageId => MessageId.PlayerInput;

	public Vector2 MoveInput;
	public bool Shoot;
	public Vector2 ShootDirection;

	public PlayerInputMessage(Vector2 moveInput, bool shoot, Vector2 shootDirection) {
		MoveInput = moveInput;
		Shoot = shoot;
		ShootDirection = shootDirection;
	}
	
	public PlayerInputMessage(IPEndPoint ipEndPoint = null, Reader reader = null) : base(ipEndPoint, reader) { }

	public override async GDTask Encode() {
		await Stream.WriteSingle(MoveInput.X);
		await Stream.WriteSingle(MoveInput.Y);

		Stream.WriteBool(Shoot);
		
		await Stream.WriteSingle(ShootDirection.X);
		await Stream.WriteSingle(ShootDirection.Y);
	}

	public override void Decode() {
		MoveInput = new Vector2(Reader.ReadSingle(), Reader.ReadSingle());
		Shoot = Reader.ReadBoolean();
		ShootDirection = new Vector2(Reader.ReadSingle(), Reader.ReadSingle());
	}

	public override async GDTask Process() {
		var actor = ControllerVisionUpdate.Instance.GetActor(IpEndPoint);

		if (actor == null) {
			return;
		}
		
		actor.Input = MoveInput;

		if (!Shoot) {
			return;
		}
		
		GD.Print("Shoot");

		var projectile = actor.PrefabProjectileLinear.Instantiate<ProjectileLinear>();
		SpawnerSceneTree.Instance.Spawn(projectile);
		projectile.Initialize(actor.GlobalPosition + new Vector3(0, 0.5f, 0), new Vector3(ShootDirection.X, 0f, ShootDirection.Y), 5);
	}
}