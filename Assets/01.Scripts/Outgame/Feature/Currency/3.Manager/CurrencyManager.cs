using System;
using UnityEngine;
using JunkyardClicker.Core;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public static event Action OnDataChanged;

    private Currency[] _currencies = new Currency[(int)ECurrencyType.Count];

    private ICurrencyRepository _repository;

    private void Awake()
    {
        Instance = this;
        _repository = new FirebaseCurrencyRepository();
    }

    private void OnEnable()
    {
        // 파츠 수집 이벤트 구독
        GameEvents.OnPartCollected += HandlePartCollected;
        GameEvents.OnCarDestroyed += HandleCarDestroyed;
    }

    private void OnDisable()
    {
        GameEvents.OnPartCollected -= HandlePartCollected;
        GameEvents.OnCarDestroyed -= HandleCarDestroyed;
    }

    private void Start()
    {
        LoadData();
    }

    private async void LoadData()
    {
        CurrencySaveData saveData = await _repository.Load();
        double[] currencyValues = saveData.Currencies;

        for (int i = 0; i < _currencies.Length; i++)
        {
            _currencies[i] = currencyValues[i];
        }
    }

    private void HandlePartCollected(PartType partType, int amount)
    {
        // PartType을 ECurrencyType으로 변환하여 자원 추가
        ECurrencyType currencyType = ConvertPartTypeToCurrency(partType);
        Add(currencyType, amount);
    }

    private void HandleCarDestroyed(int reward)
    {
        // 차량 파괴 보상 (돈)
        Add(ECurrencyType.Money, reward);
    }

    private ECurrencyType ConvertPartTypeToCurrency(PartType partType)
    {
        return partType switch
        {
            PartType.Scrap => ECurrencyType.Scrap,
            PartType.Glass => ECurrencyType.Glass,
            PartType.Plate => ECurrencyType.Plate,
            PartType.Rubber => ECurrencyType.Rubber,
            _ => ECurrencyType.Scrap
        };
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
