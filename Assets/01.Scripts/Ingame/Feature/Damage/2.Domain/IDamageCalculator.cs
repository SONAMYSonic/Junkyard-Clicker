namespace JunkyardClicker.Ingame.Damage
{
    /// <summary>
    /// 데미지 계산 인터페이스
    /// </summary>
    public interface IDamageCalculator
    {
        int CalculateClickDamage();
        int CalculateAutoDamage();
    }
}
