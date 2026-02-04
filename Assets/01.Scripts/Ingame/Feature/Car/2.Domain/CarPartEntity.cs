using System;
using UnityEngine;

namespace JunkyardClicker.Ingame.Car
{
    using JunkyardClicker.Core;

    /// <summary>
    /// 차량 파츠 엔티티 - MonoBehaviour와 도메인 로직의 연결
    /// 기존 CarPart.cs를 대체하는 새로운 구현
    /// </summary>
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

        public void Initialize(CarPartData data, int carMaxHp)
        {
            _data = data;
            int partMaxHp = data.CalculateMaxHp(carMaxHp);
            _state = new CarPartState(data.PartType, partMaxHp);

            UpdateVisual();
        }

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

            GameEvents.RaisePartDamaged(_data.PartType, _state.CurrentHp);
            UpdateVisual();

            if (_state.IsDestroyed)
            {
                HandleDestroyed();
            }

            return actualDamage;
        }

        private void HandleDestroyed()
        {
            DropParts();

            if (_data != null)
            {
                GameEvents.RaisePartDestroyed(_data.PartType);
            }

            OnDestroyed?.Invoke(this);
        }

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
