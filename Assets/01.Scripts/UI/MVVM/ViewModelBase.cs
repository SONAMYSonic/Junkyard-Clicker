using System;

namespace JunkyardClicker.UI.MVVM
{
    // ViewModel 기본 클래스
    public abstract class ViewModelBase : IDisposable
    {
        private bool _isDisposed;

        // 초기화
        public virtual void Initialize()
        {
        }

        // 리소스 해제
        public virtual void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            OnDispose();
        }

        // 파생 클래스에서 정리 로직 구현
        protected virtual void OnDispose()
        {
        }
    }
}
