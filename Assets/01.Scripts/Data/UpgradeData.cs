using UnityEngine;

namespace JunkyardClicker.Core
{
    [CreateAssetMenu(fileName = "NewUpgradeData", menuName = "JunkyardClicker/Upgrade Data")]
    public class UpgradeData : ScriptableObject
    {
        [SerializeField]
        private UpgradeType _type;

        [SerializeField]
        private string _displayName;

        [SerializeField]
        private string[] _levelNames;

        [SerializeField]
        private int[] _costs;

        [SerializeField]
        private int[] _values;

        [SerializeField]
        private Sprite[] _icons;

        public UpgradeType Type => _type;
        public string DisplayName => _displayName;
        public int MaxLevel => _costs.Length;

        public string GetLevelName(int level)
        {
            if (level < 0 || level >= _levelNames.Length)
            {
                return "???";
            }

            return _levelNames[level];
        }

        public int GetCost(int currentLevel)
        {
            if (currentLevel < 0 || currentLevel >= _costs.Length)
            {
                return int.MaxValue;
            }

            return _costs[currentLevel];
        }

        public int GetValue(int level)
        {
            if (level < 0 || level >= _values.Length)
            {
                return _values.Length > 0 ? _values[^1] : 0;
            }

            return _values[level];
        }

        public Sprite GetIcon(int level)
        {
            if (_icons == null || _icons.Length == 0)
            {
                return null;
            }

            if (level < 0 || level >= _icons.Length)
            {
                return _icons[^1];
            }

            return _icons[level];
        }

        public bool IsMaxLevel(int level)
        {
            return level >= MaxLevel;
        }
    }
}
