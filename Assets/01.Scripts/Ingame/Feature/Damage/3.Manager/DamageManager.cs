using System;
using UnityEngine;

namespace JunkyardClicker.Ingame.Damage
{
    using JunkyardClicker.Core;
    using JunkyardClicker.Ingame.Car;

    /// <summary>
    /// 데미지 시스템 매니저
    /// 클릭 및 자동 데미지 처리를 담당
    /// </summary>
    public class DamageManager : MonoBehaviour, IDamageManager
    {
        public static DamageManager Instance { get; private set; }

        private IDamageCalculator _damageCalculator;
        private ICarManager _carManager;

        public event Action<DamageInfo> OnDamageApplied;

        private void Awake()
        {
            SetupSingleton();
            _damageCalculator = new UpgradeBasedDamageCalculator();
            ServiceLocator.Register<IDamageManager>(this);
        }

        private void SetupSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            if (ServiceLocator.TryGet<ICarManager>(out var carManager))
            {
                _carManager = carManager;
            }
            else
            {
                _carManager = CarManager.Instance;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                ServiceLocator.Unregister<IDamageManager>();
            }
        }

        public void ApplyClickDamage(Vector2 worldPosition)
        {
            if (_carManager == null || !_carManager.HasActiveCar)
            {
                return;
            }

            int damage = _damageCalculator.CalculateClickDamage();
            var damageInfo = new DamageInfo(damage, worldPosition, DamageSource.Click);

            _carManager.ApplyDamageAtPosition(damage, worldPosition);

            OnDamageApplied?.Invoke(damageInfo);
        }

        public void ApplyAutoDamage()
        {
            if (_carManager == null || !_carManager.HasActiveCar)
            {
                return;
            }

            int damage = _damageCalculator.CalculateAutoDamage();

            if (damage <= 0)
            {
                return;
            }

            var damageInfo = new DamageInfo(damage, DamageSource.Auto);

            _carManager.ApplyDamage(damage);

            OnDamageApplied?.Invoke(damageInfo);
        }

        public void SetDamageCalculator(IDamageCalculator calculator)
        {
            _damageCalculator = calculator;
        }
    }
}
