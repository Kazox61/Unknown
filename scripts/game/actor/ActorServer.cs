using System.Net;
using Godot;

namespace Unknown.Game; 

public partial class ActorServer : CharacterBody3D {
	private const float Speed = 64f;
	private const float Friction = 10f;

	[Export] public PackedScene PrefabProjectileLinear;
	
	public int MatchPlayerId;
	public IPEndPoint IpEndPoint;
	public Vector2 Input = Vector2.Zero;
	public int Tick;
	
	public static ActorServer Create(IPEndPoint ipEndPoint) {
		var actor = ControllerVisionUpdate.Instance.PrefabActorServer.Instantiate<ActorServer>();
		SpawnerSceneTree.Instance.Spawn(actor);

		actor.IpEndPoint = ipEndPoint;
		
		return ControllerVisionUpdate.Instance.AddActor(actor);
	}
	
	public override void _Process(double delta) { 
		var velocity = Velocity;
		var targetVelocity = Input * Speed;
		var velocityChange = new Vector3 {
			X = (targetVelocity.X - velocity.X) * Friction * (float)delta,
			Y = velocity.Y,
			Z = (targetVelocity.Y - velocity.Y) * Friction * (float)delta
		};
		
		Velocity = velocityChange;
		MoveAndSlide();
	}
}