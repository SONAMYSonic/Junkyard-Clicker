using System;

namespace JunkyardClicker.Core
{
    using Car;

    public static class GameEvents
    {
        // Car 참조를 직접 전달하여 타이밍 문제 해결
        public static event Action<Car> OnCarSpawned;

        public static event Action<int> OnDamageDealt;
        public static event Action<CarPartType, int> OnPartDamaged;
        public static event Action<CarPartType> OnPartDestroyed;
        public static event Action<int> OnCarDestroyed;
        public static event Action<int> OnMoneyChanged;
        public static event Action<PartType, int> OnPartCollected;

        public static void RaiseCarSpawned(Car car)
        {
            OnCarSpawned?.Invoke(car);
        }

        public static void RaiseDamageDealt(int damage)
        {
            OnDamageDealt?.Invoke(damage);
        }

        public static void RaisePartDamaged(CarPartType partType, int currentHp)
        {
            OnPartDamaged?.Invoke(partType, currentHp);
        }

        public static void RaisePartDestroyed(CarPartType partType)
        {
            OnPartDestroyed?.Invoke(partType);
        }

        public static void RaiseCarDestroyed(int reward)
        {
            OnCarDestroyed?.Invoke(reward);
        }

        public static void RaiseMoneyChanged(int totalMoney)
        {
            OnMoneyChanged?.Invoke(totalMoney);
        }

        public static void RaisePartCollected(PartType partType, int amount)
        {
            OnPartCollected?.Invoke(partType, amount);
        }

        public static void ClearAllEvents()
        {
            OnCarSpawned = null;
            OnDamageDealt = null;
            OnPartDamaged = null;
            OnPartDestroyed = null;
            OnCarDestroyed = null;
            OnMoneyChanged = null;
            OnPartCollected = null;
        }
    }
}
