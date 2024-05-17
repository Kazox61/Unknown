using System.Threading;
using Godot;
using GodotTask;

namespace Unknown.Game;

public partial class ActorClient : Node3D {
	[Export] public AttackIndicator AttackIndicator;
	
	public int MatchPlayerId;

	private CancellationTokenSource _cancellationTokenSource = new();

	public static ActorClient Create(int matchPlayerId, bool isEnemy) {
		var actor = isEnemy
			? ControllerClient.Instance.PrefabActorClientEnemy.Instantiate<ActorClient>()
			: ControllerClient.Instance.PrefabActorClient.Instantiate<ActorClient>();
		SpawnerSceneTree.Instance.Spawn(actor);

		actor.MatchPlayerId = matchPlayerId;

		return ControllerClient.Instance.AddActor(actor);
	}

	public void StartLerpPosition(Vector3 endPosition) {
		_cancellationTokenSource.Cancel();
		_cancellationTokenSource = new CancellationTokenSource();

		LerpPosition(endPosition, _cancellationTokenSource.Token).Forget();
	}

	private async GDTask LerpPosition(Vector3 endPosition, CancellationToken cancellationToken) {
		var startPosition = GlobalPosition;
		var time = 0f;
		while (time <= ControllerVisionUpdate.VisionUpdateTickDuration) {
			if (cancellationToken.IsCancellationRequested) {
				return;
			}

			GlobalPosition = startPosition.Lerp(endPosition, time / ControllerVisionUpdate.VisionUpdateTickDuration);
			time += (float)GetProcessDeltaTime();

			await GDTask.Yield(PlayerLoopTiming.Process);
		}
	}
}