using UnityEngine;
using TMPro;

namespace JunkyardClicker.UI
{
    public class ResourceUI : MonoBehaviour
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

        private void OnEnable()
        {
            CurrencyManager.OnDataChanged += UpdateAllUI;
        }

        private void OnDisable()
        {
            CurrencyManager.OnDataChanged -= UpdateAllUI;
        }

        private void Start()
        {
            UpdateAllUI();
        }

        private void UpdateAllUI()
        {
            if (CurrencyManager.Instance == null)
            {
                return;
            }

            if (_moneyText != null)
            {
                _moneyText.text = CurrencyManager.Instance.Money.ToString();
            }

            if (_scrapText != null)
            {
                _scrapText.text = CurrencyManager.Instance.Scrap.ToString();
            }

            if (_glassText != null)
            {
                _glassText.text = CurrencyManager.Instance.Glass.ToString();
            }

            if (_plateText != null)
            {
                _plateText.text = CurrencyManager.Instance.Plate.ToString();
            }

            if (_rubberText != null)
            {
                _rubberText.text = CurrencyManager.Instance.Rubber.ToString();
            }
        }
    }
}
