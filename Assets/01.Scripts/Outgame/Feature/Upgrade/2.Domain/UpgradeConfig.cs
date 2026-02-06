using UnityEngine;

/// <summary>
/// 업그레이드 밸런스 테이블 설정
/// ScriptableObject로 분리하여 에디터에서 수정 가능
/// </summary>
[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "JunkyardClicker/Upgrade Config")]
public class UpgradeConfig : ScriptableObject
{
    [Header("Tool Upgrade")]
    [Tooltip("레벨별 클릭 데미지 (index = level)")]
    [SerializeField]
    private int[] _toolDamageTable = { 1, 3, 8, 15, 30, 60, 120 };

    [Tooltip("레벨별 업그레이드 비용 (index = level, 0은 초기 비용 없음)")]
    [SerializeField]
    private int[] _toolCostTable = { 0, 100, 500, 2000, 10000, 50000, 200000 };

    [Header("Worker Upgrade")]
    [Tooltip("레벨별 초당 자동 데미지 (index = level)")]
    [SerializeField]
    private int[] _workerDpsTable = { 0, 1, 3, 8, 20, 50 };

    [Tooltip("레벨별 업그레이드 비용 (index = level, 0은 초기 비용 없음)")]
    [SerializeField]
    private int[] _workerCostTable = { 0, 200, 1000, 5000, 25000, 100000 };

    // 읽기 전용 프로퍼티
    public int[] ToolDamageTable => _toolDamageTable;
    public int[] ToolCostTable => _toolCostTable;
    public int[] WorkerDpsTable => _workerDpsTable;
    public int[] WorkerCostTable => _workerCostTable;

    public int ToolMaxLevel => _toolDamageTable.Length - 1;
    public int WorkerMaxLevel => _workerDpsTable.Length - 1;

    /// <summary>
    /// 레벨에 해당하는 Tool 데미지 반환
    /// </summary>
    public int GetToolDamage(int level)
    {
        if (level < 0 || level >= _toolDamageTable.Length)
        {
            return _toolDamageTable[^1];
        }
        return _toolDamageTable[level];
    }

    /// <summary>
    /// 레벨에 해당하는 Worker DPS 반환
    /// </summary>
    public int GetWorkerDps(int level)
    {
        if (level < 0 || level >= _workerDpsTable.Length)
        {
            return _workerDpsTable[^1];
        }
        return _workerDpsTable[level];
    }

    /// <summary>
    /// 다음 레벨 업그레이드 비용 반환
    /// </summary>
    public int GetToolUpgradeCost(int currentLevel)
    {
        int nextLevel = currentLevel + 1;
        if (nextLevel >= _toolCostTable.Length)
        {
            return int.MaxValue;
        }
        return _toolCostTable[nextLevel];
    }

    /// <summary>
    /// 다음 레벨 업그레이드 비용 반환
    /// </summary>
    public int GetWorkerUpgradeCost(int currentLevel)
    {
        int nextLevel = currentLevel + 1;
        if (nextLevel >= _workerCostTable.Length)
        {
            return int.MaxValue;
        }
        return _workerCostTable[nextLevel];
    }
}
