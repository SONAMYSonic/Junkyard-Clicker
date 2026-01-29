using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JunkyardClicker.UI
{
    using Resource;

    public class SellPartsButtonUI : MonoBehaviour
    {
        [SerializeField]
        private Button _sellButton;

        [SerializeField]
        private TextMeshProUGUI _buttonText;

        [SerializeField]
        private ResourceManager _resourceManager;

        private void Awake()
        {
            if (_sellButton != null)
            {
                _sellButton.onClick.AddListener(OnSellButtonClicked);
            }
        }

        private void OnSellButtonClicked()
        {
            if (_resourceManager == null)
            {
                return;
            }

            int earnedMoney = _resourceManager.SellAllParts();

            if (earnedMoney > 0 && _buttonText != null)
            {
                StartCoroutine(ShowSoldFeedback(earnedMoney));
            }
        }

        private System.Collections.IEnumerator ShowSoldFeedback(int amount)
        {
            string originalText = _buttonText.text;
            _buttonText.text = $"+${amount}!";

            yield return new WaitForSeconds(1f);

            _buttonText.text = originalText;
        }

        private void OnDestroy()
        {
            if (_sellButton != null)
            {
                _sellButton.onClick.RemoveListener(OnSellButtonClicked);
            }
        }
    }
}
