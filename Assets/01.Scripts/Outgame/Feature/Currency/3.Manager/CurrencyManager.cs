using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public static event Action OnDataChanged;

    private Currency[] _currencies = new Currency[(int)ECurrencyType.Count];

    private ICurrencyRepository _repository;

    private void Awake()
    {
        Instance = this;
        _repository = new LocalCurrencyRepository();
    }

    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        double[] currencyValues = _repository.Load().Currencies;

        for (int i = 0; i < _currencies.Length; i++)
        {
            _currencies[i] = currencyValues[i];
        }
    }

    public Currency Get(ECurrencyType currencyType)
    {
        return _currencies[(int)currencyType];
    }

    public Currency Money => Get(ECurrencyType.Money);
    public Currency Scrap => Get(ECurrencyType.Scrap);
    public Currency Glass => Get(ECurrencyType.Glass);
    public Currency Plate => Get(ECurrencyType.Plate);
    public Currency Rubber => Get(ECurrencyType.Rubber);

    public void Add(ECurrencyType type, Currency amount)
    {
        _currencies[(int)type] += amount;
        Save();
        OnDataChanged?.Invoke();
    }

    public void Add(ECurrencyType type, double amount)
    {
        Add(type, new Currency(amount));
    }

    public bool TrySpend(ECurrencyType type, Currency amount)
    {
        if (_currencies[(int)type] >= amount)
        {
            _currencies[(int)type] -= amount;
            Save();
            OnDataChanged?.Invoke();
            return true;
        }

        return false;
    }

    public bool CanAfford(ECurrencyType type, Currency amount)
    {
        return _currencies[(int)type] >= amount;
    }

    public int SellAllParts()
    {
        int totalValue = 0;

        totalValue += (int)(Scrap.Value * 5);
        totalValue += (int)(Glass.Value * 3);
        totalValue += (int)(Plate.Value * 8);
        totalValue += (int)(Rubber.Value * 4);

        _currencies[(int)ECurrencyType.Scrap] = 0;
        _currencies[(int)ECurrencyType.Glass] = 0;
        _currencies[(int)ECurrencyType.Plate] = 0;
        _currencies[(int)ECurrencyType.Rubber] = 0;

        if (totalValue > 0)
        {
            Add(ECurrencyType.Money, totalValue);
        }

        return totalValue;
    }

    private void Save()
    {
        var saveData = new CurrencySaveData
        {
            Currencies = new double[_currencies.Length]
        };

        for (int i = 0; i < _currencies.Length; i++)
        {
            saveData.Currencies[i] = (double)_currencies[i];
        }

        _repository.Save(saveData);
    }
}
