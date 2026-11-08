using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GolbaharTycoon.Economy;
using GolbaharTycoon.Core;

namespace GolbaharTycoon.UI
{
    /// <summary>
    /// کنترلر رابط کاربری به سبک بازی Rush Royale و Clash Royale
    /// شامل کپسول‌های منابع سه‌بعدی، نشان‌های فلزی سپر سطح ساختمان (۱ تا ۵) و پنجره پاپ‌آپ ارتقا
    /// </summary>
    public class RushRoyaleHUDController : MonoBehaviour
    {
        public static RushRoyaleHUDController Instance { get; private set; }

        [Header("کپسول‌های منابع بالای صفحه (Pill Counters)")]
        public TextMeshProUGUI treasuryText;
        public TextMeshProUGUI gemsText;
        public TextMeshProUGUI populationText;
        public TextMeshProUGUI happinessText;
        public Slider happinessSlider;

        [Header("پاپ‌آپ ارتقای ساختمان (Rush Royale Card Modal)")]
        public GameObject upgradeModalRoot;
        public TextMeshProUGUI buildingNameText;
        public TextMeshProUGUI currentLevelBadgeText;
        public TextMeshProUGUI nextLevelBadgeText;
        public TextMeshProUGUI upgradeCostText;
        public TextMeshProUGUI incomeDeltaText;
        public Button confirmUpgradeButton;
        public Button closeModalButton;

        [Header("انیمیشن‌های Juice / Feel")]
        public AnimationCurve bounceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private BuildingEntity _selectedBuilding;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (closeModalButton != null)
                closeModalButton.onClick.AddListener(CloseUpgradeModal);
            if (confirmUpgradeButton != null)
                confirmUpgradeButton.onClick.AddListener(OnConfirmUpgradeClicked);
        }

        private void Start()
        {
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.OnTreasuryChanged += UpdateTreasuryUI;
                EconomyManager.Instance.OnGemsChanged += UpdateGemsUI;
                EconomyManager.Instance.OnPopulationChanged += UpdatePopulationUI;
                EconomyManager.Instance.OnHappinessChanged += UpdateHappinessUI;

                // مقداردهی اولیه
                UpdateTreasuryUI(EconomyManager.Instance.TreasuryToman);
                UpdateGemsUI(EconomyManager.Instance.Gems);
                UpdatePopulationUI(EconomyManager.Instance.TotalPopulation);
                UpdateHappinessUI(EconomyManager.Instance.Happiness);
            }

            if (upgradeModalRoot != null)
                upgradeModalRoot.SetActive(false);
        }

        public void OpenUpgradeModal(BuildingEntity building)
        {
            _selectedBuilding = building;
            if (upgradeModalRoot == null || building == null) return;

            upgradeModalRoot.SetActive(true);
            upgradeModalRoot.transform.localScale = Vector3.one * 0.7f;
            StartCoroutine(PopScaleCoroutine(upgradeModalRoot.transform, Vector3.one, 0.25f));

            if (buildingNameText != null) buildingNameText.text = building.buildingData.name;
            if (currentLevelBadgeText != null) currentLevelBadgeText.text = $"سطح {building.buildingData.level}";
            if (nextLevelBadgeText != null) nextLevelBadgeText.text = $"سطح {building.buildingData.level + 1}";

            long cost = (long)(building.buildingData.level * 25000000);
            if (upgradeCostText != null) upgradeCostText.text = $"{cost:N0} تومان";
            if (incomeDeltaText != null) incomeDeltaText.text = $"+{(building.buildingData.level * 35):N0} درآمد/دقیقه";
        }

        public void CloseUpgradeModal()
        {
            if (upgradeModalRoot != null)
                StartCoroutine(PopScaleCoroutine(upgradeModalRoot.transform, Vector3.zero, 0.15f, () => upgradeModalRoot.SetActive(false)));
        }

        private void OnConfirmUpgradeClicked()
        {
            if (_selectedBuilding != null)
            {
                _selectedBuilding.UpgradeBuildingLevel();
                CloseUpgradeModal();
            }
        }

        private void UpdateTreasuryUI(long amount)
        {
            if (treasuryText != null) treasuryText.text = $"{amount:N0} ت";
        }

        private void UpdateGemsUI(int amount)
        {
            if (gemsText != null) gemsText.text = amount.ToString();
        }

        private void UpdatePopulationUI(int count)
        {
            if (populationText != null) populationText.text = $"{count:N0} نفر";
        }

        private void UpdateHappinessUI(float value)
        {
            if (happinessText != null) happinessText.text = $"%{Mathf.RoundToInt(value)}";
            if (happinessSlider != null) happinessSlider.value = value / 100f;
        }

        private IEnumerator PopScaleCoroutine(Transform target, Vector3 targetScale, float duration, System.Action onComplete = null)
        {
            Vector3 startScale = target.localScale;
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                target.localScale = Vector3.LerpUnclamped(startScale, targetScale, bounceCurve.Evaluate(t));
                yield return null;
            }
            target.localScale = targetScale;
            onComplete?.Invoke();
        }
    }
}
