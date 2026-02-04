using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using JunkyardClicker.Core;

/// <summary>
/// 재화 관리자
/// ICurrencyService 인터페이스를 구현하여 DIP 준수
/// Firebase 초기화 완료 후 데이터 로드
/// </summary>
public class CurrencyManager : MonoBehaviour, ICurrencyService
{
    public static CurrencyManager Instance { get; private set; }

    public static event Action OnDataChanged;

    // ICurrencyService 인터페이스 이벤트 구현
    event Action ICurrencyService.OnDataChanged
    {
        add => OnDataChanged += value;
        remove => OnDataChanged -= value;
    }

    private Currency[] _currencies = new Currency[(int)ECurrencyType.Count];

    private ICurrencyRepository _repository;

    /// <summary>
    /// 데이터 로드 완료 여부
    /// </summary>
    public bool IsDataLoaded { get; private set; }

    private void Awake()
    {
        Instance = this;
        _repository = new FirebaseCurrencyRepository();
        ServiceLocator.Register<ICurrencyService>(this);
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
        InitializeAsync().Forget();
    }

    /// <summary>
    /// 비동기 초기화 - Firebase 준비 후 데이터 로드
    /// </summary>
    private async UniTaskVoid InitializeAsync()
    {
        try
        {
            // Firebase 초기화 완료 대기
            await FirebaseInitializer.InitializationTask;

            await LoadDataAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"[CurrencyManager] 초기화 실패: {e.Message}");
            // 기본값으로 초기화
            InitializeWithDefaults();
        }
    }

    private async UniTask LoadDataAsync()
    {
        try
        {
            CurrencySaveData saveData = await _repository.Load();

            if (saveData?.Currencies == null || saveData.Currencies.Length == 0)
            {
                Debug.LogWarning("[CurrencyManager] 저장 데이터 없음, 기본값 사용");
                InitializeWithDefaults();
                return;
            }

            double[] currencyValues = saveData.Currencies;

            for (int i = 0; i < _currencies.Length; i++)
            {
                _currencies[i] = i < currencyValues.Length ? currencyValues[i] : 0;
            }

            IsDataLoaded = true;
            OnDataChanged?.Invoke();
            Debug.Log("[CurrencyManager] 데이터 로드 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"[CurrencyManager] 데이터 로드 실패: {e.Message}");
            InitializeWithDefaults();
        }
    }

    private void InitializeWithDefaults()
    {
        for (int i = 0; i < _currencies.Length; i++)
        {
            _currencies[i] = 0;
        }

        IsDataLoaded = true;
        OnDataChanged?.Invoke();
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
