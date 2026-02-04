using System;
using UnityEngine;

namespace JunkyardClicker.Ingame.Car
{
    using JunkyardClicker.Core;

    /// <summary>
    /// 차량 시스템 매니저
    /// 차량 스폰, 데미지 처리, 이벤트 발행을 담당
    /// </summary>
    public class CarManager : MonoBehaviour, ICarManager
    {
        public static CarManager Instance { get; private set; }

        [SerializeField]
        private Transform _spawnPoint;

        [SerializeField]
        private CarEntity _carPrefab;

        [SerializeField]
        private CarData[] _carDataList;

        [SerializeField]
        private float _respawnDelay = 1f;

        private CarEntity _currentCar;
        private CarSpawnSelector _spawnSelector;

        public event Action<CarEntity> OnCarSpawned;
        public event Action<int> OnCarDestroyed;
        public event Action<int> OnDamageDealt;

        public CarEntity CurrentCar => _currentCar;
        public bool HasActiveCar => _currentCar != null && !_currentCar.IsDestroyed;

        private void Awake()
        {
            SetupSingleton();
            _spawnSelector = new CarSpawnSelector(_carDataList);
            ServiceLocator.Register<ICarManager>(this);
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
            SpawnRandomCar();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                ServiceLocator.Unregister<ICarManager>();
            }
        }

        public void SpawnRandomCar()
        {
            CarData selectedData = _spawnSelector.SelectRandom();

            if (selectedData == null)
            {
                Debug.LogError("[CarManager] 선택할 수 있는 CarData가 없습니다.");
                return;
            }

            SpawnCar(selectedData);
        }

        public void SpawnCar(CarData carData)
        {
            if (_currentCar != null)
            {
                _currentCar.OnDestroyed -= HandleCarDestroyed;
                _currentCar.OnDamageReceived -= HandleDamageReceived;
                Destroy(_currentCar.gameObject);
            }

            if (_carPrefab == null)
            {
                Debug.LogError("[CarManager] Car Prefab이 연결되지 않았습니다.");
                return;
            }

            Vector3 spawnPosition = _spawnPoint != null ? _spawnPoint.position : Vector3.zero;

            _currentCar = Instantiate(_carPrefab, spawnPosition, Quaternion.identity);
            _currentCar.Initialize(carData);

            _currentCar.OnDestroyed += HandleCarDestroyed;
            _currentCar.OnDamageReceived += HandleDamageReceived;

            OnCarSpawned?.Invoke(_currentCar);

            // 기존 GameEvents와의 호환성 유지
            GameEvents.RaiseCarSpawned(null);

            Debug.Log($"[CarManager] 새 차량 스폰: {carData.CarName}");
        }

        public void ApplyDamage(int damage)
        {
            if (!HasActiveCar)
            {
                return;
            }

            _currentCar.TakeDamage(damage);
        }

        public void ApplyDamageAtPosition(int damage, Vector2 worldPosition)
        {
            if (!HasActiveCar)
            {
                return;
            }

            CarPartEntity clickedPart = _currentCar.GetPartAtPosition(worldPosition);

            if (clickedPart != null)
            {
                _currentCar.TakeDamageOnPart(clickedPart, damage);
            }
            else
            {
                _currentCar.TakeDamage(damage);
            }
        }

        private void HandleCarDestroyed(CarEntity car, int reward)
        {
            OnCarDestroyed?.Invoke(reward);

            Invoke(nameof(SpawnRandomCar), _respawnDelay);
        }

        private void HandleDamageReceived(int damage)
        {
            OnDamageDealt?.Invoke(damage);
        }
    }
}
