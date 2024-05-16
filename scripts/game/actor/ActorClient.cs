using System.Threading;
using Godot;
using GodotTask;

namespace Unknown.Game; 

public partial class ActorClient : Node3D {
	private CancellationTokenSource _cancellationTokenSource = new();

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