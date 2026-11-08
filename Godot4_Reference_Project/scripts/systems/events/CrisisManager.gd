class_name CrisisManager
extends Node

## CrisisManager.gd
## مدیریت بحران‌ها، زلزله، رویدادهای تصادفی و تصمیم‌گیری‌های سیاسی

@export var crisis_check_interval: float = 60.0
var _timer: Timer = Timer.new()

# بانک رویدادهای پایه
var crisis_deck: Array[Dictionary] = [
	{
		"id": "earthquake_minor",
		"title": "زلزله با شدت ۴.۲ ریشتر در دشت چناران و گلبهار",
		"description": "زمین‌لرزه‌ای خفیف منطقه را لرزاند. برخی دیوارهای مسکن مهر ترک برداشته و نیازمند تخصیص بودجه بازسازی است.",
		"type": "natural",
		"cost": 150000,
		"satisfaction_impact": -0.08,
		"severity": 0.4
	},
	{
		"id": "water_scarcity",
		"title": "افت شدید سطح سفره‌های آب زیرزمینی",
		"description": "شرکت آب و فاضلاب خواستار اعمال جریمه بر مصرف بی‌رویه و تکمیل پروژه بازچرخانی پساب شهری شد.",
		"type": "infrastructure",
		"cost": 80000,
		"satisfaction_impact": -0.05,
		"severity": 0.3
	},
	{
		"id": "cultural_festival",
		"title": "برگزاری جشنواره نوروزی فرهنگ و هنر خراسان",
		"description": "استقبال پرشور مسافران مشهد و گردشگران از بازارچه‌های محلی گلبهار؛ افزایش درآمدهای اصناف و رضایت عمومی.",
		"type": "cultural",
		"cost": -50000, # درآمدزا
		"satisfaction_impact": 0.12,
		"severity": 0.0
	}
]

func _ready() -> void:
	add_child(_timer)
	_timer.wait_time = crisis_check_interval
	_timer.autostart = true
	_timer.timeout.connect(_on_crisis_roll)
	_timer.start()

func _on_crisis_roll() -> void:
	# شانس وقوع رویداد در هر چرخه
	if randf() < 0.45 and not crisis_deck.is_empty():
		var event = crisis_deck.pick_random()
		EventBus.crisis_triggered.emit(
			event["id"],
			event["title"],
			Vector2i(randi_range(10, 50), randi_range(10, 50)),
			event["severity"]
		)
