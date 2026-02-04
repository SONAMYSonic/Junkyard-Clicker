using UnityEngine;

namespace JunkyardClicker.Resource
{
    /// <summary>
    /// 데미지 정보를 나타내는 값 객체
    /// </summary>
    public readonly struct DamageInfo
    {
        public int Amount { get; }
        public Vector2 Position { get; }
        public DamageSource Source { get; }

        public DamageInfo(int amount, Vector2 position, DamageSource source)
        {
            Amount = amount;
            Position = position;
            Source = source;
        }

        public DamageInfo(int amount, DamageSource source)
            : this(amount, Vector2.zero, source)
        {
        }
    }

    public enum DamageSource
    {
        Click,
        Auto
    }
}
