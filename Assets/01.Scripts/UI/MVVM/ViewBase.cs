using UnityEngine;

namespace JunkyardClicker.UI.MVVM
{
    // View 기본 클래스 - ViewModel에 바인딩하여 UI 업데이트
    public abstract class ViewBase<TViewModel> : MonoBehaviour where TViewModel : ViewModelBase, new()
    {
        protected TViewModel ViewModel { get; private set; }

        // ViewModel 생성
        protected virtual void Awake()
        {
            ViewModel = CreateViewModel();
        }

        // ViewModel 초기화 및 바인딩
        protected virtual void OnEnable()
        {
            ViewModel?.Initialize();
            BindViewModel();
        }

        // 바인딩 해제
        protected virtual void OnDisable()
        {
            UnbindViewModel();
        }

        // ViewModel 해제
        protected virtual void OnDestroy()
        {
            ViewModel?.Dispose();
        }

        // ViewModel 인스턴스 생성
        protected virtual TViewModel CreateViewModel()
        {
            return new TViewModel();
        }

        protected abstract void BindViewModel();
        protected abstract void UnbindViewModel();
    }
}
