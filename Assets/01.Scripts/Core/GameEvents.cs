using System;

namespace JunkyardClicker.Core
{
    using Car;

    /// <summary>
    /// 게임 이벤트 시스템
    /// static 이벤트 기반의 간단한 Pub/Sub 패턴
    /// </summary>
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

        public static void RaiseCarSpawned(CarEntity car)
        {
            OnCarSpawned?.Invoke(car);
        }

        public static void RaiseDamageDealt(int damage)
        {
            OnDamageDealt?.Invoke(damage);
        }

        public static void RaisePartDestroyed(CarPartType partType)
        {
            OnPartDestroyed?.Invoke(partType);
        }

        public static void RaiseCarDestroyed(int reward)
        {
            OnCarDestroyed?.Invoke(reward);
        }

        public static void RaisePartCollected(PartType partType, int amount)
        {
            OnPartCollected?.Invoke(partType, amount);
        }

        #endregion
    }
}
