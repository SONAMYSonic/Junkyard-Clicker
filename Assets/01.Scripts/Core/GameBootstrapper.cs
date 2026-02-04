using UnityEngine;

namespace JunkyardClicker.Core
{
    /// <summary>
    /// 게임 초기화 및 서비스 설정을 담당하는 부트스트래퍼
    /// 씬에 배치하여 서비스들을 초기화하고 연결
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            ServiceLocator.Clear();
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            InitializeServices();
        }

        private void InitializeServices()
        {
            Debug.Log("[GameBootstrapper] 서비스 초기화 시작");
        }

        private void OnDestroy()
        {
            ServiceLocator.Clear();
        }
    }
}
