namespace JunkyardClicker.Car
{
    using JunkyardClicker.Core;

    // 차량 파츠 상태를 나타내는 도메인 객체
    public class CarPartState
    {
        public CarPartType PartType { get; private set; }
        public int CurrentHp { get; private set; }
        public int MaxHp { get; private set; }
        public bool IsDestroyed => CurrentHp <= 0;
        public float HpRatio => MaxHp > 0 ? (float)CurrentHp / MaxHp : 0f;

        // 파츠 상태 생성자
        public CarPartState(CarPartType partType, int maxHp)
        {
            Guard.Positive(maxHp, nameof(maxHp));
            PartType = partType;
            MaxHp = maxHp;
            CurrentHp = maxHp;
        }

        // 데미지를 적용하고 실제 적용된 데미지를 반환
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

        // HP를 최대값으로 리셋
        public void Reset(int maxHp)
        {
            MaxHp = maxHp;
            CurrentHp = maxHp;
        }
    }
}
