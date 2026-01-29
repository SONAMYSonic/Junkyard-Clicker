using System;
using UnityEngine;

namespace JunkyardClicker.Car
{
    using Core;

    public class CarPart : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        private CarPartData _data;
        private int _maxHp;
        private int _currentHp;
        private bool _isDestroyed;

        public CarPartType PartType => _data.PartType;
        public bool IsDestroyed => _isDestroyed;
        public float HpRatio => _maxHp > 0 ? (float)_currentHp / _maxHp : 0f;

        public event Action<CarPart> OnDestroyed;

        public void Initialize(CarPartData data, int carMaxHp)
        {
            _data = data;
            _maxHp = data.CalculateMaxHp(carMaxHp);
            _currentHp = _maxHp;
            _isDestroyed = false;

            UpdateVisual();
        }

        public int TakeDamage(int damage)
        {
            if (_isDestroyed)
            {
                return 0;
            }

            int actualDamage = Mathf.Min(damage, _currentHp);
            _currentHp -= actualDamage;

            GameEvents.RaisePartDamaged(_data.PartType, _currentHp);
            UpdateVisual();

            if (_currentHp <= 0)
            {
                Destroy();
            }

            return actualDamage;
        }

        private void Destroy()
        {
            _isDestroyed = true;
            DropParts();
            GameEvents.RaisePartDestroyed(_data.PartType);
            OnDestroyed?.Invoke(this);
        }

        private void DropParts()
        {
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

            if (_isDestroyed)
            {
                _spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
            }
        }

        public void ResetPart()
        {
            _currentHp = _maxHp;
            _isDestroyed = false;
            
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.white;
            }
            
            UpdateVisual();
        }
    }
}
