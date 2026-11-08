using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GolbaharTycoon.Economy;

namespace GolbaharTycoon.Crisis
{
    public enum CrisisType
    {
        KashafRudEarthquake, // زلزله گسل کشف‌رود
        ChenaranDustStorm,   // طوفان شن و ریزگرد دشت چناران
        WinterWaterPipeBurst,// ترکیدگی لوله‌های آب در یخبندان زمستانه
        IndustrialPowerCut   // قطعی برق شهرک صنعتی چناران و گلبهار
    }

    [System.Serializable]
    public class CrisisEvent
    {
        public CrisisType type;
        public string titleFa;
        public string descriptionFa;
        public float severity; // 1 to 10
        public long repairCostToman;
        public float happinessPenalty;
    }

    /// <summary>
    /// مدیریت حوادث و بلایای طبیعی واقعی منطقه گلبهار و چناران
    /// </summary>
    public class CrisisSystem : MonoBehaviour
    {
        public static CrisisSystem Instance { get; private set; }

        public event Action<CrisisEvent> OnCrisisStarted;
        public event Action<CrisisEvent> OnCrisisResolved;

        [Header("تنظیمات شبیه‌ساز بحران")]
        public bool enableRandomCrises = true;
        public float minCrisisIntervalSeconds = 180f;
        public float maxCrisisIntervalSeconds = 420f;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (enableRandomCrises)
            {
                StartCoroutine(CrisisSchedulerCoroutine());
            }
        }

        private IEnumerator CrisisSchedulerCoroutine()
        {
            while (true)
            {
                float waitTime = UnityEngine.Random.Range(minCrisisIntervalSeconds, maxCrisisIntervalSeconds);
                yield return new WaitForSeconds(waitTime);

                TriggerRandomCrisis();
            }
        }

        public void TriggerRandomCrisis()
        {
            CrisisType randomType = (CrisisType)UnityEngine.Random.Range(0, 4);
            CrisisEvent ev = CreateCrisisEvent(randomType);

            // اعمال اثر بر رضایت
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.ModifyHappiness(-ev.happinessPenalty);
            }

            OnCrisisStarted?.Invoke(ev);
            Debug.LogWarning($"[Crisis] بحران رخ داد: {ev.titleFa} - شدت {ev.severity}");
        }

        public bool ResolveCrisis(CrisisEvent ev)
        {
            if (EconomyManager.Instance != null && EconomyManager.Instance.SpendTreasury(ev.repairCostToman))
            {
                EconomyManager.Instance.ModifyHappiness(ev.happinessPenalty * 1.2f); // بازیابی رضایت با پاداش مدیریت بحران
                OnCrisisResolved?.Invoke(ev);
                return true;
            }
            return false;
        }

        private CrisisEvent CreateCrisisEvent(CrisisType type)
        {
            switch (type)
            {
                case CrisisType.KashafRudEarthquake:
                    return new CrisisEvent {
                        type = type,
                        titleFa = "زلزله گسل کشف‌رود",
                        descriptionFa = "زمین‌لرزه ۵.۲ ریشتری در امتداد دشت کشف‌رود به برخی سازه‌ها آسیب وارد کرد.",
                        severity = 6.8f,
                        repairCostToman = 45000000,
                        happinessPenalty = 14f
                    };
                case CrisisType.ChenaranDustStorm:
                    return new CrisisEvent {
                        type = type,
                        titleFa = "ریزگرد و طوفان شن دشت چناران",
                        descriptionFa = "وزش بادهای شدید و ریزگرد باعث کاهش دید و اختلال در عبور و مرور بزرگراه شد.",
                        severity = 4.2f,
                        repairCostToman = 12000000,
                        happinessPenalty = 8f
                    };
                case CrisisType.WinterWaterPipeBurst:
                    return new CrisisEvent {
                        type = type,
                        titleFa = "یخبندان و ترکیدگی لوله‌های آب",
                        descriptionFa = "سرمای منفی ۱۵ درجه شبانه گلبهار منجر به افت فشار آب محله‌های مسکن ملی شد.",
                        severity = 5.0f,
                        repairCostToman = 22000000,
                        happinessPenalty = 11f
                    };
                case CrisisType.IndustrialPowerCut:
                default:
                    return new CrisisEvent {
                        type = type,
                        titleFa = "افت ولتاژ شهرک صنعتی",
                        descriptionFa = "قطعی برق اضطراری در پست‌های توزیع منجر به کاهش درآمد کارگاه‌ها شد.",
                        severity = 3.5f,
                        repairCostToman = 18000000,
                        happinessPenalty = 6f
                    };
            }
        }
    }
}
