using Cysharp.Threading.Tasks;

public interface IUpgradeRepository
{
    UniTaskVoid Save(UpgradeSaveData saveData);
    UniTask<UpgradeSaveData> Load();
}
