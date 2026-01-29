using System.Collections.Generic;
using UnityEngine;

namespace JunkyardClicker.Car
{
    using Core;

    public class Car : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _baseRenderer;

        [SerializeField]
        private List<CarPart> _parts = new List<CarPart>();

        private CarData _data;
        private int _currentHp;
        private int _destroyedPartsCount;

        public CarData Data => _data;
        public int CurrentHp => _currentHp;
        public int MaxHp => _data != null ? _data.MaxHp : 0;
        public float HpRatio => MaxHp > 0 ? (float)_currentHp / MaxHp : 0f;
        public bool IsDestroyed => _currentHp <= 0;

        public void Initialize(CarData data)
        {
            _data = data;
            _currentHp = data.MaxHp;
            _destroyedPartsCount = 0;

            if (_baseRenderer != null && data.BaseSprite != null)
            {
                _baseRenderer.sprite = data.BaseSprite;
            }

            InitializeParts();
            GameEvents.RaiseCarSpawned();
        }

        private void InitializeParts()
        {
            for (int i = 0; i < _parts.Count && i < _data.PartDataList.Length; i++)
            {
                CarPart part = _parts[i];
                CarPartData partData = _data.PartDataList[i];
                
                part.Initialize(partData, _data.MaxHp);
                part.OnDestroyed += HandlePartDestroyed;
            }
        }

        public void TakeDamage(int damage)
        {
            if (IsDestroyed)
            {
                return;
            }

            _currentHp -= damage;
            _currentHp = Mathf.Max(0, _currentHp);

            GameEvents.RaiseDamageDealt(damage);

            if (IsDestroyed)
            {
                HandleCarDestroyed();
            }
        }

        public void TakeDamageOnPart(CarPart targetPart, int damage)
        {
            if (IsDestroyed || targetPart == null)
            {
                return;
            }

            if (targetPart.IsDestroyed)
            {
                TakeDamage(damage);
                return;
            }

            int actualDamage = targetPart.TakeDamage(damage);
            _currentHp -= actualDamage;
            _currentHp = Mathf.Max(0, _currentHp);

            GameEvents.RaiseDamageDealt(actualDamage);

            if (IsDestroyed)
            {
                HandleCarDestroyed();
            }
        }

        private void HandlePartDestroyed(CarPart part)
        {
            _destroyedPartsCount++;
            part.OnDestroyed -= HandlePartDestroyed;
        }

        private void HandleCarDestroyed()
        {
            DestroyRemainingParts();
            int totalReward = CalculateTotalReward();
            GameEvents.RaiseCarDestroyed(totalReward);
        }

        private void DestroyRemainingParts()
        {
            foreach (CarPart part in _parts)
            {
                if (!part.IsDestroyed)
                {
                    part.TakeDamage(int.MaxValue);
                }
            }
        }

        private int CalculateTotalReward()
        {
            float gradeMultiplier = _data.Grade switch
            {
                CarGrade.Common => 1f,
                CarGrade.Rare => 2.5f,
                CarGrade.Epic => 5f,
                CarGrade.Legendary => 10f,
                _ => 1f
            };

            return Mathf.RoundToInt(_data.BaseReward * gradeMultiplier);
        }

        public CarPart GetPartAtPosition(Vector2 worldPosition)
        {
            foreach (CarPart part in _parts)
            {
                Collider2D partCollider = part.GetComponent<Collider2D>();
                if (partCollider != null && partCollider.OverlapPoint(worldPosition))
                {
                    return part;
                }
            }

            return null;
        }

        private void OnDestroy()
        {
            foreach (CarPart part in _parts)
            {
                if (part != null)
                {
                    part.OnDestroyed -= HandlePartDestroyed;
                }
            }
        }
    }
}
