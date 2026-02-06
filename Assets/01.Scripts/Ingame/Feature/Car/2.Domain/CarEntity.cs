using System;
using System.Collections.Generic;
using UnityEngine;

namespace JunkyardClicker.Car
{
    using JunkyardClicker.Core;

    /// <summary>
    /// 차량 클래스 - MonoBehaviour와 도메인 로직의 연결
    /// </summary>
    public class CarEntity : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _baseRenderer;

        [SerializeField]
        private List<CarPartEntity> _parts = new List<CarPartEntity>();

        private CarData _data;
        private CarState _state;

        public CarData Data => _data;
        public int CurrentHp => _state?.CurrentHp ?? 0;
        public int MaxHp => _data != null ? _data.MaxHp : 0;
        public float HpRatio => _state?.HpRatio ?? 0f;
        public bool IsDestroyed => _state?.IsDestroyed ?? true;

        public event Action<CarEntity, int> OnDestroyed;
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
                CarPartEntity part = _parts[i];
                CarPartData partData = _data.PartDataList[i];

                part.Initialize(partData, _data.MaxHp);
                part.OnDestroyed += HandlePartDestroyed;
            }
        }

        /// <summary>
        /// 차량에 데미지 적용
        /// 주의: 이벤트 발행은 DamageManager가 담당 (단일 책임)
        /// </summary>
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
                // GameEvents.RaiseDamageDealt 제거 - DamageManager에서만 발행 (중복 방지)
            }

            if (IsDestroyed)
            {
                HandleCarDestroyed();
            }

            return actualDamage;
        }

        /// <summary>
        /// 특정 파츠에 데미지 적용
        /// 주의: 이벤트 발행은 DamageManager가 담당 (단일 책임)
        /// </summary>
        public int TakeDamageOnPart(CarPartEntity targetPart, int damage)
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
                // GameEvents.RaiseDamageDealt 제거 - DamageManager에서만 발행 (중복 방지)
            }

            if (IsDestroyed)
            {
                HandleCarDestroyed();
            }

            return actualDamage;
        }

        private void HandlePartDestroyed(CarPartEntity part)
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
            foreach (CarPartEntity part in _parts)
            {
                if (!part.IsDestroyed)
                {
                    part.TakeDamage(int.MaxValue);
                }
            }
        }

        public CarPartEntity GetPartAtPosition(Vector2 worldPosition)
        {
            foreach (CarPartEntity part in _parts)
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
            foreach (CarPartEntity part in _parts)
            {
                if (part != null)
                {
                    part.OnDestroyed -= HandlePartDestroyed;
                }
            }
        }
    }
}
