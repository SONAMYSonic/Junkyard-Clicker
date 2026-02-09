using System;
using System.Collections.Generic;
using UnityEngine;

namespace JunkyardClicker.Car
{
    using JunkyardClicker.Core;

    // 차량 클래스 - MonoBehaviour와 도메인 로직의 연결
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

        // 차량 데이터로 초기화
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

        // 모든 파츠 초기화
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

        // 차량에 데미지 적용 (이벤트 발행은 DamageManager가 담당)
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

        // 특정 파츠에 데미지 적용 (이벤트 발행은 DamageManager가 담당)
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

        // 파츠 파괴 시 처리
        private void HandlePartDestroyed(CarPartEntity part)
        {
            _state.IncrementDestroyedParts();
            part.OnDestroyed -= HandlePartDestroyed;
        }

        // 차량 파괴 시 처리
        private void HandleCarDestroyed()
        {
            DestroyRemainingParts();
            int totalReward = CarRewardCalculator.CalculateReward(_data);

            OnDestroyed?.Invoke(this, totalReward);
            GameEvents.RaiseCarDestroyed(totalReward);
        }

        // 남은 파츠 모두 파괴
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

        // 월드 좌표에 해당하는 파츠 반환
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

        // 오브젝트 파괴 시 이벤트 구독 해제
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
