using UnityEngine;
using TMPro;

namespace JunkyardClicker.UI
{
    using Core;
    using Resource;

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

        [SerializeField]
        private ResourceManager _resourceManager;

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void Start()
        {
            UpdateAllUI();
        }

        private void SubscribeToEvents()
        {
            GameEvents.OnMoneyChanged += UpdateMoneyUI;
            GameEvents.OnPartCollected += HandlePartCollected;

            if (_resourceManager != null)
            {
                _resourceManager.OnPartUpdated += UpdatePartUI;
            }
        }

        private void UnsubscribeFromEvents()
        {
            GameEvents.OnMoneyChanged -= UpdateMoneyUI;
            GameEvents.OnPartCollected -= HandlePartCollected;

            if (_resourceManager != null)
            {
                _resourceManager.OnPartUpdated -= UpdatePartUI;
            }
        }

        private void UpdateAllUI()
        {
            if (_resourceManager == null)
            {
                return;
            }

            UpdateMoneyUI(_resourceManager.Money);
            UpdatePartUI(PartType.Scrap, _resourceManager.GetPartCount(PartType.Scrap));
            UpdatePartUI(PartType.Glass, _resourceManager.GetPartCount(PartType.Glass));
            UpdatePartUI(PartType.Plate, _resourceManager.GetPartCount(PartType.Plate));
            UpdatePartUI(PartType.Rubber, _resourceManager.GetPartCount(PartType.Rubber));
        }

        private void UpdateMoneyUI(int money)
        {
            if (_moneyText != null)
            {
                _moneyText.text = money.ToFormattedString();
            }
        }

        private void UpdatePartUI(PartType partType, int count)
        {
            TextMeshProUGUI targetText = GetTextForPartType(partType);

            if (targetText != null)
            {
                targetText.text = count.ToFormattedString();
            }
        }

        private void HandlePartCollected(PartType partType, int amount)
        {
            if (_resourceManager == null)
            {
                return;
            }

            int totalCount = _resourceManager.GetPartCount(partType);
            UpdatePartUI(partType, totalCount);
        }

        private TextMeshProUGUI GetTextForPartType(PartType partType)
        {
            return partType switch
            {
                PartType.Scrap => _scrapText,
                PartType.Glass => _glassText,
                PartType.Plate => _plateText,
                PartType.Rubber => _rubberText,
                _ => null
            };
        }
    }
}
