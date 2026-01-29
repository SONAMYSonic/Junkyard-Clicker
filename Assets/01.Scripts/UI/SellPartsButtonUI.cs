using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JunkyardClicker.UI
{
    public class SellPartsButtonUI : MonoBehaviour
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private TextMeshProUGUI _valueText;

        private void Awake()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(OnButtonClicked);
            }
        }

        private void OnEnable()
        {
            CurrencyManager.OnDataChanged += UpdateUI;
            UpdateUI();
        }

        private void OnDisable()
        {
            CurrencyManager.OnDataChanged -= UpdateUI;
        }

        private void Start()
        {
            UpdateUI();
        }

        private void OnButtonClicked()
        {
            if (CurrencyManager.Instance != null)
            {
                int soldValue = CurrencyManager.Instance.SellAllParts();

                if (soldValue > 0)
                {
                    Debug.Log($"부품 판매 완료: {soldValue}원");
                }
            }
        }

        private void UpdateUI()
        {
            if (CurrencyManager.Instance == null)
            {
                return;
            }

            int totalValue = CalculateTotalValue();

            if (_valueText != null)
            {
                Currency valueCurrency = totalValue;
                _valueText.text = $"판매 (${valueCurrency})";
            }

            if (_button != null)
            {
                _button.interactable = totalValue > 0;
            }
        }

        private int CalculateTotalValue()
        {
            int total = 0;
            total += (int)(CurrencyManager.Instance.Scrap.Value * 5);
            total += (int)(CurrencyManager.Instance.Glass.Value * 3);
            total += (int)(CurrencyManager.Instance.Plate.Value * 8);
            total += (int)(CurrencyManager.Instance.Rubber.Value * 4);
            return total;
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
