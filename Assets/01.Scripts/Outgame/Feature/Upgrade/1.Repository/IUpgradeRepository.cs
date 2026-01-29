public interface IUpgradeRepository
{
    void Save(UpgradeSaveData saveData);
    UpgradeSaveData Load();
}
