using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JunkyardClicker.UI.Views
{
    using JunkyardClicker.UI.MVVM;
    using JunkyardClicker.UI.ViewModels;

    /// <summary>
    /// HP 바 View
    /// </summary>
    public class HpBarView : ViewBase<HpBarViewModel>
    {
        [SerializeField]
        private Image _fillImage;

        [SerializeField]
        private TextMeshProUGUI _hpText;

        [SerializeField]
        private TextMeshProUGUI _carNameText;

        [SerializeField]
        private Image _gradeIndicator;

        protected override void BindViewModel()
        {
            ViewModel.HpRatio.OnValueChanged += UpdateFillAmount;
            ViewModel.HpText.OnValueChanged += UpdateHpText;
            ViewModel.CarName.OnValueChanged += UpdateCarName;
            ViewModel.GradeColor.OnValueChanged += UpdateGradeColor;
            ViewModel.HpBarColor.OnValueChanged += UpdateHpBarColor;

            // 초기값 설정
            UpdateFillAmount(ViewModel.HpRatio.Value);
            UpdateHpText(ViewModel.HpText.Value);
            UpdateCarName(ViewModel.CarName.Value);
            UpdateGradeColor(ViewModel.GradeColor.Value);
            UpdateHpBarColor(ViewModel.HpBarColor.Value);
        }

        protected override void UnbindViewModel()
        {
            ViewModel.HpRatio.OnValueChanged -= UpdateFillAmount;
            ViewModel.HpText.OnValueChanged -= UpdateHpText;
            ViewModel.CarName.OnValueChanged -= UpdateCarName;
            ViewModel.GradeColor.OnValueChanged -= UpdateGradeColor;
            ViewModel.HpBarColor.OnValueChanged -= UpdateHpBarColor;
        }

        private void UpdateFillAmount(float ratio)
        {
            if (_fillImage != null)
            {
                _fillImage.fillAmount = ratio;
            }
        }

        private void UpdateHpText(string text)
        {
            if (_hpText != null)
            {
                _hpText.text = text;
            }
        }

        private void UpdateCarName(string name)
        {
            if (_carNameText != null)
            {
                _carNameText.text = name;
            }
        }

        private void UpdateGradeColor(Color color)
        {
            if (_gradeIndicator != null)
            {
                _gradeIndicator.color = color;
            }
        }

        private void UpdateHpBarColor(Color color)
        {
            if (_fillImage != null)
            {
                _fillImage.color = color;
            }
        }
    }
}
