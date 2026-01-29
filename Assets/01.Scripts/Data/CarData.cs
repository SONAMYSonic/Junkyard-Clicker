using UnityEngine;

namespace JunkyardClicker.Core
{
    [CreateAssetMenu(fileName = "NewCarData", menuName = "JunkyardClicker/Car Data")]
    public class CarData : ScriptableObject
    {
        [SerializeField]
        private string _carName;

        [SerializeField]
        private CarGrade _grade;

        [SerializeField]
        private int _maxHp;

        [SerializeField]
        private int _baseReward;

        [SerializeField]
        private Sprite _baseSprite;

        [SerializeField]
        private CarPartData[] _partDataList;

        public string CarName => _carName;
        public CarGrade Grade => _grade;
        public int MaxHp => _maxHp;
        public int BaseReward => _baseReward;
        public Sprite BaseSprite => _baseSprite;
        public CarPartData[] PartDataList => _partDataList;

        public float GetSpawnWeight()
        {
            return _grade switch
            {
                CarGrade.Common => 60f,
                CarGrade.Rare => 30f,
                CarGrade.Epic => 8f,
                CarGrade.Legendary => 2f,
                _ => 60f
            };
        }

        public Color GetGradeColor()
        {
            return _grade switch
            {
                CarGrade.Common => Color.white,
                CarGrade.Rare => new Color(0.3f, 0.7f, 1f),
                CarGrade.Epic => new Color(0.8f, 0.3f, 1f),
                CarGrade.Legendary => new Color(1f, 0.8f, 0.2f),
                _ => Color.white
            };
        }
    }
}
