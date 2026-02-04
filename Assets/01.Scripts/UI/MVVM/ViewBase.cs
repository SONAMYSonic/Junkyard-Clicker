using UnityEngine;

namespace JunkyardClicker.UI.MVVM
{
    /// <summary>
    /// View 기본 클래스
    /// ViewModel에 바인딩하여 UI를 업데이트
    /// </summary>
    public abstract class ViewBase<TViewModel> : MonoBehaviour where TViewModel : ViewModelBase, new()
    {
        protected TViewModel ViewModel { get; private set; }

        protected virtual void Awake()
        {
            ViewModel = CreateViewModel();
        }

        protected virtual void OnEnable()
        {
            ViewModel?.Initialize();
            BindViewModel();
        }

        protected virtual void OnDisable()
        {
            UnbindViewModel();
        }

        protected virtual void OnDestroy()
        {
            ViewModel?.Dispose();
        }

        protected virtual TViewModel CreateViewModel()
        {
            return new TViewModel();
        }

        protected abstract void BindViewModel();
        protected abstract void UnbindViewModel();
    }
}
