using UnityEngine;

namespace JunkyardClicker.Core
{
    /// <summary>
    /// 서비스 초기화 및 등록을 담당하는 인스톨러
    /// 씬에 배치하여 서비스들을 ServiceLocator에 등록
    /// </summary>
    public class ServiceInstaller : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            ServiceLocator.Clear();
        }

        private void Awake()
        {
            InstallServices();
        }

        private void InstallServices()
        {
            // 서비스들은 각자의 Awake에서 ServiceLocator에 등록됨
            // 이 클래스는 순서 보장이 필요할 때 사용
            Debug.Log("[ServiceInstaller] 서비스 설치 시작");
        }

        private void OnDestroy()
        {
            ServiceLocator.Clear();
        }
    }
}
