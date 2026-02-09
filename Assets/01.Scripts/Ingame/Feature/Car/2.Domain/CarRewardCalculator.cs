using UnityEngine;

namespace JunkyardClicker.Car
{
    using JunkyardClicker.Core;

    // 차량 보상 계산 도메인 서비스
    public static class CarRewardCalculator
    {
        // 차량 파괴 보상 계산
        public static int CalculateReward(CarData carData)
        {
            if (carData == null)
            {
                return 0;
            }

            float gradeMultiplier = GetGradeMultiplier(carData.Grade);
            return Mathf.RoundToInt(carData.BaseReward * gradeMultiplier);
        }

        // 등급별 보상 배율 반환
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
