using UnityEngine;

namespace JunkyardClicker.Ingame.Car
{
    using JunkyardClicker.Core;

    /// <summary>
    /// 차량 스폰 선택 로직을 담당하는 도메인 서비스
    /// 가중치 기반 랜덤 선택
    /// </summary>
    public class CarSpawnSelector
    {
        private readonly CarData[] _carDataList;
        private readonly float _totalWeight;

        public CarSpawnSelector(CarData[] carDataList)
        {
            _carDataList = carDataList;
            _totalWeight = CalculateTotalWeight();
        }

        private float CalculateTotalWeight()
        {
            float total = 0f;

            foreach (CarData data in _carDataList)
            {
                if (data != null)
                {
                    total += data.GetSpawnWeight();
                }
            }

            return total;
        }

        public CarData SelectRandom()
        {
            if (_carDataList == null || _carDataList.Length == 0)
            {
                return null;
            }

            float randomValue = Random.Range(0f, _totalWeight);
            float currentWeight = 0f;

            foreach (CarData data in _carDataList)
            {
                if (data == null)
                {
                    continue;
                }

                currentWeight += data.GetSpawnWeight();

                if (randomValue <= currentWeight)
                {
                    return data;
                }
            }

            return _carDataList[0];
        }
    }
}
