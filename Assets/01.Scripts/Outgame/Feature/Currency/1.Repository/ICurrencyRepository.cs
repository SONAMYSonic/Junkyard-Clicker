using Cysharp.Threading.Tasks;

public interface ICurrencyRepository
{
    public UniTaskVoid Save(CurrencySaveData saveData);
    // 값 저장(리스트 아님!) 클래스를 저장해라
    
    public UniTask<CurrencySaveData> Load();
}
