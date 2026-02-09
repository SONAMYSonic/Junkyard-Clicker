using System;
using UnityEngine;

namespace JunkyardClicker.Resource
{
    using JunkyardClicker.Core;
    using JunkyardClicker.Car;
    using CarEntity = JunkyardClicker.Car.CarEntity;
    using CarPartEntity = JunkyardClicker.Car.CarPartEntity;

    // 데미지 시스템 매니저 - 모든 데미지 처리를 담당
    public class DamageManager : MonoBehaviour, IDamageManager
    {
        public static DamageManager Instance { get; private set; }

        private IDamageCalculator _damageCalculator;
        private ICarManager _carManager;

        public event Action<DamageInfo> OnDamageApplied;

        // 싱글톤 설정 및 서비스 등록
        private void Awake()
        {
            SetupSingleton();
            ServiceLocator.Register<IDamageManager>(this);
        }

        // 싱글톤 인스턴스 설정
        private void SetupSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        // 의존성 초기화
        private void Start()
        {
            InitializeDependencies();
        }

        // CarManager와 DamageCalculator 의존성 주입
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

        // 오브젝트 파괴 시 정리
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                ServiceLocator.Unregister<IDamageManager>();
            }
        }

        // 클릭 데미지 적용
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

        // 자동 데미지 적용
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

        // 현재 활성화된 차량 가져오기
        private CarEntity GetCurrentCar()
        {
            if (_carManager == null || !_carManager.HasActiveCar)
            {
                return null;
            }

            return _carManager.CurrentCar;
        }

        // 특정 위치에 데미지 적용
        private void ApplyDamageAtPosition(CarEntity car, int damage, Vector2 worldPosition)
        {
            CarPartEntity clickedPart = car.GetPartAtPosition(worldPosition);

            if (clickedPart != null)
            {
                car.TakeDamageOnPart(clickedPart, damage);
            }
            else
            {
                car.TakeDamage(damage);
            }
        }

        // 데미지 계산기 교체
        public void SetDamageCalculator(IDamageCalculator calculator)
        {
            _damageCalculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        }
    }
}
