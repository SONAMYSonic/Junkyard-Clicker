using System;

namespace JunkyardClicker.Core
{
    /// <summary>
    /// 자동 데미지 서비스 인터페이스
    /// 자동 데미지 시스템을 추상화하여 테스트 가능성 향상
    /// </summary>
    public interface IAutoDamageService
    {
        /// <summary>
        /// 자동 데미지 틱 발생 시
        /// </summary>
        event Action OnAutoDamageTick;

        /// <summary>
        /// 자동 데미지 활성화 여부
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// 틱 간격 (초)
        /// </summary>
        float TickInterval { get; set; }

        /// <summary>
        /// 타이머 리셋
        /// </summary>
        void ResetTimer();
    }
}
