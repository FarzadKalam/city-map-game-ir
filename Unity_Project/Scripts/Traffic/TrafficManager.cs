using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GolbaharTycoon.Core;
using GolbaharTycoon.MapSystem;
using GolbaharTycoon.Economy;

namespace GolbaharTycoon.Traffic
{
    public enum VehicleType
    {
        PeugeotPars,
        PrideWhite,
        SamandTaxiYellow,
        SuburbanBus
    }

    /// <summary>
    /// مدیریت تردد خودروها بین پل‌های ورودی استقلال و امام رضا و شبکه‌های داخلی
    /// </summary>
    public class TrafficManager : MonoBehaviour
    {
        public static TrafficManager Instance { get; private set; }

        [Header("پریفب‌های خودروهای بومی")]
        public GameObject peugeotPrefab;
        public GameObject pridePrefab;
        public GameObject taxiYellowPrefab;
        public GameObject busPrefab;

        [Header("تنظیمات تردد")]
        public int maxActiveCars = 35;
        public float spawnIntervalSeconds = 2.5f;

        private List<GameObject> activeVehicles = new List<GameObject>();

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
            StartCoroutine(CarSpawnerCoroutine());
        }

        private IEnumerator CarSpawnerCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnIntervalSeconds);
                if (activeVehicles.Count < maxActiveCars)
                {
                    SpawnCarAtEntrance();
                }
            }
        }

        public void SpawnCarAtEntrance()
        {
            // انتخاب ورودی تصادفی: پل استقلال یا پل امام رضا
            bool useEsteghlal = UnityEngine.Random.value > 0.5f;
            Vector3 startPos = useEsteghlal ? new Vector3(28, 0.4f, -108) : new Vector3(-65, 0.4f, -108);

            GameObject chosenPrefab = peugeotPrefab;
            float r = UnityEngine.Random.value;
            if (r < 0.35f) chosenPrefab = pridePrefab;
            else if (r < 0.65f) chosenPrefab = taxiYellowPrefab;
            else if (r < 0.85f) chosenPrefab = peugeotPrefab;
            else chosenPrefab = busPrefab;

            if (chosenPrefab != null)
            {
                GameObject car = Instantiate(chosenPrefab, startPos, Quaternion.identity, transform);
                activeVehicles.Add(car);

                // دریافت عوارضی اتوماتیک هنگام ورود از پل
                if (EconomyManager.Instance != null)
                {
                    EconomyManager.Instance.CollectHighwayToll(1);
                }
            }
        }

        public void DespawnCar(GameObject car)
        {
            if (activeVehicles.Contains(car))
            {
                activeVehicles.Remove(car);
                Destroy(car);
            }
        }
    }
}
