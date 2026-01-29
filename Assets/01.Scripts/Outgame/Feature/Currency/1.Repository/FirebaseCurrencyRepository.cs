public class FirebaseCurrencyRepository : ICurrencyRepository
{
    public void Save(CurrencySaveData saveData)
    {
        // TODO: Firebase 연동 시 구현
    }

    public CurrencySaveData Load()
    {
        // TODO: Firebase 연동 시 구현
        return CurrencySaveData.Default;
    }
}
