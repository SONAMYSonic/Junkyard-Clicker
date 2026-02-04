using System;
using System.Collections.Generic;
using UnityEngine;

namespace JunkyardClicker.Core
{
    /// <summary>
    /// 타입 안전한 EventBus
    /// 이벤트 기반 느슨한 결합을 제공
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> s_handlers = new();

        /// <summary>
        /// 이벤트 구독
        /// </summary>
        public static void Subscribe<T>(Action<T> handler) where T : struct, IGameEvent
        {
            Type type = typeof(T);

            if (s_handlers.TryGetValue(type, out var existing))
            {
                s_handlers[type] = Delegate.Combine(existing, handler);
            }
            else
            {
                s_handlers[type] = handler;
            }
        }

        /// <summary>
        /// 이벤트 구독 해제
        /// </summary>
        public static void Unsubscribe<T>(Action<T> handler) where T : struct, IGameEvent
        {
            Type type = typeof(T);

            if (s_handlers.TryGetValue(type, out var existing))
            {
                var newHandler = Delegate.Remove(existing, handler);

                if (newHandler == null)
                {
                    s_handlers.Remove(type);
                }
                else
                {
                    s_handlers[type] = newHandler;
                }
            }
        }

        /// <summary>
        /// 이벤트 발행
        /// </summary>
        public static void Publish<T>(T evt) where T : struct, IGameEvent
        {
            Type type = typeof(T);

            if (s_handlers.TryGetValue(type, out var handler))
            {
                try
                {
                    ((Action<T>)handler)?.Invoke(evt);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[EventBus] 이벤트 처리 중 오류 발생: {e.Message}");
                }
            }
        }

        /// <summary>
        /// 모든 구독 해제 (씬 전환 시 호출)
        /// </summary>
        public static void Clear()
        {
            s_handlers.Clear();
        }

        /// <summary>
        /// 특정 이벤트 타입의 모든 구독 해제
        /// </summary>
        public static void Clear<T>() where T : struct, IGameEvent
        {
            s_handlers.Remove(typeof(T));
        }
    }

    /// <summary>
    /// 게임 이벤트 마커 인터페이스
    /// </summary>
    public interface IGameEvent { }

    #region Game Events

    /// <summary>
    /// 차량 스폰 이벤트
    /// </summary>
    public struct CarSpawnedEvent : IGameEvent
    {
        public Car.Car Car { get; }

        public CarSpawnedEvent(Car.Car car)
        {
            Car = car;
        }
    }

    /// <summary>
    /// 데미지 처리 이벤트
    /// </summary>
    public struct DamageDealtEvent : IGameEvent
    {
        public int Damage { get; }
        public UnityEngine.Vector2 Position { get; }
        public bool IsClickDamage { get; }

        public DamageDealtEvent(int damage, UnityEngine.Vector2 position = default, bool isClickDamage = true)
        {
            Damage = damage;
            Position = position;
            IsClickDamage = isClickDamage;
        }
    }

    /// <summary>
    /// 파츠 데미지 이벤트
    /// </summary>
    public struct PartDamagedEvent : IGameEvent
    {
        public CarPartType PartType { get; }
        public int CurrentHp { get; }

        public PartDamagedEvent(CarPartType partType, int currentHp)
        {
            PartType = partType;
            CurrentHp = currentHp;
        }
    }

    /// <summary>
    /// 파츠 파괴 이벤트
    /// </summary>
    public struct PartDestroyedEvent : IGameEvent
    {
        public CarPartType PartType { get; }

        public PartDestroyedEvent(CarPartType partType)
        {
            PartType = partType;
        }
    }

    /// <summary>
    /// 차량 파괴 이벤트
    /// </summary>
    public struct CarDestroyedEvent : IGameEvent
    {
        public int Reward { get; }

        public CarDestroyedEvent(int reward)
        {
            Reward = reward;
        }
    }

    /// <summary>
    /// 재화 변경 이벤트
    /// </summary>
    public struct CurrencyChangedEvent : IGameEvent
    {
        public ECurrencyType CurrencyType { get; }
        public Currency Amount { get; }

        public CurrencyChangedEvent(ECurrencyType currencyType, Currency amount)
        {
            CurrencyType = currencyType;
            Amount = amount;
        }
    }

    /// <summary>
    /// 파츠 수집 이벤트
    /// </summary>
    public struct PartCollectedEvent : IGameEvent
    {
        public PartType PartType { get; }
        public int Amount { get; }

        public PartCollectedEvent(PartType partType, int amount)
        {
            PartType = partType;
            Amount = amount;
        }
    }

    /// <summary>
    /// 업그레이드 완료 이벤트
    /// </summary>
    public struct UpgradeCompletedEvent : IGameEvent
    {
        public EUpgradeType UpgradeType { get; }
        public int NewLevel { get; }

        public UpgradeCompletedEvent(EUpgradeType upgradeType, int newLevel)
        {
            UpgradeType = upgradeType;
            NewLevel = newLevel;
        }
    }

    #endregion
}
