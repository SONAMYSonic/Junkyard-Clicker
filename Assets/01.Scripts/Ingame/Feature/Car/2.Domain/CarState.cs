using System;
using JunkyardClicker.Core;

namespace JunkyardClicker.Car
{
    /// <summary>
    /// 차량 상태를 나타내는 도메인 객체
    /// 무결성 검사가 적용된 불변 상태 관리
    /// </summary>
    public class CarState
    {
        public int CurrentHp { get; private set; }
        public int MaxHp { get; private set; }
        public int DestroyedPartsCount { get; private set; }
        public bool IsDestroyed => CurrentHp <= 0;
        public float HpRatio => MaxHp > 0 ? (float)CurrentHp / MaxHp : 0f;

        /// <summary>
        /// 차량 상태 생성
        /// </summary>
        /// <param name="maxHp">최대 HP (1 이상)</param>
        /// <exception cref="ArgumentOutOfRangeException">maxHp가 0 이하일 경우</exception>
        public CarState(int maxHp)
        {
            Guard.Positive(maxHp, nameof(maxHp));

            MaxHp = maxHp;
            CurrentHp = maxHp;
            DestroyedPartsCount = 0;
        }

        /// <summary>
        /// 데미지 적용
        /// </summary>
        /// <param name="damage">데미지 양 (음수는 0으로 처리)</param>
        /// <returns>실제 적용된 데미지</returns>
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

        /// <summary>
        /// 파괴된 파츠 수 증가
        /// </summary>
        public void IncrementDestroyedParts()
        {
            DestroyedPartsCount++;
        }

        /// <summary>
        /// 상태 리셋
        /// </summary>
        /// <param name="maxHp">새로운 최대 HP (1 이상)</param>
        /// <exception cref="ArgumentOutOfRangeException">maxHp가 0 이하일 경우</exception>
        public void Reset(int maxHp)
        {
            Guard.Positive(maxHp, nameof(maxHp));

            MaxHp = maxHp;
            CurrentHp = maxHp;
            DestroyedPartsCount = 0;
        }
    }
}
