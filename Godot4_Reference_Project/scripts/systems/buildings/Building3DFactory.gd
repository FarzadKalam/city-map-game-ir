class_name Building3DFactory
extends Node3D

## Building3DFactory.gd (Godot 4.x)
## سیستم تولید و ارتقای ۵ سطحی ساختمان‌ها به همراه وضعیت کارگاه ساخت (Under Construction)
## و انیمیشن‌های فنری (Game Feel Pop) و متریال مات رسی (Matte Clay Look)

signal upgrade_started(building_id: String, target_level: int)
signal upgrade_completed(building_id: String, new_level: int)

enum BuildingCategory {
	VILLA,          # خانه ویلایی (۱ تا ۵ سطح)
	MASKAN_MELLI,   # آپارتمان مسکن ملی (۳ طبقه تا ۵ طبقه)
	SHOP,           # مغازه، کافه (CAFE) و جایگاه خدمات (GAS)
	PASSAGE,        # پاساژ تجاری
	MALL,           # مجتمع تجاری بزرگ
	MUNICIPALITY,   # ساختمان شهرداری
	FIRE_STATION,   # ایستگاه آتش‌نشانی
	POLICE,         # کلانتری
	AIRPORT         # فرودگاه و برج مراقبت
}

static func create_clay_material(hex_color: String, roughness: float = 0.88) -> StandardMaterial3D:
	var mat := StandardMaterial3D.new()
	mat.albedo_color = Color(hex_color)
	mat.roughness = roughness
	mat.metallic = 0.04
	return mat

## انیمیشن فنری Squash & Stretch هنگام پایان ارتقا (برگرفته از مهارت game-feel)
func play_completion_pop(target_node: Node3D) -> void:
	target_node.scale = Vector3(1.26, 0.72, 1.26)
	var tw := create_tween()
	tw.tween_property(target_node, "scale", Vector3.ONE, 0.24) \
		.set_trans(Tween.TRANS_BACK).set_ease(Tween.EASE_OUT)
