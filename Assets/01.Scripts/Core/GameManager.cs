using UnityEngine;

namespace JunkyardClicker.Core
{
    using Car;

    // 게임 매니저 - 싱글톤 및 핵심 참조 관리
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Core References")]
        [SerializeField]
        private CarSpawner _carSpawner;

        // ClickHandler, AutoDamageHandler는 자체 싱글톤으로 작동
        // 불필요한 참조 제거 (미사용 필드)

        public CarSpawner Spawner => _carSpawner;

        // 싱글톤 설정
        private void Awake()
        {
            SetupSingleton();
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

        // 오브젝트 파괴 시 정리
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
