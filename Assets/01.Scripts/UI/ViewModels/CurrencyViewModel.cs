using UnityEngine;

namespace JunkyardClicker.UI.ViewModels
{
    using JunkyardClicker.Core;
    using JunkyardClicker.UI.MVVM;

    // 재화 UI를 위한 ViewModel
    public class CurrencyViewModel : ViewModelBase
    {
        private ICurrencyService _currencyService;

        public Observable<string> Money { get; } = new Observable<string>("0");
        public Observable<string> Scrap { get; } = new Observable<string>("0");
        public Observable<string> Glass { get; } = new Observable<string>("0");
        public Observable<string> Plate { get; } = new Observable<string>("0");
        public Observable<string> Rubber { get; } = new Observable<string>("0");

        // 초기화 및 이벤트 구독
        public override void Initialize()
        {
            base.Initialize();

            if (!ServiceLocator.TryGet<ICurrencyService>(out _currencyService))
            {
                Debug.LogError("[CurrencyViewModel] ICurrencyService를 찾을 수 없습니다.");
                return;
            }

            _currencyService.OnDataChanged += RefreshAll;
            RefreshAll();
        }

        // 이벤트 구독 해제
        protected override void OnDispose()
        {
            if (_currencyService != null)
            {
                _currencyService.OnDataChanged -= RefreshAll;
            }

            base.OnDispose();
        }

        // 모든 재화 표시 갱신
        private void RefreshAll()
        {
            if (_currencyService == null) return;

            Money.Value = _currencyService.Money.ToString();
            Scrap.Value = _currencyService.Scrap.ToString();
            Glass.Value = _currencyService.Glass.ToString();
            Plate.Value = _currencyService.Plate.ToString();
            Rubber.Value = _currencyService.Rubber.ToString();
        }
    }
}
