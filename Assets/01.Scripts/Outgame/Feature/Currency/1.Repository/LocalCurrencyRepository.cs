using Cysharp.Threading.Tasks;
using UnityEngine;

public class LocalCurrencyRepository : ICurrencyRepository
{
    private const string KeyPrefix = "Currency_";

    public UniTask Save(CurrencySaveData saveData)
    {
        for (int i = 0; i < (int)ECurrencyType.Count; i++)
        {
            var type = (ECurrencyType)i;
            string key = KeyPrefix + type.ToString();
            PlayerPrefs.SetString(key, saveData.Currencies[i].ToString("G17"));
        }

        PlayerPrefs.Save();
        return default;
    }

    public UniTask<CurrencySaveData> Load()
    {
        CurrencySaveData data = CurrencySaveData.Default;

        for (int i = 0; i < (int)ECurrencyType.Count; i++)
        {
            var type = (ECurrencyType)i;
            string key = KeyPrefix + type.ToString();

            if (PlayerPrefs.HasKey(key))
            {
                data.Currencies[i] = double.Parse(PlayerPrefs.GetString(key, "0"));
            }
        }

        return UniTask.FromResult(data);
    }
}
