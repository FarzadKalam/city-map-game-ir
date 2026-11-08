class_name RoadNetworkManager
extends Node3D

## RoadNetworkManager.gd (Godot 4.x)
## سیستم ارتقای یکپارچه کل طول معابر (نه بلوک‌به‌بلوک!)
## شامل ۵ سطح: ۱. کوچه خاکی -> ۲. خیابان فرعی -> ۳. خیابان اصلی -> ۴. بزرگراه بلواردار -> ۵. آزادراه هوشمند + میدان و پل روگذر

signal thoroughfare_upgraded(road_id: String, new_level: int)

enum RoadTier {
	ALLEY = 1,            # کوچه خاکی/باریک محلی
	SECONDARY_STREET = 2, # خیابان فرعی آسفالت با جدول
	MAIN_STREET = 3,      # خیابان اصلی دوطرفه با خط‌کشی سفید، پیاده‌رو و تیر چراغ‌برق
	HIGHWAY_BLVD = 4,     # بزرگراه ۴ بانده با بلوار چمن‌کاری میانی و درختان
	FREEWAY_OVERPASS = 5  # آزادراه هوشمند با پل روگذر غیرهمسطح و گانتری
}

var thoroughfares: Dictionary = {
	"rd_esteghlal": {"name": "بلوار استقلال گلبهار", "level": 4, "upgrading": false},
	"rd_parand": {"name": "بلوار پرند", "level": 3, "upgrading": false},
	"rd_mashhad_hwy": {"name": "بزرگراه مشهد - قوچان", "level": 5, "upgrading": false},
	"rd_baharestan": {"name": "خیابان بهارستان (مسکن ملی)", "level": 2, "upgrading": false},
	"rd_daneshjoo": {"name": "خیابان دانشجو", "level": 3, "upgrading": false},
	"rb_parand": {"name": "میدان مرکزی پرند", "level": 4, "upgrading": false}
}

func upgrade_entire_thoroughfare(road_id: String) -> void:
	if not thoroughfares.has(road_id):
		return
	var rd = thoroughfares[road_id]
	if rd["level"] < 5 and not rd["upgrading"]:
		rd["upgrading"] = true
		await get_tree().create_timer(2.8).timeout
		rd["upgrading"] = false
		rd["level"] += 1
		thoroughfare_upgraded.emit(road_id, rd["level"])
