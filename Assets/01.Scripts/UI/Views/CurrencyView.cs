using UnityEngine;
using TMPro;

namespace JunkyardClicker.UI.Views
{
    using JunkyardClicker.UI.MVVM;
    using JunkyardClicker.UI.ViewModels;

    /// <summary>
    /// 재화 표시 View
    /// </summary>
    public class CurrencyView : ViewBase<CurrencyViewModel>
    {
        [SerializeField]
        private TextMeshProUGUI _moneyText;

        [SerializeField]
        private TextMeshProUGUI _scrapText;

        [SerializeField]
        private TextMeshProUGUI _glassText;

        [SerializeField]
        private TextMeshProUGUI _plateText;

        [SerializeField]
        private TextMeshProUGUI _rubberText;

        protected override void BindViewModel()
        {
            ViewModel.Money.OnValueChanged += UpdateMoneyText;
            ViewModel.Scrap.OnValueChanged += UpdateScrapText;
            ViewModel.Glass.OnValueChanged += UpdateGlassText;
            ViewModel.Plate.OnValueChanged += UpdatePlateText;
            ViewModel.Rubber.OnValueChanged += UpdateRubberText;

            // 초기값 설정
            UpdateMoneyText(ViewModel.Money.Value);
            UpdateScrapText(ViewModel.Scrap.Value);
            UpdateGlassText(ViewModel.Glass.Value);
            UpdatePlateText(ViewModel.Plate.Value);
            UpdateRubberText(ViewModel.Rubber.Value);
        }

        protected override void UnbindViewModel()
        {
            ViewModel.Money.OnValueChanged -= UpdateMoneyText;
            ViewModel.Scrap.OnValueChanged -= UpdateScrapText;
            ViewModel.Glass.OnValueChanged -= UpdateGlassText;
            ViewModel.Plate.OnValueChanged -= UpdatePlateText;
            ViewModel.Rubber.OnValueChanged -= UpdateRubberText;
        }

        private void UpdateMoneyText(string value)
        {
            if (_moneyText != null)
            {
                _moneyText.text = value;
            }
        }

        private void UpdateScrapText(string value)
        {
            if (_scrapText != null)
            {
                _scrapText.text = value;
            }
        }

        private void UpdateGlassText(string value)
        {
            if (_glassText != null)
            {
                _glassText.text = value;
            }
        }

        private void UpdatePlateText(string value)
        {
            if (_plateText != null)
            {
                _plateText.text = value;
            }
        }

        private void UpdateRubberText(string value)
        {
            if (_rubberText != null)
            {
                _rubberText.text = value;
            }
        }
    }
}
