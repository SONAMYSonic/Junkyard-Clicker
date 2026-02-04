using System;
using UnityEngine;

namespace JunkyardClicker.Core
{
    /// <summary>
    /// 입력 핸들러 인터페이스
    /// 입력 시스템을 추상화하여 테스트 가능성 향상
    /// </summary>
    public interface IInputHandler
    {
        /// <summary>
        /// 클릭/터치 이벤트 발생 시
        /// </summary>
        event Action<Vector2> OnClicked;

        /// <summary>
        /// 입력 활성화 여부
        /// </summary>
        bool IsEnabled { get; set; }
    }
}
