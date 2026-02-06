using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JunkyardClicker.UI.Views
{
    using JunkyardClicker.UI.MVVM;
    using JunkyardClicker.UI.ViewModels;

    /// <summary>
    /// 부품 판매 버튼 View
    /// </summary>
    public class SellPartsButtonView : ViewBase<SellPartsViewModel>
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private TextMeshProUGUI _valueText;

        protected override void Awake()
        {
            base.Awake();

            if (_button != null)
            {
                _button.onClick.AddListener(OnButtonClicked);
            }
        }

        protected override void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnButtonClicked);
            }

            base.OnDestroy();
        }

        protected override void BindViewModel()
        {
            ViewModel.SellValueText.OnValueChanged += UpdateValueText;
            ViewModel.CanSell.OnValueChanged += UpdateButtonInteractable;

            // 초기값 설정
            UpdateValueText(ViewModel.SellValueText.Value);
            UpdateButtonInteractable(ViewModel.CanSell.Value);
        }

        protected override void UnbindViewModel()
        {
            ViewModel.SellValueText.OnValueChanged -= UpdateValueText;
            ViewModel.CanSell.OnValueChanged -= UpdateButtonInteractable;
        }

        private void OnButtonClicked()
        {
            ViewModel.RequestSell();
        }

        private void UpdateValueText(string value)
        {
            if (_valueText != null)
            {
                _valueText.text = value;
            }
        }

        private void UpdateButtonInteractable(bool canSell)
        {
            if (_button != null)
            {
                _button.interactable = canSell;
            }
        }
    }
}
