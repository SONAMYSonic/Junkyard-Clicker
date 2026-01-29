using System;

namespace JunkyardClicker.Core
{
    public static class GameEvents
    {
        public static event Action<int> OnDamageDealt;
        public static event Action<CarPartType, int> OnPartDamaged;
        public static event Action<CarPartType> OnPartDestroyed;
        public static event Action<int> OnCarDestroyed;
        public static event Action<int> OnMoneyChanged;
        public static event Action<PartType, int> OnPartCollected;
        public static event Action<UpgradeType, int> OnUpgraded;
        public static event Action OnCarSpawned;

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

        public static void RaiseUpgraded(UpgradeType upgradeType, int newLevel)
        {
            OnUpgraded?.Invoke(upgradeType, newLevel);
        }

        public static void RaiseCarSpawned()
        {
            OnCarSpawned?.Invoke();
        }

        public static void ClearAllEvents()
        {
            OnDamageDealt = null;
            OnPartDamaged = null;
            OnPartDestroyed = null;
            OnCarDestroyed = null;
            OnMoneyChanged = null;
            OnPartCollected = null;
            OnUpgraded = null;
            OnCarSpawned = null;
        }
    }
}
