using UnityEngine;

namespace JunkyardClicker.Ingame.Car
{
    using JunkyardClicker.Core;

    /// <summary>
    /// 차량 보상 계산 로직을 담당하는 도메인 서비스
    /// </summary>
    public static class CarRewardCalculator
    {
        public static int CalculateReward(CarData carData)
        {
            if (carData == null)
            {
                return 0;
            }

            float gradeMultiplier = GetGradeMultiplier(carData.Grade);
            return Mathf.RoundToInt(carData.BaseReward * gradeMultiplier);
        }

        public static float GetGradeMultiplier(CarGrade grade)
        {
            return grade switch
            {
                CarGrade.Common => 1f,
                CarGrade.Rare => 2.5f,
                CarGrade.Epic => 5f,
                CarGrade.Legendary => 10f,
                _ => 1f
            };
        }
    }
}
