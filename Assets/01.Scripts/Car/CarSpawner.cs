using System.Collections.Generic;
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
                Debug.LogError("CarSpawner: 선택할 수 있는 CarData가 없습니다.");
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

            Vector3 spawnPosition = _spawnPoint != null ? _spawnPoint.position : Vector3.zero;
            
            _currentCar = Instantiate(_carPrefab, spawnPosition, Quaternion.identity);
            _currentCar.Initialize(carData);
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
            Invoke(nameof(SpawnRandomCar), 1f);
        }

        public Car GetCurrentCar()
        {
            return _currentCar;
        }
    }
}
