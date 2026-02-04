using System;

namespace JunkyardClicker.Core
{
    /// <summary>
    /// 업그레이드 서비스 인터페이스
    /// DIP를 위해 Manager 대신 인터페이스 사용
    /// </summary>
    public interface IUpgradeService
    {
        event Action OnUpgraded;

        int ToolLevel { get; }
        int WorkerLevel { get; }
        int ToolDamage { get; }
        int WorkerDps { get; }

        int GetLevel(EUpgradeType type);
        int GetToolDamage(int level);
        int GetWorkerDps(int level);
        int GetUpgradeCost(EUpgradeType type);
        bool IsMaxLevel(EUpgradeType type);
        bool TryUpgrade(EUpgradeType type);
    }
}
