using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JunkyardClicker.UI.Views
{
    using JunkyardClicker.UI.MVVM;
    using JunkyardClicker.UI.ViewModels;

    /// <summary>
    /// 업그레이드 버튼 View
    /// </summary>
    public class UpgradeButtonView : ViewBase<UpgradeViewModel>
    {
        [SerializeField]
        private EUpgradeType _upgradeType;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private TextMeshProUGUI _nameText;

        [SerializeField]
        private TextMeshProUGUI _levelText;

        [SerializeField]
        private TextMeshProUGUI _costText;

        [SerializeField]
        private TextMeshProUGUI _effectText;

        protected override void Awake()
        {
            base.Awake();

            if (_button != null)
            {
                _button.onClick.AddListener(OnButtonClicked);
            }
        }

        protected override void OnEnable()
        {
            ViewModel.SetUpgradeType(_upgradeType);
            base.OnEnable();
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
            ViewModel.Name.OnValueChanged += UpdateNameText;
            ViewModel.Level.OnValueChanged += UpdateLevelText;
            ViewModel.Cost.OnValueChanged += UpdateCostText;
            ViewModel.Effect.OnValueChanged += UpdateEffectText;
            ViewModel.CanUpgrade.OnValueChanged += UpdateButtonInteractable;

            // 초기값 설정
            UpdateNameText(ViewModel.Name.Value);
            UpdateLevelText(ViewModel.Level.Value);
            UpdateCostText(ViewModel.Cost.Value);
            UpdateEffectText(ViewModel.Effect.Value);
            UpdateButtonInteractable(ViewModel.CanUpgrade.Value);
        }

        protected override void UnbindViewModel()
        {
            ViewModel.Name.OnValueChanged -= UpdateNameText;
            ViewModel.Level.OnValueChanged -= UpdateLevelText;
            ViewModel.Cost.OnValueChanged -= UpdateCostText;
            ViewModel.Effect.OnValueChanged -= UpdateEffectText;
            ViewModel.CanUpgrade.OnValueChanged -= UpdateButtonInteractable;
        }

        private void OnButtonClicked()
        {
            ViewModel.RequestUpgrade();
        }

        private void UpdateNameText(string value)
        {
            if (_nameText != null)
            {
                _nameText.text = value;
            }
        }

        private void UpdateLevelText(string value)
        {
            if (_levelText != null)
            {
                _levelText.text = value;
            }
        }

        private void UpdateCostText(string value)
        {
            if (_costText != null)
            {
                _costText.text = value;
            }
        }

        private void UpdateEffectText(string value)
        {
            if (_effectText != null)
            {
                _effectText.text = value;
            }
        }

        private void UpdateButtonInteractable(bool canUpgrade)
        {
            if (_button != null)
            {
                _button.interactable = canUpgrade;
            }
        }
    }
}
