using System;
using UnityEngine;
using GolbaharTycoon.Core;

namespace GolbaharTycoon.Economy
{
    /// <summary>
    /// سیستم جامع اقتصاد شهری، خزانه‌داری، دریافت عوارض و رضایت شهروندان
    /// </summary>
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }

        [Header("خزانه و منابع (Rush Royale Style)")]
        [SerializeField] private long _treasuryToman = 125000000; // تومان
        [SerializeField] private int _gems = 150;                  // الماس / ارز ویژه
        [SerializeField] private int _totalPopulation = 42500;    // جمعیت شهر
        [SerializeField] private float _happiness = 84f;          // رضایت ۰ تا ۱۰۰

        [Header("نرخ‌های عوارض و درآمد")]
        public float taxCollectionInterval = 60f; // ثانیه
        public long tollIncomePerCar = 15000;    // عوارضی ورودی بزرگراه
        public float renovationTaxRate = 0.05f;  // عوارض نوسازی
        
        [Header("رویدادهای اقتصادی")]
        public event Action<long> OnTreasuryChanged;
        public event Action<int> OnGemsChanged;
        public event Action<int> OnPopulationChanged;
        public event Action<float> OnHappinessChanged;
        public event Action<long> OnTollCollected;

        private float _taxTimer = 0f;

        public long TreasuryToman => _treasuryToman;
        public int Gems => _gems;
        public int TotalPopulation => _totalPopulation;
        public float Happiness => _happiness;

        private void Awake()
        {
            if (Instance != null && Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            _taxTimer += Time.deltaTime;
            if (_taxTimer >= taxCollectionInterval)
            {
                _taxTimer = 0f;
                CollectPeriodicCityTaxes();
            }
        }

        /// <summary>
        /// دریافت عوارض دوره‌ای نوسازی و خدمات شهری از تمام محلات
        /// </summary>
        public void CollectPeriodicCityTaxes()
        {
            long periodicIncome = (long)(_totalPopulation * 1200 * (_happiness / 100f));
            AddTreasury(periodicIncome);
            Debug.Log($"[Economy] عوارض دوره‌ای وصول شد: {periodicIncome:N0} تومان");
        }

        /// <summary>
        /// دریافت عوارضی خودروهای ورودی از پل‌های استقلال و امام رضا
        /// </summary>
        public void CollectHighwayToll(int carCount = 1)
        {
            long totalToll = carCount * tollIncomePerCar;
            AddTreasury(totalToll);
            OnTollCollected?.Invoke(totalToll);
        }

        public bool SpendTreasury(long amount)
        {
            if (_treasuryToman >= amount)
            {
                _treasuryToman -= amount;
                OnTreasuryChanged?.Invoke(_treasuryToman);
                return true;
            }
            return false;
        }

        public void AddTreasury(long amount)
        {
            _treasuryToman += amount;
            OnTreasuryChanged?.Invoke(_treasuryToman);
        }

        public bool SpendGems(int amount)
        {
            if (_gems >= amount)
            {
                _gems -= amount;
                OnGemsChanged?.Invoke(_gems);
                return true;
            }
            return false;
        }

        public void AddGems(int amount)
        {
            _gems += amount;
            OnGemsChanged?.Invoke(_gems);
        }

        public void ModifyHappiness(float delta)
        {
            _happiness = Mathf.Clamp(_happiness + delta, 0f, 100f);
            OnHappinessChanged?.Invoke(_happiness);
        }

        public void AddPopulation(int count)
        {
            _totalPopulation += count;
            OnPopulationChanged?.Invoke(_totalPopulation);
        }
    }
}
