namespace JunkyardClicker.Ingame.Car
{
    /// <summary>
    /// 차량 상태를 나타내는 도메인 객체
    /// </summary>
    public class CarState
    {
        public int CurrentHp { get; private set; }
        public int MaxHp { get; private set; }
        public int DestroyedPartsCount { get; private set; }
        public bool IsDestroyed => CurrentHp <= 0;
        public float HpRatio => MaxHp > 0 ? (float)CurrentHp / MaxHp : 0f;

        public CarState(int maxHp)
        {
            MaxHp = maxHp;
            CurrentHp = maxHp;
            DestroyedPartsCount = 0;
        }

        public int ApplyDamage(int damage)
        {
            if (IsDestroyed || damage <= 0)
            {
                return 0;
            }

            int actualDamage = damage;

            if (damage > CurrentHp)
            {
                actualDamage = CurrentHp;
            }

            CurrentHp -= actualDamage;

            if (CurrentHp < 0)
            {
                CurrentHp = 0;
            }

            return actualDamage;
        }

        public void IncrementDestroyedParts()
        {
            DestroyedPartsCount++;
        }

        public void Reset(int maxHp)
        {
            MaxHp = maxHp;
            CurrentHp = maxHp;
            DestroyedPartsCount = 0;
        }
    }
}
