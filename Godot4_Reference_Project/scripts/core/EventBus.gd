extends Node

## EventBus.gd
## گذرگاه رویداد سراسری (Global Event Bus)
## جهت تفکیک کامل ماژول‌ها و ارتباط رویدادمحور بین سیستم‌ها

# رویدادهای ساخت‌وساز و نقشه
signal building_placed(building_id: String, grid_pos: Vector2i, cost: int)
signal building_removed(building_id: String, grid_pos: Vector2i)
signal building_upgraded(building_id: String, grid_pos: Vector2i, new_level: int)
signal zone_selected(grid_pos: Vector2i)

# رویدادهای مالی و اقتصاد
signal budget_changed(current_balance: int, delta: int)
signal tax_collected(total_tax: int)
signal revenue_cycle_completed(gross: int, maintenance: int)

# رویدادهای اجتماعی و رضایت عمومی
signal satisfaction_updated(new_rate: float)
signal population_changed(current_pop: int, growth_rate: float)

# رویدادهای حوادث و بحران‌ها
signal crisis_triggered(crisis_id: String, title: String, impact_zone: Vector2i, severity: float)
signal crisis_resolved(crisis_id: String, success: bool)

# رویدادهای سیاسی، شورا و انتخابات
signal election_called(level: String, candidates: Array)
signal council_resolution_voted(resolution_id: String, passed: bool)
signal government_budget_allocated(amount: int, department: String)
