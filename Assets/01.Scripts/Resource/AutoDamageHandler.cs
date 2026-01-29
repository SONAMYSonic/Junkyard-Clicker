using UnityEngine;

namespace JunkyardClicker.Resource
{
    using Car;
    using Core;
    using Upgrade;

    public class AutoDamageHandler : MonoBehaviour
    {
        [SerializeField]
        private UpgradeManager _upgradeManager;

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

            int workerLevel = GetWorkerLevel();
            
            if (workerLevel <= 0)
            {
                return;
            }

            _tickTimer += Time.deltaTime;

            if (_tickTimer >= _tickInterval)
            {
                ApplyAutoDamage(workerLevel);
                _tickTimer = 0f;
            }
        }

        private int GetWorkerLevel()
        {
            if (_upgradeManager == null)
            {
                return 0;
            }

            return _upgradeManager.GetLevel(UpgradeType.Worker);
        }

        private void ApplyAutoDamage(int workerLevel)
        {
            int damage = DamageCalculator.CalculateAutoDamagePerSecond(workerLevel);
            
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
