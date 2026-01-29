using System;
using System.Collections.Generic;
using UnityEngine;

namespace JunkyardClicker.Upgrade
{
    using Core;
    using Resource;

    public class UpgradeManager : MonoBehaviour
    {
        [SerializeField]
        private UpgradeData[] _upgradeDataList;

        [SerializeField]
        private ResourceManager _resourceManager;

        private Dictionary<UpgradeType, int> _levels = new Dictionary<UpgradeType, int>();
        private Dictionary<UpgradeType, UpgradeData> _dataMap = new Dictionary<UpgradeType, UpgradeData>();

        public event Action<UpgradeType, int> OnLevelChanged;

        private void Awake()
        {
            InitializeUpgrades();
        }

        private void InitializeUpgrades()
        {
            _levels.Clear();
            _dataMap.Clear();

            foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
            {
                _levels[type] = 0;
            }

            foreach (UpgradeData data in _upgradeDataList)
            {
                if (data != null)
                {
                    _dataMap[data.Type] = data;
                }
            }
        }

        public int GetLevel(UpgradeType type)
        {
            return _levels.TryGetValue(type, out int level) ? level : 0;
        }

        public int GetCurrentValue(UpgradeType type)
        {
            if (!_dataMap.TryGetValue(type, out UpgradeData data))
            {
                return 0;
            }

            int level = GetLevel(type);
            return data.GetValue(level);
        }

        public int GetUpgradeCost(UpgradeType type)
        {
            if (!_dataMap.TryGetValue(type, out UpgradeData data))
            {
                return int.MaxValue;
            }

            int level = GetLevel(type);
            return data.GetCost(level);
        }

        public bool CanUpgrade(UpgradeType type)
        {
            if (!_dataMap.TryGetValue(type, out UpgradeData data))
            {
                return false;
            }

            int level = GetLevel(type);
            
            if (data.IsMaxLevel(level))
            {
                return false;
            }

            int cost = data.GetCost(level);
            return _resourceManager.Money >= cost;
        }

        public bool TryUpgrade(UpgradeType type)
        {
            if (!CanUpgrade(type))
            {
                return false;
            }

            int cost = GetUpgradeCost(type);
            
            if (!_resourceManager.SpendMoney(cost))
            {
                return false;
            }

            _levels[type]++;
            int newLevel = _levels[type];

            OnLevelChanged?.Invoke(type, newLevel);
            GameEvents.RaiseUpgraded(type, newLevel);

            return true;
        }

        public bool IsMaxLevel(UpgradeType type)
        {
            if (!_dataMap.TryGetValue(type, out UpgradeData data))
            {
                return true;
            }

            return data.IsMaxLevel(GetLevel(type));
        }

        public UpgradeData GetUpgradeData(UpgradeType type)
        {
            return _dataMap.TryGetValue(type, out UpgradeData data) ? data : null;
        }

        public string GetCurrentLevelName(UpgradeType type)
        {
            if (!_dataMap.TryGetValue(type, out UpgradeData data))
            {
                return "???";
            }

            return data.GetLevelName(GetLevel(type));
        }

        public void SetLevel(UpgradeType type, int level)
        {
            _levels[type] = Mathf.Max(0, level);
            OnLevelChanged?.Invoke(type, _levels[type]);
        }

        public Dictionary<UpgradeType, int> GetAllLevels()
        {
            return new Dictionary<UpgradeType, int>(_levels);
        }
    }
}
