public interface ICurrencyRepository
{
    void Save(CurrencySaveData saveData);
    CurrencySaveData Load();
}
