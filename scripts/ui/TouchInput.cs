using Godot;
using System;
using System.Collections.Generic;

namespace Unknown.UI;

public partial class TouchInput : Node {
	private double _time;

	private readonly Dictionary<int, TouchData> _startTouches = new();
	private readonly Dictionary<int, TouchData> _lastTouches = new();

	public SwipeAxis CurrentSwipeAxis = SwipeAxis.None;
	
	public Action<TouchData> OnFingerDown;
	public Action<TouchData> OnFingerMove;
	public Action<TouchData> OnFingerUp;
	
	public Action<TouchData> OnHorizontalSwipeDown;
	public Action<TouchData> OnHorizontalSwipeMove;
	public Action<TouchData> OnHorizontalSwipeUp;
	
	public Action<TouchData> OnVerticalSwipeDown;
	public Action<TouchData> OnVerticalSwipeMove;
	public Action<TouchData> OnVerticalSwipeUp;

	public override void _Process(double delta) {
		_time += delta;
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventScreenTouch touch) {
			if (touch.IsPressed()) {
				var data = new TouchData(
					touch.Index,
					true,
					false,
					false,
					touch.Position,
					0f,
					_time,
					_time,
					Vector2.Zero, 
					Vector2.Zero,
					Vector2.Zero
				);
				_startTouches.Add(touch.Index, data);
				_lastTouches.Add(touch.Index, data);

				OnFingerDown?.Invoke(data);
			}
			
			if (touch.IsReleased()) {
				var startData = _startTouches[touch.Index];
				var lastData = _lastTouches[touch.Index];
				
				var data = new TouchData(
					touch.Index,
					false,
					false,
					true,
					touch.Position,
					(float)(_time - startData.StartTime),
					startData.StartTime,
					_time,
					touch.Position - lastData.Position,
					lastData.FrameDelta,
					touch.Position - startData.Position
				);

				_startTouches.Remove(touch.Index);
				_lastTouches.Remove(touch.Index);
				
				OnFingerUp?.Invoke(data);

				switch (CurrentSwipeAxis) {
					case SwipeAxis.Horizontal:
						OnHorizontalSwipeUp?.Invoke(data);
						break;
					case SwipeAxis.Vertical:
						OnVerticalSwipeUp?.Invoke(data);
						break;
				}

				CurrentSwipeAxis = SwipeAxis.None;
			}
		}

		if (@event is InputEventScreenDrag drag) {
			var startData = _startTouches[drag.Index];
			var lastData = _lastTouches[drag.Index];
			
			var data = new TouchData(
				drag.Index,
				false,
				true,
				false,
				drag.Position,
				0f,
				startData.StartTime,
				_time,
				drag.Position - lastData.Position,
				lastData.FrameDelta,
				drag.Position - startData.Position
			);
			
			_lastTouches[drag.Index] = data;
			
			if (CurrentSwipeAxis == SwipeAxis.None) {
				var horizontal = Mathf.Abs(data.MoveDelta.X) > Mathf.Abs(data.MoveDelta.Y);
				CurrentSwipeAxis = horizontal ? SwipeAxis.Horizontal : SwipeAxis.Vertical;

				switch (CurrentSwipeAxis) {
					case SwipeAxis.Horizontal:
						OnHorizontalSwipeDown?.Invoke(data);
						break;
					case SwipeAxis.Vertical:
						OnVerticalSwipeDown?.Invoke(data);
						break;
				}
			}
			
			OnFingerMove?.Invoke(data);
			
			switch (CurrentSwipeAxis) {
				case SwipeAxis.Horizontal:
					OnHorizontalSwipeMove?.Invoke(data);
					break;
				case SwipeAxis.Vertical:
					OnVerticalSwipeMove?.Invoke(data);
					break;
			}
		}
	}

	public struct TouchData {
		public int FingerId { get; private set; }
		public bool Start { get; private set; }
		public bool Move { get; private set; }
		public bool End { get; private set; }
		public Vector2 Position { get; private set; }
		public float Duration { get; private set; }

		public double StartTime { get; private set; }
		public double Time { get; private set; }
		public Vector2 FrameDelta { get; private set; }
		public Vector2 LastFrameDelta { get; private set; }
		public Vector2 MoveDelta { get; private set; }

		public TouchData(int fingerId, bool start, bool move, bool end, Vector2 position, float duration, double startTime, double time, Vector2 frameDelta, Vector2 lastFrameDelta, Vector2 moveDelta) {
			FingerId = fingerId;
			Start = start;
			Move = move;
			End = end;
			Position = position;
			Duration = duration;
			StartTime = startTime;
			Time = time;
			FrameDelta = frameDelta;
			LastFrameDelta = lastFrameDelta;
			MoveDelta = moveDelta;
		}
	}
	
	public enum SwipeAxis {
		None,
		Horizontal,
		Vertical,
	}
}
