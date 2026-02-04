using System;

namespace JunkyardClicker.UI.MVVM
{
    /// <summary>
    /// 관찰 가능한 값 인터페이스
    /// </summary>
    public interface IObservable<T>
    {
        T Value { get; }
        event Action<T> OnValueChanged;
    }
}
