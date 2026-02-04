using System;

namespace JunkyardClicker.Core
{
    using Car;

    /// <summary>
    /// 게임 이벤트 시스템
    /// 레거시 static 이벤트와 새로운 EventBus를 모두 지원
    /// 점진적으로 EventBus로 마이그레이션 권장
    /// </summary>
    public static class GameEvents
    {
        #region Legacy Events (하위 호환성)

        // Car 참조를 직접 전달하여 타이밍 문제 해결
        public static event Action<Car> OnCarSpawned;

        public static event Action<int> OnDamageDealt;
        public static event Action<CarPartType, int> OnPartDamaged;
        public static event Action<CarPartType> OnPartDestroyed;
        public static event Action<int> OnCarDestroyed;
        public static event Action<int> OnMoneyChanged;
        public static event Action<PartType, int> OnPartCollected;

        #endregion

        #region Event Raise Methods (Legacy + EventBus 동시 발행)

        public static void RaiseCarSpawned(Car car)
        {
            OnCarSpawned?.Invoke(car);
            EventBus.Publish(new CarSpawnedEvent(car));
        }

        public static void RaiseDamageDealt(int damage)
        {
            OnDamageDealt?.Invoke(damage);
            EventBus.Publish(new DamageDealtEvent(damage));
        }

        public static void RaisePartDamaged(CarPartType partType, int currentHp)
        {
            OnPartDamaged?.Invoke(partType, currentHp);
            EventBus.Publish(new PartDamagedEvent(partType, currentHp));
        }

        public static void RaisePartDestroyed(CarPartType partType)
        {
            OnPartDestroyed?.Invoke(partType);
            EventBus.Publish(new PartDestroyedEvent(partType));
        }

        public static void RaiseCarDestroyed(int reward)
        {
            OnCarDestroyed?.Invoke(reward);
            EventBus.Publish(new CarDestroyedEvent(reward));
        }

        public static void RaiseMoneyChanged(int totalMoney)
        {
            OnMoneyChanged?.Invoke(totalMoney);
            EventBus.Publish(new CurrencyChangedEvent(ECurrencyType.Money, totalMoney));
        }

        public static void RaisePartCollected(PartType partType, int amount)
        {
            OnPartCollected?.Invoke(partType, amount);
            EventBus.Publish(new PartCollectedEvent(partType, amount));
        }

        #endregion

        #region Cleanup

        public static void ClearAllEvents()
        {
            OnCarSpawned = null;
            OnDamageDealt = null;
            OnPartDamaged = null;
            OnPartDestroyed = null;
            OnCarDestroyed = null;
            OnMoneyChanged = null;
            OnPartCollected = null;

            EventBus.Clear();
        }

        #endregion
    }
}
