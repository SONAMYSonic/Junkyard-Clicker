using System;
using UnityEngine;

namespace JunkyardClicker.Car
{
    using JunkyardClicker.Core;

    // 차량 파츠 클래스 - MonoBehaviour와 도메인 로직의 연결
    public class CarPartEntity : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        private CarPartData _data;
        private CarPartState _state;

        public bool IsInitialized => _data != null;
        public CarPartType PartType => _data != null ? _data.PartType : CarPartType.Body;
        public bool IsDestroyed => _state?.IsDestroyed ?? true;
        public float HpRatio => _state?.HpRatio ?? 0f;

        public event Action<CarPartEntity> OnDestroyed;

        // 파츠 데이터로 초기화
        public void Initialize(CarPartData data, int carMaxHp)
        {
            _data = data;
            int partMaxHp = data.CalculateMaxHp(carMaxHp);
            _state = new CarPartState(data.PartType, partMaxHp);

            UpdateVisual();
        }

        // 데미지를 받고 실제 적용된 데미지를 반환
        public int TakeDamage(int damage)
        {
            if (IsDestroyed)
            {
                return 0;
            }

            if (_data == null)
            {
                Debug.LogWarning($"CarPartEntity '{gameObject.name}'이 초기화되지 않았습니다.");
                return 0;
            }

            int actualDamage = _state.ApplyDamage(damage);
            UpdateVisual();

            if (_state.IsDestroyed)
            {
                HandleDestroyed();
            }

            return actualDamage;
        }

        // 파츠 파괴 시 처리
        private void HandleDestroyed()
        {
            DropParts();

            if (_data != null)
            {
                GameEvents.RaisePartDestroyed(_data.PartType);
            }

            OnDestroyed?.Invoke(this);
        }

        // 파츠 드롭 아이템 처리
        private void DropParts()
        {
            if (_data == null || _data.Drops == null)
            {
                return;
            }

            foreach (PartDropInfo dropInfo in _data.Drops)
            {
                if (dropInfo.RollDrop())
                {
                    int amount = dropInfo.GetRandomAmount();
                    GameEvents.RaisePartCollected(dropInfo.PartType, amount);
                }
            }
        }

        // HP 비율에 따라 스프라이트 업데이트
        private void UpdateVisual()
        {
            if (_spriteRenderer == null || _data == null)
            {
                return;
            }

            Sprite newSprite = _data.GetSpriteForState(HpRatio);
            if (newSprite != null)
            {
                _spriteRenderer.sprite = newSprite;
            }

            if (IsDestroyed)
            {
                _spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
            }
        }

        // 파츠 상태 리셋
        public void ResetPart()
        {
            if (_state != null)
            {
                _state.Reset(_state.MaxHp);
            }

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.white;
            }

            UpdateVisual();
        }
    }
}
