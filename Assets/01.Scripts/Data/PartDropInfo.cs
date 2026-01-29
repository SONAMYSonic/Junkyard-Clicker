using System;
using UnityEngine;

namespace JunkyardClicker.Core
{
    [Serializable]
    public class PartDropInfo
    {
        [SerializeField]
        private PartType _partType;

        [SerializeField]
        private int _minAmount;

        [SerializeField]
        private int _maxAmount;

        [SerializeField]
        [Range(0f, 1f)]
        private float _dropChance;

        public PartType PartType => _partType;
        public int MinAmount => _minAmount;
        public int MaxAmount => _maxAmount;
        public float DropChance => _dropChance;

        public int GetRandomAmount()
        {
            return UnityEngine.Random.Range(_minAmount, _maxAmount + 1);
        }

        public bool RollDrop()
        {
            return UnityEngine.Random.value <= _dropChance;
        }
    }
}
