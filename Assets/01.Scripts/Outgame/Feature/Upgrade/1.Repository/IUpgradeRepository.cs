using Cysharp.Threading.Tasks;

public interface IUpgradeRepository
{
    UniTask Save(UpgradeSaveData saveData);
    UniTask<UpgradeSaveData> Load();
}
