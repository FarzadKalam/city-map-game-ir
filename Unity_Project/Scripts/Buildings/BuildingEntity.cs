using System.Collections;
using UnityEngine;
using GolbaharTycoon.Core;

namespace GolbaharTycoon.Buildings
{
    /// <summary>
    /// مدیریت وضعیت، فاز کارگاه ساختمانی و ارتقای ۵ سطحی در یونیتی
    /// همراه با انیمیشن فنری Squash & Stretch به سبک Rush Royale
    /// </summary>
    public class BuildingEntity : MonoBehaviour
    {
        public BuildingData data;
        public GameObject scaffoldingPrefab; // مدل داربست و جرثقیل متحرک
        public GameObject[] levelModels = new GameObject[5]; // مدل‌های سطوح ۱ تا ۵

        public BuildingData buildingData => data;

        public void Initialize(BuildingData buildingData)
        {
            this.data = buildingData;
            transform.position = buildingData.worldPosition;
            UpdateVisualMesh();
        }

        public void UpgradeBuildingLevel()
        {
            StartUpgrade();
        }

        public void StartUpgrade()
        {
            if (data == null || data.isUpgrading || data.level >= 5) return;
            StartCoroutine(UpgradeRoutine());
        }

        private IEnumerator UpgradeRoutine()
        {
            data.isUpgrading = true;
            data.upgradeProgress = 0f;

            // نمایش داربست کارگاهی و پنهان‌سازی موقت مدل
            if (scaffoldingPrefab != null) scaffoldingPrefab.SetActive(true);

            float duration = 2.8f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                data.upgradeProgress = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }

            data.isUpgrading = false;
            data.upgradeProgress = 1f;
            data.level++;

            if (scaffoldingPrefab != null) scaffoldingPrefab.SetActive(false);
            UpdateVisualMesh();

            // انیمیشن فنری جهش سطح جدید (Juice Pop)
            StartCoroutine(SquashAndStretchPop());
        }

        public void UpdateVisualMesh()
        {
            for (int i = 0; i < levelModels.Length; i++)
            {
                if (levelModels[i] != null)
                {
                    levelModels[i].SetActive(i == data.level - 1 && !data.isUpgrading);
                }
            }
        }

        private IEnumerator SquashAndStretchPop()
        {
            Vector3 originalScale = Vector3.one;
            transform.localScale = new Vector3(1.28f, 0.72f, 1.28f);

            float time = 0f;
            float popDuration = 0.24f;
            while (time < popDuration)
            {
                time += Time.deltaTime;
                float t = time / popDuration;
                // Ease Out Back
                transform.localScale = Vector3.LerpUnclamped(new Vector3(1.28f, 0.72f, 1.28f), originalScale, t);
                yield return null;
            }
            transform.localScale = originalScale;
        }
    }
}
