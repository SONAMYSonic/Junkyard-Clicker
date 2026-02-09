using System;

namespace JunkyardClicker.UI.MVVM
{
    // 관찰 가능한 값 구현 - MVVM 패턴에서 ViewModel 속성으로 사용
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

        // 기본 생성자
        public Observable()
        {
            _value = default;
        }

        // 초기값 지정 생성자
        public Observable(T initialValue)
        {
            _value = initialValue;
        }

        // 이벤트 발생 없이 값 설정
        public void SetValueWithoutNotify(T value)
        {
            _value = value;
        }

        // 암시적 변환 연산자
        public static implicit operator T(Observable<T> observable)
        {
            return observable.Value;
        }
    }
}
