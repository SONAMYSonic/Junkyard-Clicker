using Cysharp.Threading.Tasks;

public interface ICurrencyRepository
{
    UniTask Save(CurrencySaveData saveData);
    UniTask<CurrencySaveData> Load();
}
