using Godot;

namespace Unknown.Game; 

public partial class LoadScene : Node {
	
	[Export] public PackedScene ClientScene;
	[Export] public PackedScene ServerScene;

	
	public override void _Process(double delta) {
		foreach (var cmdlineArg in OS.GetCmdlineArgs()) {
			if (cmdlineArg == "Client") {
				GetTree().ChangeSceneToPacked(ClientScene);
			}
		
			if (cmdlineArg == "Server") {
				GetTree().ChangeSceneToPacked(ServerScene);
			}
		}
	}
}