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

    private static readonly int[] s_toolDamageTable = new int[] { 1, 3, 8, 15, 30, 60, 120 };
    private static readonly int[] s_workerDpsTable = new int[] { 0, 1, 3, 8, 20, 50 };
    private static readonly int[] s_toolCostTable = new int[] { 0, 100, 500, 2000, 10000, 50000, 200000 };
    private static readonly int[] s_workerCostTable = new int[] { 0, 200, 1000, 5000, 25000, 100000 };

    private int[] _levels = new int[(int)EUpgradeType.Count];

    private IUpgradeRepository _repository;

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
        if (level < 0 || level >= s_toolDamageTable.Length)
        {
            return s_toolDamageTable[^1];
        }

        return s_toolDamageTable[level];
    }

    public int GetWorkerDps(int level)
    {
        if (level < 0 || level >= s_workerDpsTable.Length)
        {
            return s_workerDpsTable[^1];
        }

        return s_workerDpsTable[level];
    }

    public int GetUpgradeCost(EUpgradeType type)
    {
        int level = _levels[(int)type];

        return type switch
        {
            EUpgradeType.Tool => GetToolUpgradeCost(level),
            EUpgradeType.Worker => GetWorkerUpgradeCost(level),
            _ => int.MaxValue
        };
    }

    private int GetToolUpgradeCost(int currentLevel)
    {
        int nextLevel = currentLevel + 1;

        if (nextLevel >= s_toolCostTable.Length)
        {
            return int.MaxValue;
        }

        return s_toolCostTable[nextLevel];
    }

    private int GetWorkerUpgradeCost(int currentLevel)
    {
        int nextLevel = currentLevel + 1;

        if (nextLevel >= s_workerCostTable.Length)
        {
            return int.MaxValue;
        }

        return s_workerCostTable[nextLevel];
    }

    public bool IsMaxLevel(EUpgradeType type)
    {
        int level = _levels[(int)type];

        return type switch
        {
            EUpgradeType.Tool => level >= s_toolDamageTable.Length - 1,
            EUpgradeType.Worker => level >= s_workerDpsTable.Length - 1,
            _ => true
        };
    }

    public bool TryUpgrade(EUpgradeType type)
    {
        if (IsMaxLevel(type))
        {
            return false;
        }

        int cost = GetUpgradeCost(type);

        if (CurrencyManager.Instance.TrySpend(ECurrencyType.Money, cost))
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
