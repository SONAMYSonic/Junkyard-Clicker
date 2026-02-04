namespace JunkyardClicker.Ingame.Damage
{
    /// <summary>
    /// 업그레이드 레벨에 기반한 데미지 계산
    /// </summary>
    public class UpgradeBasedDamageCalculator : IDamageCalculator
    {
        public int CalculateClickDamage()
        {
            if (NewUpgradeManager.Instance == null)
            {
                return 1;
            }

            return NewUpgradeManager.Instance.ToolDamage;
        }

        public int CalculateAutoDamage()
        {
            if (NewUpgradeManager.Instance == null)
            {
                return 0;
            }

            return NewUpgradeManager.Instance.WorkerDps;
        }
    }
}
