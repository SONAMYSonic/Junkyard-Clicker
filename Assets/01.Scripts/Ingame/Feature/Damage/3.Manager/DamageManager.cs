using System;
using UnityEngine;

namespace JunkyardClicker.Resource
{
    using JunkyardClicker.Core;
    using JunkyardClicker.Car;
    using CarEntity = JunkyardClicker.Car.Car;

    /// <summary>
    /// 데미지 시스템 매니저
    /// 모든 데미지 처리를 담당 (SRP - 데미지 전용)
    /// CarManager와 분리되어 단일 책임을 가짐
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
            InitializeDependencies();
        }

        private void InitializeDependencies()
        {
            // CarManager 의존성 주입
            if (ServiceLocator.TryGet<ICarManager>(out var carManager))
            {
                _carManager = carManager;
            }
            else
            {
                _carManager = CarSpawner.Instance;
            }

            // DamageCalculator 의존성 주입 (업그레이드 서비스 주입)
            if (ServiceLocator.TryGet<IUpgradeService>(out var upgradeService))
            {
                _damageCalculator = new UpgradeBasedDamageCalculator(upgradeService);
            }
            else
            {
                // 폴백: 기존 방식 (하위 호환성)
                _damageCalculator = new UpgradeBasedDamageCalculator();
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
            CarEntity currentCar = GetCurrentCar();

            if (currentCar == null)
            {
                return;
            }

            int damage = _damageCalculator.CalculateClickDamage();
            var damageInfo = new DamageInfo(damage, worldPosition, DamageSource.Click);

            // 직접 Car에 데미지 적용 (CarManager를 거치지 않음)
            ApplyDamageAtPosition(currentCar, damage, worldPosition);

            OnDamageApplied?.Invoke(damageInfo);
            GameEvents.RaiseDamageDealt(damage);
        }

        public void ApplyAutoDamage()
        {
            CarEntity currentCar = GetCurrentCar();

            if (currentCar == null)
            {
                return;
            }

            int damage = _damageCalculator.CalculateAutoDamage();

            if (damage <= 0)
            {
                return;
            }

            var damageInfo = new DamageInfo(damage, DamageSource.Auto);

            // 직접 Car에 데미지 적용
            currentCar.TakeDamage(damage);

            OnDamageApplied?.Invoke(damageInfo);
            GameEvents.RaiseDamageDealt(damage);
        }

        private CarEntity GetCurrentCar()
        {
            if (_carManager == null || !_carManager.HasActiveCar)
            {
                return null;
            }

            return _carManager.CurrentCar;
        }

        private void ApplyDamageAtPosition(CarEntity car, int damage, Vector2 worldPosition)
        {
            CarPart clickedPart = car.GetPartAtPosition(worldPosition);

            if (clickedPart != null)
            {
                car.TakeDamageOnPart(clickedPart, damage);
            }
            else
            {
                car.TakeDamage(damage);
            }
        }

        public void SetDamageCalculator(IDamageCalculator calculator)
        {
            _damageCalculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        }
    }
}
