public class FirebaseUpgradeRepository : IUpgradeRepository
{
    public void Save(UpgradeSaveData saveData)
    {
        // TODO: Firebase 연동 시 구현
    }

    public UpgradeSaveData Load()
    {
        // TODO: Firebase 연동 시 구현
        return UpgradeSaveData.Default;
    }
}
