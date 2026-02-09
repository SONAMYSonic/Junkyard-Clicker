using UnityEngine;

namespace JunkyardClicker.UI.MVVM
{
    // View 기본 클래스 - ViewModel에 바인딩하여 UI 업데이트
    public abstract class ViewBase<TViewModel> : MonoBehaviour where TViewModel : ViewModelBase, new()
    {
        protected TViewModel ViewModel { get; private set; }

        private bool _isInitialized;

        // ViewModel 생성
        protected virtual void Awake()
        {
            ViewModel = CreateViewModel();
        }

        // Start에서 초기화 (모든 Awake 완료 후 실행되므로 서비스 등록 보장)
        protected virtual void Start()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                ViewModel?.Initialize();
                BindViewModel();
            }
        }

        // 재활성화 시 바인딩 복원
        protected virtual void OnEnable()
        {
            if (_isInitialized)
            {
                ViewModel?.Initialize();
                BindViewModel();
            }
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
