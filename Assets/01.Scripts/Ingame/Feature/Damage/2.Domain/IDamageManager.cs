using System;

namespace JunkyardClicker.Ingame.Damage
{
    /// <summary>
    /// 데미지 시스템 매니저 인터페이스
    /// </summary>
    public interface IDamageManager
    {
        event Action<DamageInfo> OnDamageApplied;

        void ApplyClickDamage(UnityEngine.Vector2 worldPosition);
        void ApplyAutoDamage();
    }
}
