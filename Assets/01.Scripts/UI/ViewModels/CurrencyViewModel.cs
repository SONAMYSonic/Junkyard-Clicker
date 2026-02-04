namespace JunkyardClicker.UI.ViewModels
{
    using JunkyardClicker.UI.MVVM;

    /// <summary>
    /// 재화 UI를 위한 ViewModel
    /// </summary>
    public class CurrencyViewModel : ViewModelBase
    {
        public Observable<string> Money { get; } = new Observable<string>("0");
        public Observable<string> Scrap { get; } = new Observable<string>("0");
        public Observable<string> Glass { get; } = new Observable<string>("0");
        public Observable<string> Plate { get; } = new Observable<string>("0");
        public Observable<string> Rubber { get; } = new Observable<string>("0");

        public override void Initialize()
        {
            base.Initialize();
            CurrencyManager.OnDataChanged += RefreshAll;
            RefreshAll();
        }

        protected override void OnDispose()
        {
            CurrencyManager.OnDataChanged -= RefreshAll;
            base.OnDispose();
        }

        private void RefreshAll()
        {
            if (CurrencyManager.Instance == null)
            {
                return;
            }

            Money.Value = CurrencyManager.Instance.Money.ToString();
            Scrap.Value = CurrencyManager.Instance.Scrap.ToString();
            Glass.Value = CurrencyManager.Instance.Glass.ToString();
            Plate.Value = CurrencyManager.Instance.Plate.ToString();
            Rubber.Value = CurrencyManager.Instance.Rubber.ToString();
        }
    }
}
