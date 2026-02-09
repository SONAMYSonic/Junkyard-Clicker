using UnityEngine;

namespace JunkyardClicker.Core
{
    // 차량 파츠 데이터 ScriptableObject
    [CreateAssetMenu(fileName = "NewCarPartData", menuName = "JunkyardClicker/Car Part Data")]
    public class CarPartData : ScriptableObject
    {
        [SerializeField]
        private CarPartType _partType;

        [SerializeField]
        [Range(0f, 1f)]
        private float _hpRatio;

        [SerializeField]
        private PartDropInfo[] _drops;

        [SerializeField]
        private Sprite _normalSprite;

        [SerializeField]
        private Sprite _damagedSprite;

        [SerializeField]
        private Sprite _destroyedSprite;

        public CarPartType PartType => _partType;
        public float HpRatio => _hpRatio;
        public PartDropInfo[] Drops => _drops;
        public Sprite NormalSprite => _normalSprite;
        public Sprite DamagedSprite => _damagedSprite;
        public Sprite DestroyedSprite => _destroyedSprite;

        // 차량 HP 비율로 파츠 최대 HP 계산
        public int CalculateMaxHp(int carMaxHp)
        {
            return Mathf.RoundToInt(carMaxHp * _hpRatio);
        }

        // HP 비율에 따른 스프라이트 반환
        public Sprite GetSpriteForState(float hpRatio)
        {
            if (hpRatio <= 0f)
            {
                return _destroyedSprite;
            }

            if (hpRatio < 0.5f)
            {
                return _damagedSprite;
            }

            return _normalSprite;
        }
    }
}
