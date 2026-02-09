using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using JunkyardClicker.Core;

// 재화 관리자 - ICurrencyService 구현
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

    // 데이터 로드 전 발생한 보상 대기열
    private readonly List<(PartType partType, int amount)> _pendingPartRewards = new();
    private readonly List<int> _pendingMoneyRewards = new();

    public bool IsDataLoaded { get; private set; }

    // 싱글톤 설정 및 서비스 등록
    private void Awake()
    {
        Instance = this;
#if UNITY_WEBGL
        _repository = new LocalCurrencyRepository();
#else
        _repository = new FirebaseCurrencyRepository();
#endif
        ServiceLocator.Register<ICurrencyService>(this);
    }

    // 이벤트 구독
    private void OnEnable()
    {
        // 파츠 수집 이벤트 구독
        GameEvents.OnPartCollected += HandlePartCollected;
        GameEvents.OnCarDestroyed += HandleCarDestroyed;
    }

    // 이벤트 구독 해제
    private void OnDisable()
    {
        GameEvents.OnPartCollected -= HandlePartCollected;
        GameEvents.OnCarDestroyed -= HandleCarDestroyed;
    }

    // 초기화 시작
    private void Start()
    {
        InitializeAsync().Forget();
    }

    // 비동기 초기화 - Firebase 준비 후 데이터 로드
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

    // 저장된 데이터 로드
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
            ProcessPendingRewards();
            OnDataChanged?.Invoke();
            Debug.Log("[CurrencyManager] 데이터 로드 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"[CurrencyManager] 데이터 로드 실패: {e.Message}");
            InitializeWithDefaults();
        }
    }

    // 기본값으로 초기화
    private void InitializeWithDefaults()
    {
        for (int i = 0; i < _currencies.Length; i++)
        {
            _currencies[i] = 0;
        }

        IsDataLoaded = true;
        ProcessPendingRewards();
        OnDataChanged?.Invoke();
    }

    // 데이터 로드 전 발생한 보상 처리
    private void ProcessPendingRewards()
    {
        // 대기열이 비어있으면 리턴
        if (_pendingPartRewards.Count == 0 && _pendingMoneyRewards.Count == 0)
        {
            return;
        }

        // 파츠 보상 처리
        foreach (var (partType, amount) in _pendingPartRewards)
        {
            ECurrencyType currencyType = CurrencyTypeHelper.ToECurrencyType(partType);
            _currencies[(int)currencyType] += amount;
        }
        _pendingPartRewards.Clear();

        // 돈 보상 처리
        foreach (int reward in _pendingMoneyRewards)
        {
            _currencies[(int)ECurrencyType.Money] += reward;
        }
        _pendingMoneyRewards.Clear();

        // 대기열 처리 후 저장
        Save();
    }

    // 파츠 수집 이벤트 처리
    private void HandlePartCollected(PartType partType, int amount)
    {
        // 데이터 로드 전이면 대기열에 추가
        if (!IsDataLoaded)
        {
            _pendingPartRewards.Add((partType, amount));
            return;
        }

        // PartType을 ECurrencyType으로 변환하여 자원 추가
        ECurrencyType currencyType = CurrencyTypeHelper.ToECurrencyType(partType);
        Add(currencyType, amount);
    }

    // 차량 파괴 이벤트 처리
    private void HandleCarDestroyed(int reward)
    {
        // 데이터 로드 전이면 대기열에 추가
        if (!IsDataLoaded)
        {
            _pendingMoneyRewards.Add(reward);
            return;
        }

        // 차량 파괴 보상 (돈)
        Add(ECurrencyType.Money, reward);
    }

    // 재화 조회
    public Currency Get(ECurrencyType currencyType)
    {
        int index = (int)currencyType;
        if (index < 0 || index >= _currencies.Length)
        {
            Debug.LogError($"[CurrencyManager] 잘못된 재화 타입: {currencyType}");
            return Currency.Zero;
        }
        return _currencies[index];
    }

    public Currency Money => Get(ECurrencyType.Money);
    public Currency Scrap => Get(ECurrencyType.Scrap);
    public Currency Glass => Get(ECurrencyType.Glass);
    public Currency Plate => Get(ECurrencyType.Plate);
    public Currency Rubber => Get(ECurrencyType.Rubber);

    // 재화 추가
    public void Add(ECurrencyType type, Currency amount)
    {
        int index = (int)type;
        if (index < 0 || index >= _currencies.Length)
        {
            Debug.LogError($"[CurrencyManager] 잘못된 재화 타입: {type}");
            return;
        }
        _currencies[index] += amount;
        Save();
        OnDataChanged?.Invoke();
    }

    // 재화 추가 (double 오버로드)
    public void Add(ECurrencyType type, double amount)
    {
        Add(type, new Currency(amount));
    }

    // 재화 사용 시도
    public bool TrySpend(ECurrencyType type, Currency amount)
    {
        int index = (int)type;
        if (index < 0 || index >= _currencies.Length)
        {
            Debug.LogError($"[CurrencyManager] 잘못된 재화 타입: {type}");
            return false;
        }

        if (_currencies[index] >= amount)
        {
            _currencies[index] -= amount;
            Save();
            OnDataChanged?.Invoke();
            return true;
        }

        return false;
    }

    // 구매 가능 여부 확인
    public bool CanAfford(ECurrencyType type, Currency amount)
    {
        int index = (int)type;
        if (index < 0 || index >= _currencies.Length)
        {
            return false;
        }
        return _currencies[index] >= amount;
    }

    // 모든 파츠 판매
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

    // Firebase에 저장
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
