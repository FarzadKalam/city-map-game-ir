using System;
using System.Collections.Generic;
using UnityEngine;
using GolbaharTycoon.Economy;

namespace GolbaharTycoon.Governance
{
    [System.Serializable]
    public class CouncilMember
    {
        public string nameFa;
        public string commission; // کمیسیون عمران، فرهنگی، بودجه، خدمات شهری
        public float loyalty;     // ۰ تا ۱۰۰
        public bool votesYes;
    }

    [System.Serializable]
    public class CouncilBill
    {
        public string id;
        public string titleFa;
        public string descriptionFa;
        public long budgetRequiredToman;
        public float happinessImpact;
        public bool isPassed;
    }

    /// <summary>
    /// شورای اسلامی شهر گلبهار (۵ نفره) و لوایح شهرداری
    /// </summary>
    public class CityCouncilManager : MonoBehaviour
    {
        public static CityCouncilManager Instance { get; private set; }

        public List<CouncilMember> councilMembers = new List<CouncilMember>();
        public List<CouncilBill> activeBills = new List<CouncilBill>();

        public event Action<CouncilBill, bool> OnBillVoted;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            InitializeCouncil();
        }

        private void InitializeCouncil()
        {
            councilMembers.Clear();
            councilMembers.Add(new CouncilMember { nameFa = "مهندس رضوی", commission = "کمیسیون عمران و شهرسازی", loyalty = 75 });
            councilMembers.Add(new CouncilMember { nameFa = "دکتر جلالی", commission = "کمیسیون بودجه و سرمایه‌گذاری", loyalty = 60 });
            councilMembers.Add(new CouncilMember { nameFa = "خانم علیزاده", commission = "کمیسیون خدمات شهری و محیط زیست", loyalty = 85 });
            councilMembers.Add(new CouncilMember { nameFa = "حاج احمدی", commission = "کمیسیون فرهنگی و اجتماعی", loyalty = 68 });
            councilMembers.Add(new CouncilMember { nameFa = "مهندس کاوه", commission = "کمیسیون حمل و نقل و ترافیک", loyalty = 80 });
        }

        /// <summary>
        /// ارائه لایحه جدید به صحن شورا برای رای‌گیری
        /// </summary>
        public bool SubmitBillToCouncil(CouncilBill bill)
        {
            int yesVotes = 0;
            foreach (var member in councilMembers)
            {
                // بر اساس وفاداری عضو و بودجه شانس رای مثبت مشخص می‌شود
                float voteChance = member.loyalty - (bill.budgetRequiredToman > 50000000 ? 15 : 0);
                member.votesYes = UnityEngine.Random.Range(0f, 100f) < voteChance;
                if (member.votesYes) yesVotes++;
            }

            bool passed = yesVotes >= 3; // اکثریت ۳ از ۵
            bill.isPassed = passed;

            if (passed && EconomyManager.Instance != null)
            {
                if (EconomyManager.Instance.SpendTreasury(bill.budgetRequiredToman))
                {
                    EconomyManager.Instance.ModifyHappiness(bill.happinessImpact);
                }
                else
                {
                    passed = false;
                    Debug.LogWarning("[Council] لایحه تصویب شد اما کسری بودجه مانع اجرا گردید!");
                }
            }

            OnBillVoted?.Invoke(bill, passed);
            Debug.Log($"[Council] لایحه '{bill.titleFa}' با {yesVotes} موافق از ۵ نفر: {(passed ? "تصویب شد" : "رد شد")}");
            return passed;
        }
    }
}
