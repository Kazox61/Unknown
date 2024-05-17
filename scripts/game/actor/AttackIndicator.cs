using Godot;

namespace Unknown.Game; 

public partial class AttackIndicator : MeshInstance3D {
	public override void _PhysicsProcess(double delta) {
		if (Mesh is ImmediateMesh immediateMesh) {
			immediateMesh.ClearSurfaces();
		}
		
		DrawLine(Vector2.Zero, 2, 0.5f, new Color(0f, 0f, 0f));
	}

	public void DrawLine(Vector2 startPoint, float length, float thickness, Color color) {
		if (Mesh is not ImmediateMesh immediateMesh) {
			return;
		}
		
		immediateMesh.SurfaceBegin(Mesh.PrimitiveType.TriangleStrip);
		immediateMesh.SurfaceSetColor(color);

		var a = new Vector3(startPoint.X - thickness, 0f, startPoint.Y);
		var b = new Vector3(startPoint.X + thickness, 0f, startPoint.Y);
		var c = new Vector3(startPoint.X - thickness, 0f, startPoint.Y + length);
		var d = new Vector3(startPoint.X + thickness, 0f, startPoint.Y + length);
		
		immediateMesh.SurfaceAddVertex(a);
		immediateMesh.SurfaceAddVertex(b);
		immediateMesh.SurfaceAddVertex(c);
		immediateMesh.SurfaceAddVertex(d);
			
		immediateMesh.SurfaceEnd();
	}
	
	public void Rotate(Vector2 direction) {
		var angleRad = Mathf.Atan2(direction.X, direction.Y);
		Rotation = new Vector3(0, angleRad, 0);
	}
}