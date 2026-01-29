using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JunkyardClicker.Core;

namespace JunkyardClicker.UI
{
    public class UpgradeButtonUI : MonoBehaviour
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

        private void Awake()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(OnButtonClicked);
            }
        }

        private void OnEnable()
        {
            CurrencyManager.OnDataChanged += UpdateButtonInteractable;
            NewUpgradeManager.OnUpgraded += HandleUpgraded;
            UpdateUI();
        }

        private void OnDisable()
        {
            CurrencyManager.OnDataChanged -= UpdateButtonInteractable;
            NewUpgradeManager.OnUpgraded -= HandleUpgraded;
        }

        private void Start()
        {
            UpdateUI();
        }

        private void OnButtonClicked()
        {
            if (NewUpgradeManager.Instance != null)
            {
                NewUpgradeManager.Instance.TryUpgrade(_upgradeType);
            }
        }

        private void HandleUpgraded()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (NewUpgradeManager.Instance == null)
            {
                return;
            }

            int currentLevel = NewUpgradeManager.Instance.GetLevel(_upgradeType);
            bool isMaxLevel = NewUpgradeManager.Instance.IsMaxLevel(_upgradeType);

            UpdateNameText();
            UpdateLevelText(currentLevel, isMaxLevel);
            UpdateCostText(isMaxLevel);
            UpdateEffectText(currentLevel);
            UpdateButtonInteractable();
        }

        private void UpdateNameText()
        {
            if (_nameText != null)
            {
                _nameText.text = _upgradeType switch
                {
                    EUpgradeType.Tool => "도구",
                    EUpgradeType.Worker => "직원",
                    _ => "???"
                };
            }
        }

        private void UpdateLevelText(int level, bool isMaxLevel)
        {
            if (_levelText != null)
            {
                _levelText.text = isMaxLevel ? $"Lv.{level} (MAX)" : $"Lv.{level}";
            }
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

            int cost = NewUpgradeManager.Instance.GetUpgradeCost(_upgradeType);
            Currency costCurrency = cost;
            _costText.text = $"${costCurrency}";
        }

        private void UpdateEffectText(int currentLevel)
        {
            if (_effectText == null)
            {
                return;
            }

            string effectDescription = _upgradeType switch
            {
                EUpgradeType.Tool => $"클릭 데미지: {NewUpgradeManager.Instance.GetToolDamage(currentLevel)}",
                EUpgradeType.Worker => $"초당 데미지: {NewUpgradeManager.Instance.GetWorkerDps(currentLevel)}",
                _ => ""
            };

            _effectText.text = effectDescription;
        }

        private void UpdateButtonInteractable()
        {
            if (_button == null || NewUpgradeManager.Instance == null || CurrencyManager.Instance == null)
            {
                return;
            }

            bool isMaxLevel = NewUpgradeManager.Instance.IsMaxLevel(_upgradeType);

            if (isMaxLevel)
            {
                _button.interactable = false;
                return;
            }

            int cost = NewUpgradeManager.Instance.GetUpgradeCost(_upgradeType);
            _button.interactable = CurrencyManager.Instance.CanAfford(ECurrencyType.Money, cost);
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
