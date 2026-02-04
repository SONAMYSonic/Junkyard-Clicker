using System;

namespace JunkyardClicker.UI.MVVM
{
    /// <summary>
    /// ViewModel 기본 클래스
    /// </summary>
    public abstract class ViewModelBase : IDisposable
    {
        private bool _isDisposed;

        public virtual void Initialize()
        {
        }

        public virtual void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }
}
