using UnityEngine;
using JunkyardClicker.Core;

/// <summary>
/// 게임 이벤트 브릿지
/// 주의: 재화 처리는 CurrencyManager가 직접 GameEvents를 구독하므로
/// 여기서는 재화 관련 이벤트를 처리하지 않음 (중복 방지)
///
/// 이 클래스는 다른 시스템 간의 이벤트 연결이 필요할 때 사용
/// </summary>
public class GameEventBridge : MonoBehaviour
{
    // 재화 관련 이벤트는 CurrencyManager가 직접 처리함
    // 중복 처리 버그 수정: HandleCarDestroyed, HandlePartCollected 제거

    private void OnEnable()
    {
        // 필요한 다른 이벤트 브릿지 연결
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
    }
}
