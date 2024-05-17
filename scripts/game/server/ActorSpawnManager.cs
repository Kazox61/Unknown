using Godot;

namespace Unknown.Game; 

public partial class ActorSpawnManager : Node {
	public static ActorSpawnManager Instance;
	
	[Export] public Godot.Collections.Array<ActorSpawner> ActorSpawners;

	private int _currentIndex;

	public override void _Ready() {
		Instance = this;
	}

	public Vector3 GetSpawnPosition() {
		var position =  ActorSpawners[_currentIndex].GlobalPosition;

		_currentIndex++;

		return position;
	}
}