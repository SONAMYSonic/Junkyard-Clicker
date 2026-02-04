namespace JunkyardClicker.UI.ViewModels
{
    using JunkyardClicker.Core;
    using JunkyardClicker.UI.MVVM;

    /// <summary>
    /// 재화 UI를 위한 ViewModel
    /// DIP: ICurrencyService 인터페이스를 통해 의존성 주입
    /// </summary>
    public class CurrencyViewModel : ViewModelBase
    {
        private ICurrencyService _currencyService;

        public Observable<string> Money { get; } = new Observable<string>("0");
        public Observable<string> Scrap { get; } = new Observable<string>("0");
        public Observable<string> Glass { get; } = new Observable<string>("0");
        public Observable<string> Plate { get; } = new Observable<string>("0");
        public Observable<string> Rubber { get; } = new Observable<string>("0");

        public override void Initialize()
        {
            base.Initialize();

            // 인터페이스를 통한 의존성 주입
            if (ServiceLocator.TryGet<ICurrencyService>(out var service))
            {
                _currencyService = service;
                _currencyService.OnDataChanged += RefreshAll;
            }
            else
            {
                // 폴백: 레거시 지원
                CurrencyManager.OnDataChanged += RefreshAll;
            }

            RefreshAll();
        }

        protected override void OnDispose()
        {
            if (_currencyService != null)
            {
                _currencyService.OnDataChanged -= RefreshAll;
            }
            else
            {
                CurrencyManager.OnDataChanged -= RefreshAll;
            }

            base.OnDispose();
        }

        private void RefreshAll()
        {
            if (_currencyService != null)
            {
                Money.Value = _currencyService.Money.ToString();
                Scrap.Value = _currencyService.Scrap.ToString();
                Glass.Value = _currencyService.Glass.ToString();
                Plate.Value = _currencyService.Plate.ToString();
                Rubber.Value = _currencyService.Rubber.ToString();
            }
            else if (CurrencyManager.Instance != null)
            {
                // 폴백: 레거시 지원
                Money.Value = CurrencyManager.Instance.Money.ToString();
                Scrap.Value = CurrencyManager.Instance.Scrap.ToString();
                Glass.Value = CurrencyManager.Instance.Glass.ToString();
                Plate.Value = CurrencyManager.Instance.Plate.ToString();
                Rubber.Value = CurrencyManager.Instance.Rubber.ToString();
            }
        }
    }
}
