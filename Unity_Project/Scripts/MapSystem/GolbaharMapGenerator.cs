using System.Collections.Generic;
using UnityEngine;
using GolbaharTycoon.Core;

namespace GolbaharTycoon.MapSystem
{
    /// <summary>
    /// لودر و سازنده خودکار نقشه بسته شهر گلبهار در یونیتی
    /// شامل دو پل ورودی استقلال و امام رضا، رینگ کمربندی بسته و خروجی مسدود آبقد
    /// </summary>
    public class GolbaharMapGenerator : MonoBehaviour
    {
        [Header("پریفب‌های معابر و میادین")]
        public GameObject roadAlleyPrefab;
        public GameObject roadStreetPrefab;
        public GameObject roadBoulevardPrefab;
        public GameObject roadHighwayPrefab;
        public GameObject bridgeEntrancePrefab;
        public GameObject roundaboutPrefab;
        public GameObject blockedExitBarrierPrefab;

        [Header("نقشه راهبردی گلبهار")]
        public List<RoadEntity> golbaharNetwork = new List<RoadEntity>();

        private void Awake()
        {
            SetupGolbaharDefaultNetwork();
        }

        public void SetupGolbaharDefaultNetwork()
        {
            golbaharNetwork.Clear();

            // بزرگراه مشهد - قوچان
            golbaharNetwork.Add(new RoadEntity {
                id = "rd_hwy", name = "بزرگراه مشهد - قوچان", tier = RoadTier.FreewayOverpass,
                startPoint = new Vector3(-140, 0, -95), endPoint = new Vector3(140, 0, -95)
            });

            // پل ورودی ۱: استقلال
            golbaharNetwork.Add(new RoadEntity {
                id = "bridge_esteghlal", name = "پل ورودی ۱ استقلال", isBridge = true,
                startPoint = new Vector3(28, 0, -108), endPoint = new Vector3(28, 0, -82)
            });

            // پل ورودی ۲: امام رضا
            golbaharNetwork.Add(new RoadEntity {
                id = "bridge_imam_reza", name = "پل ورودی ۲ امام رضا", isBridge = true,
                startPoint = new Vector3(-65, 0, -108), endPoint = new Vector3(-65, 0, -82)
            });

            // کمربندی شمالی (ابن سینا)
            golbaharNetwork.Add(new RoadEntity {
                id = "ring_north", name = "بلوار کمربندی شمالی", tier = RoadTier.HighwayBlvd,
                startPoint = new Vector3(-110, 0, -80), endPoint = new Vector3(110, 0, -80)
            });

            // کمربندی جنوبی (پژوهش)
            golbaharNetwork.Add(new RoadEntity {
                id = "ring_south", name = "بلوار کمربندی جنوبی", tier = RoadTier.HighwayBlvd,
                startPoint = new Vector3(-110, 0, 85), endPoint = new Vector3(110, 0, 85)
            });

            // بلوار استقلال
            golbaharNetwork.Add(new RoadEntity {
                id = "rd_esteghlal", name = "بلوار استقلال گلبهار", tier = RoadTier.HighwayBlvd,
                startPoint = new Vector3(28, 0, -80), endPoint = new Vector3(28, 0, 85)
            });

            // بلوار پرند
            golbaharNetwork.Add(new RoadEntity {
                id = "rd_parand", name = "بلوار پرند", tier = RoadTier.HighwayBlvd,
                startPoint = new Vector3(-110, 0, 0), endPoint = new Vector3(110, 0, 0)
            });

            // خروجی مسدود آبقد
            golbaharNetwork.Add(new RoadEntity {
                id = "rd_abqad", name = "جاده آبقد (مسدود)", isBlockedExit = true, tier = RoadTier.SecondaryStreet,
                startPoint = new Vector3(-150, 0, 30), endPoint = new Vector3(-110, 0, 30)
            });

            // میدان پرند
            golbaharNetwork.Add(new RoadEntity {
                id = "rb_parand", name = "میدان پرند", isRoundabout = true, tier = RoadTier.HighwayBlvd,
                startPoint = new Vector3(28, 0, 0)
            });
        }
    }
}
