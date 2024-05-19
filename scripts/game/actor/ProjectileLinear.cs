using Godot;

namespace Unknown.Game; 

public partial class ProjectileLinear : Node3D {
	private Vector3 _direction;
	private float _speed;
	
	public void Initialize(Vector3 startPosition, Vector3 direction, float speed) {
		Position = startPosition;
		_direction = direction;
		_speed = speed;
	}
	
	public override void _Process(double delta) {
		Position += _direction * _speed * (float)delta;
	}
}