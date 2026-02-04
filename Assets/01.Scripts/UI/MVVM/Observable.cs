using System;

namespace JunkyardClicker.UI.MVVM
{
    /// <summary>
    /// 관찰 가능한 값 구현
    /// MVVM 패턴에서 ViewModel의 속성으로 사용
    /// </summary>
    public class Observable<T> : IObservable<T>
    {
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    OnValueChanged?.Invoke(_value);
                }
            }
        }

        public event Action<T> OnValueChanged;

        public Observable()
        {
            _value = default;
        }

        public Observable(T initialValue)
        {
            _value = initialValue;
        }

        public void SetValueWithoutNotify(T value)
        {
            _value = value;
        }

        public static implicit operator T(Observable<T> observable)
        {
            return observable.Value;
        }
    }
}
