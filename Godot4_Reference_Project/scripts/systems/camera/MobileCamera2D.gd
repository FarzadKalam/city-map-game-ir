class_name MobileCamera2D
extends Camera2D

## MobileCamera2D.gd
## کنترلر دوربین لمسی موبایل برای نمای ایزومتریک
## پشتیبانی از Drag-Pan، زوم دو انگشتی (Pinch-to-zoom) و اینرسی نرم

@export var min_zoom: float = 0.4
@export var max_zoom: float = 2.2
@export var pan_speed: float = 1.0
@export var inertia_damping: float = 0.92

var _touch_points: Dictionary = {}
var _touch_distance_prev: float = 0.0
var _velocity: Vector2 = Vector2.ZERO
var _is_dragging: bool = false

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventScreenTouch:
		if event.pressed:
			_touch_points[event.index] = event.position
			_velocity = Vector2.ZERO
		else:
			_touch_points.erase(event.index)
			if _touch_points.size() < 2:
				_touch_distance_prev = 0.0

	elif event is InputEventScreenDrag:
		_touch_points[event.index] = event.position
		
		# تک لمسی: جابه‌جایی دوربین روی نقشه (Pan)
		if _touch_points.size() == 1:
			var delta_pos = event.relative * pan_speed / zoom.x
			position -= delta_pos
			_velocity = -delta_pos * 40.0
			
		# دو لمسی: زوم با دو انگشت (Pinch to Zoom)
		elif _touch_points.size() == 2:
			var points = _touch_points.values()
			var dist = points[0].distance_to(points[1])
			
			if _touch_distance_prev > 0.0:
				var factor = dist / _touch_distance_prev
				var target_zoom = clamp(zoom.x * factor, min_zoom, max_zoom)
				zoom = Vector2(target_zoom, target_zoom)
				
			_touch_distance_prev = dist

func _process(delta: float) -> void:
	# اینرسی و لغزش نرم دوربین بعد از رها کردن انگشت
	if _touch_points.is_empty() and _velocity.length() > 5.0:
		position += _velocity * delta
		_velocity *= pow(inertia_damping, delta * 60.0)
	elif _touch_points.is_empty():
		_velocity = Vector2.ZERO
