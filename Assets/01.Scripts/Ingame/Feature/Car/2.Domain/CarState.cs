using System;
using JunkyardClicker.Core;

namespace JunkyardClicker.Car
{
    // 차량 상태를 나타내는 도메인 객체
    public class CarState
    {
        public int CurrentHp { get; private set; }
        public int MaxHp { get; private set; }
        public int DestroyedPartsCount { get; private set; }
        public bool IsDestroyed => CurrentHp <= 0;
        public float HpRatio => MaxHp > 0 ? (float)CurrentHp / MaxHp : 0f;

        // 차량 상태 생성자
        public CarState(int maxHp)
        {
            Guard.Positive(maxHp, nameof(maxHp));

            MaxHp = maxHp;
            CurrentHp = maxHp;
            DestroyedPartsCount = 0;
        }

        // 데미지 적용 후 실제 적용된 데미지 반환
        public int ApplyDamage(int damage)
        {
            // 이미 파괴되었거나 데미지가 0 이하면 무시
            if (IsDestroyed || damage <= 0)
            {
                return 0;
            }

            // 실제 데미지 계산 (현재 HP를 초과할 수 없음)
            int actualDamage = Math.Min(damage, CurrentHp);
            CurrentHp -= actualDamage;

            // 안전장치: HP가 음수가 되지 않도록
            CurrentHp = Math.Max(0, CurrentHp);

            return actualDamage;
        }

        // 파괴된 파츠 수 증가
        public void IncrementDestroyedParts()
        {
            DestroyedPartsCount++;
        }

        // 상태 리셋
        public void Reset(int maxHp)
        {
            Guard.Positive(maxHp, nameof(maxHp));

            MaxHp = maxHp;
            CurrentHp = maxHp;
            DestroyedPartsCount = 0;
        }
    }
}
