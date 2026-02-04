using System;
using System.Collections.Generic;
using UnityEngine;

namespace JunkyardClicker.Car
{
    using JunkyardClicker.Core;

    /// <summary>
    /// 차량 클래스 - MonoBehaviour와 도메인 로직의 연결
    /// </summary>
    public class Car : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _baseRenderer;

        [SerializeField]
        private List<CarPart> _parts = new List<CarPart>();

        private CarData _data;
        private CarState _state;

        public CarData Data => _data;
        public int CurrentHp => _state?.CurrentHp ?? 0;
        public int MaxHp => _data != null ? _data.MaxHp : 0;
        public float HpRatio => _state?.HpRatio ?? 0f;
        public bool IsDestroyed => _state?.IsDestroyed ?? true;

        public event Action<Car, int> OnDestroyed;
        public event Action<int> OnDamageReceived;

        public void Initialize(CarData data)
        {
            _data = data;
            _state = new CarState(data.MaxHp);

            if (_baseRenderer != null && data.BaseSprite != null)
            {
                _baseRenderer.sprite = data.BaseSprite;
            }

            InitializeParts();
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

        public int TakeDamage(int damage)
        {
            if (IsDestroyed)
            {
                return 0;
            }

            int actualDamage = _state.ApplyDamage(damage);

            if (actualDamage > 0)
            {
                OnDamageReceived?.Invoke(actualDamage);
                GameEvents.RaiseDamageDealt(actualDamage);
            }

            if (IsDestroyed)
            {
                HandleCarDestroyed();
            }

            return actualDamage;
        }

        public int TakeDamageOnPart(CarPart targetPart, int damage)
        {
            if (IsDestroyed || targetPart == null)
            {
                return 0;
            }

            if (targetPart.IsDestroyed)
            {
                return TakeDamage(damage);
            }

            int actualDamage = targetPart.TakeDamage(damage);
            _state.ApplyDamage(actualDamage);

            if (actualDamage > 0)
            {
                OnDamageReceived?.Invoke(actualDamage);
                GameEvents.RaiseDamageDealt(actualDamage);
            }

            if (IsDestroyed)
            {
                HandleCarDestroyed();
            }

            return actualDamage;
        }

        private void HandlePartDestroyed(CarPart part)
        {
            _state.IncrementDestroyedParts();
            part.OnDestroyed -= HandlePartDestroyed;
        }

        private void HandleCarDestroyed()
        {
            DestroyRemainingParts();
            int totalReward = CarRewardCalculator.CalculateReward(_data);

            OnDestroyed?.Invoke(this, totalReward);
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
