using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JunkyardClicker.UI
{
    using Core;
    using Upgrade;

    public class UpgradeButtonUI : MonoBehaviour
    {
        [SerializeField]
        private UpgradeType _upgradeType;

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

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private UpgradeManager _upgradeManager;

        private void Awake()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(OnButtonClicked);
            }
        }

        private void OnEnable()
        {
            GameEvents.OnMoneyChanged += HandleMoneyChanged;
            GameEvents.OnUpgraded += HandleUpgraded;
            UpdateUI();
        }

        private void OnDisable()
        {
            GameEvents.OnMoneyChanged -= HandleMoneyChanged;
            GameEvents.OnUpgraded -= HandleUpgraded;
        }

        private void OnButtonClicked()
        {
            if (_upgradeManager != null)
            {
                _upgradeManager.TryUpgrade(_upgradeType);
            }
        }

        private void HandleMoneyChanged(int money)
        {
            UpdateButtonInteractable();
        }

        private void HandleUpgraded(UpgradeType type, int level)
        {
            if (type == _upgradeType)
            {
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (_upgradeManager == null)
            {
                return;
            }

            UpgradeData data = _upgradeManager.GetUpgradeData(_upgradeType);
            
            if (data == null)
            {
                return;
            }

            int currentLevel = _upgradeManager.GetLevel(_upgradeType);
            bool isMaxLevel = _upgradeManager.IsMaxLevel(_upgradeType);

            UpdateNameText(data);
            UpdateLevelText(currentLevel, isMaxLevel);
            UpdateCostText(isMaxLevel);
            UpdateEffectText(data, currentLevel);
            UpdateIcon(data, currentLevel);
            UpdateButtonInteractable();
        }

        private void UpdateNameText(UpgradeData data)
        {
            if (_nameText != null)
            {
                _nameText.text = data.DisplayName;
            }
        }

        private void UpdateLevelText(int level, bool isMaxLevel)
        {
            if (_levelText == null)
            {
                return;
            }

            string levelName = _upgradeManager.GetCurrentLevelName(_upgradeType);
            _levelText.text = isMaxLevel ? $"{levelName} (MAX)" : levelName;
        }

        private void UpdateCostText(bool isMaxLevel)
        {
            if (_costText == null)
            {
                return;
            }

            if (isMaxLevel)
            {
                _costText.text = "-";
                return;
            }

            int cost = _upgradeManager.GetUpgradeCost(_upgradeType);
            _costText.text = $"${cost}";
        }

        private void UpdateEffectText(UpgradeData data, int currentLevel)
        {
            if (_effectText == null)
            {
                return;
            }

            int currentValue = data.GetValue(currentLevel);
            string effectDescription = _upgradeType switch
            {
                UpgradeType.Tool => $"클릭 데미지: {currentValue}",
                UpgradeType.Worker => $"초당 데미지: {currentValue}",
                _ => ""
            };

            _effectText.text = effectDescription;
        }

        private void UpdateIcon(UpgradeData data, int level)
        {
            if (_icon != null)
            {
                Sprite iconSprite = data.GetIcon(level);
                
                if (iconSprite != null)
                {
                    _icon.sprite = iconSprite;
                }
            }
        }

        private void UpdateButtonInteractable()
        {
            if (_button != null && _upgradeManager != null)
            {
                _button.interactable = _upgradeManager.CanUpgrade(_upgradeType);
            }
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnButtonClicked);
            }
        }
    }
}
