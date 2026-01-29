using UnityEngine;

namespace JunkyardClicker.Core
{
    using Car;
    using Input;
    using Resource;
    using Upgrade;

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Managers")]
        [SerializeField]
        private ResourceManager _resourceManager;

        [SerializeField]
        private UpgradeManager _upgradeManager;

        [Header("Handlers")]
        [SerializeField]
        private CarSpawner _carSpawner;

        [SerializeField]
        private ClickHandler _clickHandler;

        [SerializeField]
        private AutoDamageHandler _autoDamageHandler;

        public ResourceManager Resource => _resourceManager;
        public UpgradeManager Upgrade => _upgradeManager;
        public CarSpawner Spawner => _carSpawner;

        private void Awake()
        {
            SetupSingleton();
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

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public int GetClickDamage()
        {
            int toolLevel = _upgradeManager != null ? _upgradeManager.GetLevel(UpgradeType.Tool) : 0;
            return DamageCalculator.CalculateClickDamage(toolLevel);
        }

        public int GetAutoDamagePerSecond()
        {
            int workerLevel = _upgradeManager != null ? _upgradeManager.GetLevel(UpgradeType.Worker) : 0;
            return DamageCalculator.CalculateAutoDamagePerSecond(workerLevel);
        }
    }
}
