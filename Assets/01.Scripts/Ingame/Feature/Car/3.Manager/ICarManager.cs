using System;

namespace JunkyardClicker.Ingame.Car
{
    using JunkyardClicker.Core;

    /// <summary>
    /// 차량 관리자 인터페이스
    /// </summary>
    public interface ICarManager
    {
        event Action<CarEntity> OnCarSpawned;
        event Action<int> OnCarDestroyed;
        event Action<int> OnDamageDealt;

        CarEntity CurrentCar { get; }
        bool HasActiveCar { get; }

        void SpawnRandomCar();
        void SpawnCar(CarData carData);
        void ApplyDamage(int damage);
        void ApplyDamageAtPosition(int damage, UnityEngine.Vector2 worldPosition);
    }
}
