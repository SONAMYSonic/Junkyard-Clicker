using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using JunkyardClicker.Core;

// 업그레이드 관리자 - IUpgradeService 구현
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

    public bool IsDataLoaded { get; private set; }

    // 싱글톤 설정 및 서비스 등록
    private void Awake()
    {
        Instance = this;
        _repository = new FirebaseUpgradeRepository();
        ServiceLocator.Register<IUpgradeService>(this);

        // CurrencyService 의존성 - Start에서 다시 시도
        ServiceLocator.TryGet<ICurrencyService>(out _currencyService);

        ValidateConfig();
    }

    // Config 유효성 검사
    private void ValidateConfig()
    {
        if (_config == null)
        {
            Debug.LogError("[UpgradeManager] UpgradeConfig가 할당되지 않았습니다! Inspector에서 설정해주세요.");
        }
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
            Debug.LogError($"[UpgradeManager] 초기화 실패: {e.Message}");
            // 기본값으로 초기화
            InitializeWithDefaults();
        }
    }

    // 저장된 데이터 로드
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

    // 기본값으로 초기화
    private void InitializeWithDefaults()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            _levels[i] = 0;
        }

        IsDataLoaded = true;
        OnUpgraded?.Invoke();
    }

    // 업그레이드 레벨 조회
    public int GetLevel(EUpgradeType type)
    {
        return _levels[(int)type];
    }

    // 도구 데미지 반환
    public int GetToolDamage(int level)
    {
        if (_config == null) return 1;
        return _config.GetToolDamage(level);
    }

    // 작업자 초당 데미지 반환
    public int GetWorkerDps(int level)
    {
        if (_config == null) return 0;
        return _config.GetWorkerDps(level);
    }

    // 업그레이드 비용 반환
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

    // 최대 레벨 여부 확인
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

    // 업그레이드 시도
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

    // Firebase에 저장
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
