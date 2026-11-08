class_name IsoGridMath
extends RefCounted

## IsoGridMath.gd
## محاسبات ریاضی تبدیل مختصات ایزومتریک ۲ به ۱ و نگاشت نقشه

const TILE_WIDTH: float = 128.0
const TILE_HEIGHT: float = 64.0

## تبدیل مختصات گرید شبکه (x, y) به مختصات پیکسلی صفحه (Screen Space)
static func grid_to_screen(grid_pos: Vector2i) -> Vector2:
	var screen_x = (grid_pos.x - grid_pos.y) * (TILE_WIDTH / 2.0)
	var screen_y = (grid_pos.x + grid_pos.y) * (TILE_HEIGHT / 2.0)
	return Vector2(screen_x, screen_y)

## تبدیل مختصات پیکسلی لمس صفحه به خانه گرید شطرنجی
static func screen_to_grid(screen_pos: Vector2) -> Vector2i:
	var x = (screen_pos.x / (TILE_WIDTH / 2.0) + screen_pos.y / (TILE_HEIGHT / 2.0)) / 2.0
	var y = (screen_pos.y / (TILE_HEIGHT / 2.0) - screen_pos.x / (TILE_WIDTH / 2.0)) / 2.0
	return Vector2i(int(floor(x)), int(floor(y)))

## دریافت ۴ همسایه مستقیم خانه (جهت اتصال معابر و انتقال انشعابات آب و برق)
static func get_neighbors(grid_pos: Vector2i) -> Array[Vector2i]:
	return [
		grid_pos + Vector2i(1, 0),
		grid_pos + Vector2i(-1, 0),
		grid_pos + Vector2i(0, 1),
		grid_pos + Vector2i(0, -1)
	]
