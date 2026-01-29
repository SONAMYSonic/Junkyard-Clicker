using UnityEngine;
using JunkyardClicker.Core;

public class GameEventBridge : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.OnCarDestroyed += HandleCarDestroyed;
        GameEvents.OnPartCollected += HandlePartCollected;
    }

    private void OnDisable()
    {
        GameEvents.OnCarDestroyed -= HandleCarDestroyed;
        GameEvents.OnPartCollected -= HandlePartCollected;
    }

    private void HandleCarDestroyed(int reward)
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.Add(ECurrencyType.Money, reward);
        }
    }

    private void HandlePartCollected(PartType partType, int amount)
    {
        if (CurrencyManager.Instance != null)
        {
            ECurrencyType currencyType = CurrencyTypeHelper.ToECurrencyType(partType);
            CurrencyManager.Instance.Add(currencyType, amount);
        }
    }
}
