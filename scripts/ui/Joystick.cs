using System;
using Godot;

namespace Unknown.UI;

public partial class Joystick : Control {
	[Export] private TouchInput _touchInput;
	[Export] private Control _joystickKnob;
	[Export] private Control _joystickRing;
	[Export] private bool _isFixed;
	[Export] private float _deadZone = 0.2f;


	private bool _allowInput = true;
	private Vector2 _startPosition;
	private bool _pressing;
	private int _fingerId;

	private Vector2 _value = Vector2.Zero;
	public Vector2 Value => _value;
	
	public Action<Vector2> OnJoystickReleased;
	public Action OnClick;

	public override void _EnterTree() {
		_touchInput.OnFingerDown += OnFingerDown;
		_touchInput.OnFingerMove += OnFingerMove;
		_touchInput.OnFingerUp += OnFingerUp;
	}

	public override void _ExitTree() {
		_touchInput.OnFingerDown -= OnFingerDown;
		_touchInput.OnFingerMove -= OnFingerMove;
		_touchInput.OnFingerUp -= OnFingerUp;
	}

	public override void _Ready() {
		_startPosition = _joystickKnob.GlobalPosition + _joystickKnob.Size * 0.5f;
	}

	private void OnFingerDown(TouchInput.TouchData touchData) {
		if (!_allowInput) {
			return;
		}
		
		if (!GetGlobalRect().HasPoint(touchData.Position)) {
			return;
		}

		_allowInput = false;
		_fingerId = touchData.FingerId;
		
		if (!_isFixed) {
			_joystickKnob.GlobalPosition = touchData.Position - _joystickKnob.Size * 0.5f;
			_joystickRing.GlobalPosition = touchData.Position - _joystickRing.Size * 0.5f;
		}

		_pressing = true;
	}
	
	private void OnFingerMove(TouchInput.TouchData touchData) {
		if (!_pressing) {
			/*
			var absolute = _joystickKnob.GlobalPosition.Lerp(GlobalPosition, (float)(delta * 10));
			var distance = absolute.DistanceTo(GlobalPosition);
			if (distance <= 0.001f) {
				_value = Vector2.Zero;
				_joystickKnob.GlobalPosition = GlobalPosition;
				return;
			}
			var relative = absolute - GlobalPosition;
			_value = relative * (1 / _maxLength);
			_joystickKnob.GlobalPosition = absolute;
			*/
			return;
		}
		
		if (touchData.FingerId != _fingerId) {
			return;
		}
		
		var ringCenter = _joystickRing.GlobalPosition + _joystickRing.Size * 0.5f;
		var direction = (touchData.Position - ringCenter).Normalized();

		var maxLength = _joystickRing.Size.X * 0.5f;

		var length = Mathf.Min(touchData.Position.DistanceTo(ringCenter), maxLength);

		var scaled = direction * length * (1 / maxLength);

		_value = scaled.Length() > _deadZone ? scaled : Vector2.Zero;
		
		_joystickKnob.GlobalPosition = (ringCenter + direction * length - _joystickKnob.Size * 0.5f);
	}

	private void OnFingerUp(TouchInput.TouchData touchData) {
		if (!_pressing) {
			return;
		}
		
		if (touchData.FingerId != _fingerId) {
			return;
		}
		
		if (!_isFixed) {
			_joystickKnob.GlobalPosition = _startPosition - _joystickKnob.Size * 0.5f;
			_joystickRing.GlobalPosition = _startPosition - _joystickRing.Size * 0.5f;
		}
		
		if (Value.Length() > _deadZone) {
			OnJoystickReleased?.Invoke(Value);
		}

		if (touchData.Duration < 0.3f && touchData.MoveDelta.Length() < 5f) {
			OnClick?.Invoke();
		}

		_value = Vector2.Zero;
		_pressing = false;
		_allowInput = true;
	}
}