using System;
using System.Collections.Generic;
using UnityEngine;

namespace JunkyardClicker.Core
{
    // 서비스 로케이터 패턴 구현
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> s_services = new();

        // 서비스 등록
        public static void Register<T>(T service) where T : class
        {
            Type type = typeof(T);

            if (s_services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] 서비스 '{type.Name}'가 이미 등록되어 있습니다. 덮어씁니다.");
            }

            s_services[type] = service;
        }

        // 서비스 조회
        public static T Get<T>() where T : class
        {
            Type type = typeof(T);

            if (s_services.TryGetValue(type, out object service))
            {
                return service as T;
            }

            Debug.LogError($"[ServiceLocator] 서비스 '{type.Name}'를 찾을 수 없습니다.");
            return null;
        }

        // 서비스 조회 시도
        public static bool TryGet<T>(out T service) where T : class
        {
            Type type = typeof(T);

            if (s_services.TryGetValue(type, out object obj))
            {
                service = obj as T;
                return service != null;
            }

            service = null;
            return false;
        }

        // 서비스 등록 해제
        public static void Unregister<T>() where T : class
        {
            Type type = typeof(T);
            s_services.Remove(type);
        }

        // 모든 서비스 제거
        public static void Clear()
        {
            s_services.Clear();
        }

        // 서비스 등록 여부 확인
        public static bool IsRegistered<T>() where T : class
        {
            return s_services.ContainsKey(typeof(T));
        }
    }
}
