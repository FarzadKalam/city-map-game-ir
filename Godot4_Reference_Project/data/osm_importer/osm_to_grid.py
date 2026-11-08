#!/usr/bin/env python3
"""
osm_to_grid.py
تبدیل داده‌های جغرافیایی OpenStreetMap (GeoJSON / OSM) به شبکه تایل ایزومتریک گودو ۴
ویژه شهر پایلوت: گلبهار (خراسان رضوی)
"""

import json
import math

# محدوده جغرافیایی تقریبی شهر جدید گلبهار
GOLBAHAR_BBOX = {
    "min_lat": 36.5750,
    "max_lat": 36.6350,
    "min_lon": 59.1800,
    "max_lon": 59.2600
}

GRID_WIDTH = 120
GRID_HEIGHT = 120

def lat_lon_to_grid(lat, lon, grid_w=GRID_WIDTH, grid_h=GRID_HEIGHT):
    """تبدیل طول و عرض جغرافیایی به مختصات گرید شبکه بازی"""
    norm_x = (lon - GOLBAHAR_BBOX["min_lon"]) / (GOLBAHAR_BBOX["max_lon"] - GOLBAHAR_BBOX["min_lon"])
    norm_y = (lat - GOLBAHAR_BBOX["min_lat"]) / (GOLBAHAR_BBOX["max_lat"] - GOLBAHAR_BBOX["min_lat"])
    
    gx = int(clamp(norm_x * grid_w, 0, grid_w - 1))
    gy = int(clamp((1.0 - norm_y) * grid_h, 0, grid_h - 1))
    return gx, gy

def clamp(val, min_v, max_v):
    return max(min_v, min(val, max_v))

def generate_golbahar_starter_grid():
    """تولید ساختار شبکه خیابان‌ها و بلوک‌های اصلی گلبهار بر اساس گرید شطرنجی مدرن آن"""
    grid_cells = {}
    
    # بلوارها و خیابان‌های شریانی اصلی گلبهار (بلوار استقلال، بلوار امام رضا، بلوار پرند و...)
    main_avenues_x = [20, 40, 60, 80, 100]
    main_avenues_y = [20, 40, 60, 80, 100]
    
    for x in range(GRID_WIDTH):
        for y in range(GRID_HEIGHT):
            key = f"{x},{y}"
            
            # معابر اصلی
            if x in main_avenues_x or y in main_avenues_y:
                grid_cells[key] = {
                    "type": "road_avenue",
                    "speed": 60,
                    "zone": "public_transit"
                }
            # بلوک‌های مسکونی و تجاری
            elif (x // 20) % 2 == 0 and (y // 20) % 2 == 0:
                grid_cells[key] = {
                    "type": "residential_zone",
                    "density": "medium",
                    "zone": "residential"
                }
            elif x > 70 and y > 70:
                grid_cells[key] = {
                    "type": "university_and_civic",
                    "name": "پردیس دانشگاه آزاد و مراکز رفاهی",
                    "zone": "civic"
                }
            else:
                grid_cells[key] = {
                    "type": "empty_plot",
                    "price_per_meter": 1200,
                    "zone": "unbuilt"
                }
                
    output_data = {
        "city_name": "Golbahar",
        "province": "Khorasan Razavi",
        "grid_dimensions": [GRID_WIDTH, GRID_HEIGHT],
        "center_coordinates": {"lat": 36.6050, "lon": 59.2200},
        "tile_count": len(grid_cells),
        "cells": grid_cells
    }
    
    with open("golbahar_grid.json", "w", encoding="utf-8") as f:
        json.dump(output_data, f, ensure_ascii=False, indent=2)
        
    print(f"✅ نقشه گرید گلبهار با موفقیت ایجاد شد: {len(grid_cells)} سلول.")

if __name__ == "__main__":
    generate_golbahar_starter_grid()
