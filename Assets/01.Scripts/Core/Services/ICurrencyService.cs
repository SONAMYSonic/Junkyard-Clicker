using System;

namespace JunkyardClicker.Core
{
    /// <summary>
    /// 재화 서비스 인터페이스
    /// DIP를 위해 Manager 대신 인터페이스 사용
    /// </summary>
    public interface ICurrencyService
    {
        event Action OnDataChanged;

        Currency Money { get; }
        Currency Scrap { get; }
        Currency Glass { get; }
        Currency Plate { get; }
        Currency Rubber { get; }

        Currency Get(ECurrencyType currencyType);
        void Add(ECurrencyType type, Currency amount);
        void Add(ECurrencyType type, double amount);
        bool TrySpend(ECurrencyType type, Currency amount);
        bool CanAfford(ECurrencyType type, Currency amount);
        int SellAllParts();
    }
}
