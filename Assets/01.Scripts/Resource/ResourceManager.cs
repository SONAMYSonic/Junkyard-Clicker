using System;
using System.Collections.Generic;
using UnityEngine;

namespace JunkyardClicker.Resource
{
    using Core;

    public class ResourceManager : MonoBehaviour
    {
        private int _money;
        private Dictionary<PartType, int> _parts = new Dictionary<PartType, int>();

        public int Money => _money;

        public event Action<int> OnMoneyUpdated;
        public event Action<PartType, int> OnPartUpdated;

        private void Awake()
        {
            InitializePartsDictionary();
        }

        private void OnEnable()
        {
            GameEvents.OnCarDestroyed += HandleCarDestroyed;
            GameEvents.OnPartCollected += HandlePartCollected;
        }

        private void OnDisable()
        {
            GameEvents.OnCarDestroyed -= HandleCarDestroyed;
            GameEvents.OnPartCollected -= HandlePartCollected;
        }

        private void InitializePartsDictionary()
        {
            _parts.Clear();
            foreach (PartType partType in Enum.GetValues(typeof(PartType)))
            {
                _parts[partType] = 0;
            }
        }

        public void AddMoney(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            _money += amount;
            OnMoneyUpdated?.Invoke(_money);
            GameEvents.RaiseMoneyChanged(_money);
        }

        public bool SpendMoney(int amount)
        {
            if (amount <= 0 || _money < amount)
            {
                return false;
            }

            _money -= amount;
            OnMoneyUpdated?.Invoke(_money);
            GameEvents.RaiseMoneyChanged(_money);
            return true;
        }

        public void AddPart(PartType partType, int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            _parts[partType] += amount;
            OnPartUpdated?.Invoke(partType, _parts[partType]);
        }

        public int GetPartCount(PartType partType)
        {
            return _parts.TryGetValue(partType, out int count) ? count : 0;
        }

        public int SellAllParts()
        {
            int totalValue = CalculateTotalPartValue();
            
            if (totalValue <= 0)
            {
                return 0;
            }

            AddMoney(totalValue);
            ClearAllParts();
            
            return totalValue;
        }

        public int SellPart(PartType partType, int amount)
        {
            int available = GetPartCount(partType);
            int sellAmount = Mathf.Min(amount, available);
            
            if (sellAmount <= 0)
            {
                return 0;
            }

            int value = GetPartValue(partType) * sellAmount;
            _parts[partType] -= sellAmount;
            AddMoney(value);
            OnPartUpdated?.Invoke(partType, _parts[partType]);
            
            return value;
        }

        private int CalculateTotalPartValue()
        {
            int total = 0;
            foreach (KeyValuePair<PartType, int> pair in _parts)
            {
                total += GetPartValue(pair.Key) * pair.Value;
            }
            return total;
        }

        private void ClearAllParts()
        {
            foreach (PartType partType in Enum.GetValues(typeof(PartType)))
            {
                _parts[partType] = 0;
                OnPartUpdated?.Invoke(partType, 0);
            }
        }

        private int GetPartValue(PartType partType)
        {
            return partType switch
            {
                PartType.Scrap => 5,
                PartType.Glass => 3,
                PartType.Plate => 8,
                PartType.Rubber => 4,
                _ => 1
            };
        }

        private void HandleCarDestroyed(int reward)
        {
            AddMoney(reward);
        }

        private void HandlePartCollected(PartType partType, int amount)
        {
            AddPart(partType, amount);
        }

        public void SetMoney(int amount)
        {
            _money = Mathf.Max(0, amount);
            OnMoneyUpdated?.Invoke(_money);
            GameEvents.RaiseMoneyChanged(_money);
        }

        public void SetParts(Dictionary<PartType, int> parts)
        {
            foreach (KeyValuePair<PartType, int> pair in parts)
            {
                _parts[pair.Key] = pair.Value;
                OnPartUpdated?.Invoke(pair.Key, pair.Value);
            }
        }
    }
}
