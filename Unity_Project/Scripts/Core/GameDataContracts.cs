using System;
using System.Collections.Generic;
using UnityEngine;

namespace GolbaharTycoon.Core
{
    /// <summary>
    /// داده‌های ساختار ۵ سطحی برای هر ساختمان و معبر شهری
    /// </summary>
    public enum BuildingCategory
    {
        Villa,         // خانه ویلایی (۱ تا ۵ سطح)
        MaskanMelli,   // آپارتمان مسکن ملی (۳ تا ۵ طبقه)
        Shop,          // مغازه و کافه (CAFE / GAS)
        Passage,       // پاساژ تجاری
        Mall,          // مجتمع تجاری بزرگ
        Municipality,  // عمارت شهرداری و شورا
        FireStation,   // ایستگاه آتش‌نشانی
        Police,        // کلانتری و امنیت
        Airport        // فرودگاه گلبهار
    }

    public enum RoadTier
    {
        Alley = 1,            // کوچه خاکی باریک
        SecondaryStreet = 2,  // خیابان فرعی با جدول
        MainStreet = 3,       // خیابان اصلی دوطرفه با خط‌کشی
        HighwayBlvd = 4,      // بزرگراه ۴ بانده با بلوار چمن
        FreewayOverpass = 5   // آزادراه هوشمند با پل روگذر
    }

    [System.Serializable]
    public class BuildingData
    {
        public string id;
        public string name;
        public BuildingCategory category;
        public int level = 1; // 1 to 5
        public bool isUpgrading;
        public float upgradeProgress; // 0.0 to 1.0
        public int baseIncome = 25;
        public int basePopulation = 100;
        public Vector3 worldPosition;
    }

    [System.Serializable]
    public class RoadEntity
    {
        public string id;
        public string name;
        public RoadTier tier = RoadTier.MainStreet;
        public bool isBridge;
        public bool isRoundabout;
        public bool isBlockedExit;
        public Vector3 startPoint;
        public Vector3 endPoint;
    }
}
