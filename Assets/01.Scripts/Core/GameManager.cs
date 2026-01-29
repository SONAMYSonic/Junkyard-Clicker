using UnityEngine;

namespace JunkyardClicker.Core
{
    using Car;
    using Input;
    using Resource;

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Handlers")]
        [SerializeField]
        private CarSpawner _carSpawner;

        [SerializeField]
        private ClickHandler _clickHandler;

        [SerializeField]
        private AutoDamageHandler _autoDamageHandler;

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
            if (NewUpgradeManager.Instance == null)
            {
                return 1;
            }

            return NewUpgradeManager.Instance.ToolDamage;
        }

        public int GetAutoDamagePerSecond()
        {
            if (NewUpgradeManager.Instance == null)
            {
                return 0;
            }

            return NewUpgradeManager.Instance.WorkerDps;
        }
    }
}
