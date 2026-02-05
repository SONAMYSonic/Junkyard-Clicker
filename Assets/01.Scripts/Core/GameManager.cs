using UnityEngine;

namespace JunkyardClicker.Core
{
    using Car;

    /// <summary>
    /// 게임 매니저 - 싱글톤 및 핵심 참조 관리
    /// 데미지 계산은 DamageCalculator가 담당 (SRP)
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Core References")]
        [SerializeField]
        private CarSpawner _carSpawner;

        // ClickHandler, AutoDamageHandler는 자체 싱글톤으로 작동
        // 불필요한 참조 제거 (미사용 필드)

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

        // GetClickDamage(), GetAutoDamagePerSecond() 제거
        // DamageCalculator (IDamageCalculator)가 담당 (SRP)
    }
}
