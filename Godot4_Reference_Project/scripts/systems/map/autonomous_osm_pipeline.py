#!/usr/bin/env python3
"""
autonomous_osm_pipeline.py
پایپ‌لاین خودکار ایجنت برای ترجمه داده‌های خام OpenStreetMap به ساختار قابل رندر بازی (گودو ۴ و WebGL)
"""

import json
import math
import os

# دسته‌بندی استانداردهای تگ‌های معابر در OpenStreetMap به ۵ سطح بازی
ROAD_TIER_MAPPING = {
    # سطح ۱: کوچه‌ها و راه‌های باریک محلی
    "service": 1, "living_street": 1, "alley": 1,
    # سطح ۲: خیابان‌های فرعی محله
    "residential": 2, "unclassified": 2, "tertiary": 2,
    # سطح ۳: خیابان‌های اصلی دوطرفه
    "secondary": 3, "primary": 3,
    # سطح ۴: بلوارها و شریان‌های بزرگ شهری
    "trunk": 4, "primary_link": 4, "trunk_link": 4,
    # سطح ۵: آزادراه‌ها و پل‌های غیرهمسطح
    "motorway": 5, "motorway_link": 5, "bridge": 5
}

# دسته‌بندی استانداردهای کاربری ساختمان‌ها در OpenStreetMap به ۹ رده بازی
BUILDING_CATEGORY_MAPPING = {
    "apartments": "maskan_melli",
    "residential": "maskan_melli",
    "house": "villa",
    "detached": "villa",
    "retail": "shop",
    "shop": "shop",
    "commercial": "passage",
    "supermarket": "mall",
    "mall": "mall",
    "civic": "municipality",
    "townhall": "municipality",
    "government": "municipality",
    "fire_station": "fire_station",
    "police": "police",
    "aerodrome": "airport",
    "hangar": "airport"
}

def translate_osm_elements_to_game_entities(osm_elements, city_center_lat=36.6052, city_center_lon=59.2068):
    """
    تبدیل مختصات جغرافیایی (Lat/Lon) به دستگاه مختصات متری بازی (x, z) حول مرکز شهر
    و طبقه‌بندی خودکار معابر و ساختمان‌ها در سطوح ۱ تا ۵
    """
    METERS_PER_LAT_DEGREE = 111139.0
    METERS_PER_LON_DEGREE = 111139.0 * math.cos(math.radians(city_center_lat))

    roads_network = []
    buildings_list = []

    for el in osm_elements:
        tags = el.get("tags", {})

        # ۱. پردازش و ترجمه معابر شهری
        if "highway" in tags:
            h_type = tags.get("highway")
            tier = ROAD_TIER_MAPPING.get(h_type, 2)
            name = tags.get("name:fa") or tags.get("name") or f"معبر {h_type}"
            
            # استخراج هندسه مختصات خط خیابان
            points = []
            for node in el.get("geometry", []):
                rel_x = (node["lon"] - city_center_lon) * METERS_PER_LON_DEGREE
                rel_z = (city_center_lat - node["lat"]) * METERS_PER_LAT_DEGREE
                points.append([round(rel_x, 1), round(rel_z, 1)])

            if len(points) >= 2:
                roads_network.append({
                    "id": f"osm_way_{el.get('id')}",
                    "name": name,
                    "kind": "road",
                    "level": tier,
                    "osm_type": h_type,
                    "points": points
                })

        # ۲. پردازش و ترجمه ساختمان‌ها و کاربری‌ها
        elif "building" in tags or "amenity" in tags:
            b_tag = tags.get("building") or tags.get("amenity")
            category = BUILDING_CATEGORY_MAPPING.get(b_tag, "maskan_melli")
            name = tags.get("name:fa") or tags.get("name") or f"سازه {category}"

            # تخمین سطح اولیه بر اساس تعداد طبقات ثبت‌شده در OSM
            levels_count = 1
            if "building:levels" in tags:
                try:
                    raw_levels = int(tags.get("building:levels"))
                    levels_count = max(1, min(5, raw_levels))
                except:
                    levels_count = 2

            # مختصات مرکز ثقل ساختمان
            center_lat = el.get("lat")
            center_lon = el.get("lon")
            if not center_lat and "center" in el:
                center_lat = el["center"]["lat"]
                center_lon = el["center"]["lon"]

            if center_lat and center_lon:
                rel_x = (center_lon - city_center_lon) * METERS_PER_LON_DEGREE
                rel_z = (city_center_lat - center_lat) * METERS_PER_LAT_DEGREE

                buildings_list.append({
                    "id": f"osm_bldg_{el.get('id')}",
                    "name": name,
                    "type": category,
                    "level": levels_count,
                    "x": round(rel_x, 1),
                    "z": round(rel_z, 1),
                    "baseIncome": levels_count * 12,
                    "basePop": levels_count * 45
                })

    return {
        "city_metadata": {
            "center": [city_center_lat, city_center_lon],
            "total_roads_processed": len(roads_network),
            "total_buildings_processed": len(buildings_list)
        },
        "roads_network": roads_network,
        "buildings": buildings_list
    }

if __name__ == "__main__":
    print("ماژول مترجم خودکار OSM به داده‌های ۵ سطحی بازی فعال است.")
