using UnityEngine;

namespace JunkyardClicker.Resource
{
    using Car;
    using Core;

    public class AutoDamageHandler : MonoBehaviour
    {
        [SerializeField]
        private float _tickInterval = 1f;

        private Car _currentCar;
        private float _tickTimer;

        private void OnEnable()
        {
            GameEvents.OnCarSpawned += RefreshCarReference;
        }

        private void OnDisable()
        {
            GameEvents.OnCarSpawned -= RefreshCarReference;
        }

        private void Update()
        {
            if (_currentCar == null || _currentCar.IsDestroyed)
            {
                return;
            }

            if (NewUpgradeManager.Instance == null)
            {
                return;
            }

            int workerDps = NewUpgradeManager.Instance.WorkerDps;

            if (workerDps <= 0)
            {
                return;
            }

            _tickTimer += Time.deltaTime;

            if (_tickTimer >= _tickInterval)
            {
                ApplyAutoDamage(workerDps);
                _tickTimer = 0f;
            }
        }

        private void ApplyAutoDamage(int damage)
        {
            if (damage > 0)
            {
                _currentCar.TakeDamage(damage);
            }
        }

        private void RefreshCarReference()
        {
            _currentCar = FindAnyObjectByType<Car>();
            _tickTimer = 0f;
        }

        public void SetCar(Car car)
        {
            _currentCar = car;
            _tickTimer = 0f;
        }
    }
}
