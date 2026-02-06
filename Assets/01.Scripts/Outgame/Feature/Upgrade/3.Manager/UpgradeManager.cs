using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using JunkyardClicker.Core;

/// <summary>
/// 업그레이드 관리자
/// IUpgradeService 인터페이스를 구현하여 DIP 준수
/// Firebase 초기화 완료 후 데이터 로드
/// </summary>
public class UpgradeManager : MonoBehaviour, IUpgradeService
{
    public static UpgradeManager Instance { get; private set; }

    public static event Action OnUpgraded;

    // IUpgradeService 인터페이스 이벤트 구현
    event Action IUpgradeService.OnUpgraded
    {
        add => OnUpgraded += value;
        remove => OnUpgraded -= value;
    }

    [SerializeField]
    private UpgradeConfig _config;

    private int[] _levels = new int[(int)EUpgradeType.Count];

    private IUpgradeRepository _repository;
    private ICurrencyService _currencyService;

    public int ToolLevel => _levels[(int)EUpgradeType.Tool];
    public int WorkerLevel => _levels[(int)EUpgradeType.Worker];

    public int ToolDamage => GetToolDamage(ToolLevel);
    public int WorkerDps => GetWorkerDps(WorkerLevel);

    /// <summary>
    /// 데이터 로드 완료 여부
    /// </summary>
    public bool IsDataLoaded { get; private set; }

    private void Awake()
    {
        Instance = this;
        _repository = new FirebaseUpgradeRepository();
        ServiceLocator.Register<IUpgradeService>(this);

        // CurrencyService 의존성 - Start에서 다시 시도
        ServiceLocator.TryGet<ICurrencyService>(out _currencyService);

        ValidateConfig();
    }

    private void ValidateConfig()
    {
        if (_config == null)
        {
            Debug.LogError("[UpgradeManager] UpgradeConfig가 할당되지 않았습니다! Inspector에서 설정해주세요.");
        }
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
            Debug.LogError($"[UpgradeManager] 초기화 실패: {e.Message}");
            // 기본값으로 초기화
            InitializeWithDefaults();
        }
    }

    private async UniTask LoadDataAsync()
    {
        try
        {
            UpgradeSaveData saveData = await _repository.Load();

            if (saveData?.Levels == null || saveData.Levels.Length == 0)
            {
                Debug.LogWarning("[UpgradeManager] 저장 데이터 없음, 기본값 사용");
                InitializeWithDefaults();
                return;
            }

            int[] levelValues = saveData.Levels;

            for (int i = 0; i < _levels.Length; i++)
            {
                _levels[i] = i < levelValues.Length ? levelValues[i] : 0;
            }

            IsDataLoaded = true;
            OnUpgraded?.Invoke();
            Debug.Log("[UpgradeManager] 데이터 로드 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"[UpgradeManager] 데이터 로드 실패: {e.Message}");
            InitializeWithDefaults();
        }
    }

    private void InitializeWithDefaults()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            _levels[i] = 0;
        }

        IsDataLoaded = true;
        OnUpgraded?.Invoke();
    }

    public int GetLevel(EUpgradeType type)
    {
        return _levels[(int)type];
    }

    public int GetToolDamage(int level)
    {
        if (_config == null) return 1;
        return _config.GetToolDamage(level);
    }

    public int GetWorkerDps(int level)
    {
        if (_config == null) return 0;
        return _config.GetWorkerDps(level);
    }

    public int GetUpgradeCost(EUpgradeType type)
    {
        if (_config == null) return int.MaxValue;

        int level = _levels[(int)type];

        return type switch
        {
            EUpgradeType.Tool => _config.GetToolUpgradeCost(level),
            EUpgradeType.Worker => _config.GetWorkerUpgradeCost(level),
            _ => int.MaxValue
        };
    }

    public bool IsMaxLevel(EUpgradeType type)
    {
        if (_config == null) return true;

        int level = _levels[(int)type];

        return type switch
        {
            EUpgradeType.Tool => level >= _config.ToolMaxLevel,
            EUpgradeType.Worker => level >= _config.WorkerMaxLevel,
            _ => true
        };
    }

    public bool TryUpgrade(EUpgradeType type)
    {
        if (IsMaxLevel(type))
        {
            return false;
        }

        // CurrencyService 지연 로딩 (Awake 시점에 없을 수 있음)
        if (_currencyService == null)
        {
            ServiceLocator.TryGet<ICurrencyService>(out _currencyService);
        }

        if (_currencyService == null)
        {
            Debug.LogWarning("[UpgradeManager] CurrencyService를 찾을 수 없습니다.");
            return false;
        }

        int cost = GetUpgradeCost(type);

        if (_currencyService.TrySpend(ECurrencyType.Money, cost))
        {
            _levels[(int)type]++;
            Save();
            OnUpgraded?.Invoke();
            return true;
        }

        return false;
    }

    private void Save()
    {
        var saveData = new UpgradeSaveData
        {
            Levels = new int[_levels.Length]
        };

        for (int i = 0; i < _levels.Length; i++)
        {
            saveData.Levels[i] = _levels[i];
        }

        _repository.Save(saveData);
    }
}
