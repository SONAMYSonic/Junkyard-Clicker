using System;

namespace JunkyardClicker.Core
{
    using Car;

    // 게임 이벤트 시스템 - static 이벤트 기반 Pub/Sub 패턴
    public static class GameEvents
    {
        #region Events

        // Car 참조를 직접 전달하여 타이밍 문제 해결
        public static event Action<CarEntity> OnCarSpawned;

        public static event Action<int> OnDamageDealt;
        public static event Action<CarPartType> OnPartDestroyed;
        public static event Action<int> OnCarDestroyed;
        public static event Action<PartType, int> OnPartCollected;

        #endregion

        #region Event Raise Methods

        // 차량 스폰 이벤트 발생
        public static void RaiseCarSpawned(CarEntity car)
        {
            OnCarSpawned?.Invoke(car);
        }

        // 데미지 이벤트 발생
        public static void RaiseDamageDealt(int damage)
        {
            OnDamageDealt?.Invoke(damage);
        }

        // 파츠 파괴 이벤트 발생
        public static void RaisePartDestroyed(CarPartType partType)
        {
            OnPartDestroyed?.Invoke(partType);
        }

        // 차량 파괴 이벤트 발생
        public static void RaiseCarDestroyed(int reward)
        {
            OnCarDestroyed?.Invoke(reward);
        }

        // 파츠 수집 이벤트 발생
        public static void RaisePartCollected(PartType partType, int amount)
        {
            OnPartCollected?.Invoke(partType, amount);
        }

        #endregion
    }
}
