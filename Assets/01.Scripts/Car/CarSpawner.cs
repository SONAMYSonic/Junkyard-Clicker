using UnityEngine;

namespace JunkyardClicker.Car
{
    using Core;

    public class CarSpawner : MonoBehaviour
    {
        [SerializeField]
        private Transform _spawnPoint;

        [SerializeField]
        private Car _carPrefab;

        [SerializeField]
        private CarData[] _carDataList;

        private Car _currentCar;
        private float _totalWeight;

        private void Awake()
        {
            CalculateTotalWeight();
        }

        private void OnEnable()
        {
            GameEvents.OnCarDestroyed += HandleCarDestroyed;
            Debug.Log("[CarSpawner] OnCarDestroyed 이벤트 구독 완료");
        }

        private void OnDisable()
        {
            GameEvents.OnCarDestroyed -= HandleCarDestroyed;
        }

        private void Start()
        {
            SpawnRandomCar();
        }

        private void CalculateTotalWeight()
        {
            _totalWeight = 0f;
            foreach (CarData data in _carDataList)
            {
                if (data != null)
                {
                    _totalWeight += data.GetSpawnWeight();
                }
            }
        }

        public void SpawnRandomCar()
        {
            CarData selectedData = SelectRandomCarData();

            if (selectedData == null)
            {
                Debug.LogError("[CarSpawner] 선택할 수 있는 CarData가 없습니다. Car Data List를 확인하세요.");
                return;
            }

            SpawnCar(selectedData);
        }

        public void SpawnCar(CarData carData)
        {
            if (_currentCar != null)
            {
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
            
            Debug.Log($"[CarSpawner] 새 차량 스폰: {carData.CarName}");
        }

        private CarData SelectRandomCarData()
        {
            if (_carDataList == null || _carDataList.Length == 0)
            {
                return null;
            }

            float randomValue = Random.Range(0f, _totalWeight);
            float currentWeight = 0f;

            foreach (CarData data in _carDataList)
            {
                if (data == null)
                {
                    continue;
                }

                currentWeight += data.GetSpawnWeight();

                if (randomValue <= currentWeight)
                {
                    return data;
                }
            }

            return _carDataList[0];
        }

        private void HandleCarDestroyed(int reward)
        {
            Debug.Log($"[CarSpawner] 차량 파괴됨! 보상: {reward}, 1초 후 새 차량 스폰 예정");
            Invoke(nameof(SpawnRandomCar), 1f);
        }

        public Car GetCurrentCar()
        {
            return _currentCar;
        }
    }
}
