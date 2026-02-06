using UnityEngine;

namespace JunkyardClicker.UI.ViewModels
{
    using Core;
    using MVVM;

    /// <summary>
    /// 부품 판매 버튼을 위한 ViewModel
    /// 엄격한 MVVM: ServiceLocator를 통한 의존성 주입만 사용
    /// </summary>
    public class SellPartsViewModel : ViewModelBase
    {
        private ICurrencyService _currencyService;

        public Observable<string> SellValueText { get; } = new Observable<string>("판매 ($0)");
        public Observable<bool> CanSell { get; } = new Observable<bool>(false);

        public override void Initialize()
        {
            base.Initialize();

            if (!ServiceLocator.TryGet<ICurrencyService>(out _currencyService))
            {
                Debug.LogError("[SellPartsViewModel] ICurrencyService를 찾을 수 없습니다.");
                return;
            }

            _currencyService.OnDataChanged += RefreshAll;
            RefreshAll();
        }

        protected override void OnDispose()
        {
            if (_currencyService != null)
            {
                _currencyService.OnDataChanged -= RefreshAll;
            }

            base.OnDispose();
        }

        public void RequestSell()
        {
            if (_currencyService == null)
            {
                Debug.LogError("[SellPartsViewModel] ICurrencyService가 없어 판매할 수 없습니다.");
                return;
            }

            _currencyService.SellAllParts();
        }

        private void RefreshAll()
        {
            if (_currencyService == null) return;

            int totalValue = CalculateTotalValue();
            Currency valueCurrency = totalValue;
            SellValueText.Value = $"판매 (${valueCurrency})";
            CanSell.Value = totalValue > 0;
        }

        private int CalculateTotalValue()
        {
            if (_currencyService == null) return 0;

            int total = 0;
            total += (int)(_currencyService.Scrap.Value * 5);
            total += (int)(_currencyService.Glass.Value * 3);
            total += (int)(_currencyService.Plate.Value * 8);
            total += (int)(_currencyService.Rubber.Value * 4);
            return total;
        }
    }
}
