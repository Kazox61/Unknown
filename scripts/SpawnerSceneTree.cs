using Godot;

namespace Unknown; 

public partial class SpawnerSceneTree : Node {
	public static SpawnerSceneTree Instance { get; private set; }
	
	public override void _EnterTree() {
		Instance = this;
	}
	
	public override void _ExitTree() {
		Instance = null;
	}

	public void Spawn(Node node) {
		GetTree().Root.AddChild(node);
	}
}