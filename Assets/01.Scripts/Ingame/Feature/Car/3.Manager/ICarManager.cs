using System;

namespace JunkyardClicker.Car
{
    using JunkyardClicker.Core;

    /// <summary>
    /// 차량 관리자 인터페이스
    /// 차량 스폰 및 현재 차량 조회만 담당 (SRP)
    /// </summary>
    public interface ICarManager
    {
        event Action<Car> OnCarSpawned;
        event Action<int> OnCarDestroyed;

        Car CurrentCar { get; }
        bool HasActiveCar { get; }

        void SpawnRandomCar();
        void SpawnCar(CarData carData);
    }
}
