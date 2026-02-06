using Cysharp.Threading.Tasks;
using UnityEngine;

public class LocalUpgradeRepository : IUpgradeRepository
{
    private const string KeyPrefix = "Upgrade_";

    public UniTask Save(UpgradeSaveData saveData)
    {
        for (int i = 0; i < (int)EUpgradeType.Count; i++)
        {
            var type = (EUpgradeType)i;
            string key = KeyPrefix + type.ToString();
            PlayerPrefs.SetInt(key, saveData.Levels[i]);
        }

        PlayerPrefs.Save();
        return default;
    }

    public UniTask<UpgradeSaveData> Load()
    {
        UpgradeSaveData data = UpgradeSaveData.Default;

        for (int i = 0; i < (int)EUpgradeType.Count; i++)
        {
            var type = (EUpgradeType)i;
            string key = KeyPrefix + type.ToString();

            if (PlayerPrefs.HasKey(key))
            {
                data.Levels[i] = PlayerPrefs.GetInt(key, 0);
            }
        }

        return UniTask.FromResult(data);
    }
}
