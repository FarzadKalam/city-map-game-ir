class_name EconomyManager
extends Node

## EconomyManager.gd
## مدیریت خزانه‌داری شهری، درآمدها، مالیات و هزینه‌های نگهداری

@export var initial_balance: int = 500000
@export var tax_cycle_seconds: float = 15.0

var current_balance: int = 0
var gross_income_per_cycle: int = 0
var maintenance_cost_per_cycle: int = 0
var tax_rate: float = 0.10 # نرخ مالیات پایه ۱۰٪

@onready var _timer: Timer = Timer.new()

func _ready() -> void:
	current_balance = initial_balance
	add_child(_timer)
	_timer.wait_time = tax_cycle_seconds
	_timer.autostart = true
	_timer.timeout.connect(_on_tax_cycle_tick)
	_timer.start()
	
	# اتصال به سیگنال‌های رویدادها
	EventBus.building_placed.connect(_on_building_placed)
	EventBus.budget_changed.emit(current_balance, 0)

func _on_building_placed(_building_id: String, _grid_pos: Vector2i, cost: int) -> void:
	current_balance -= cost
	# افزایش پایه درآمد مالیاتی متناسب با ساختمان جدید
	gross_income_per_cycle += int(cost * 0.05)
	maintenance_cost_per_cycle += int(cost * 0.01)
	EventBus.budget_changed.emit(current_balance, -cost)

func _on_tax_cycle_tick() -> void:
	var net_profit = gross_income_per_cycle - maintenance_cost_per_cycle
	current_balance += net_profit
	EventBus.tax_collected.emit(gross_income_per_cycle)
	EventBus.revenue_cycle_completed.emit(gross_income_per_cycle, maintenance_cost_per_cycle)
	EventBus.budget_changed.emit(current_balance, net_profit)

func can_afford(amount: int) -> bool:
	return current_balance >= amount
