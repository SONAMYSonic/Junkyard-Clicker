using System;
using UnityEngine;

namespace JunkyardClicker.Car
{
    using JunkyardClicker.Core;

    // 차량 스폰 매니저 - 차량 스폰과 현재 차량 관리만 담당
    public class CarSpawner : MonoBehaviour, ICarManager
    {
        public static CarSpawner Instance { get; private set; }

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

        public CarEntity CurrentCar => _currentCar;
        public bool HasActiveCar => _currentCar != null && !_currentCar.IsDestroyed;

        // 싱글톤 설정 및 서비스 등록
        private void Awake()
        {
            SetupSingleton();
            _spawnSelector = new CarSpawnSelector(_carDataList);
            ServiceLocator.Register<ICarManager>(this);
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

        // 첫 차량 스폰
        private void Start()
        {
            SpawnRandomCar();
        }

        // 오브젝트 파괴 시 정리
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                ServiceLocator.Unregister<ICarManager>();
            }
        }

        // 랜덤 차량 스폰
        public void SpawnRandomCar()
        {
            CarData selectedData = _spawnSelector.SelectRandom();

            if (selectedData == null)
            {
                Debug.LogError("[CarSpawner] 선택할 수 있는 CarData가 없습니다.");
                return;
            }

            SpawnCar(selectedData);
        }

        // 특정 데이터로 차량 스폰
        public void SpawnCar(CarData carData)
        {
            if (_currentCar != null)
            {
                _currentCar.OnDestroyed -= HandleCarDestroyed;
                Destroy(_currentCar.gameObject);
            }

            if (_carPrefab == null)
            {
                Debug.LogError("[CarSpawner] Car Prefab이 연결되지 않았습니다.");
                return;
            }

            Vector3 spawnPosition = _spawnPoint != null ? _spawnPoint.position : Vector3.zero;

            _currentCar = Instantiate(_carPrefab, spawnPosition, Quaternion.identity);
            _currentCar.Initialize(carData);

            _currentCar.OnDestroyed += HandleCarDestroyed;

            OnCarSpawned?.Invoke(_currentCar);
            GameEvents.RaiseCarSpawned(_currentCar);

            Debug.Log($"[CarSpawner] 새 차량 스폰: {carData.CarName}");
        }

        // 차량 파괴 시 처리
        private void HandleCarDestroyed(CarEntity car, int reward)
        {
            OnCarDestroyed?.Invoke(reward);
            Invoke(nameof(SpawnRandomCar), _respawnDelay);
        }
    }
}
