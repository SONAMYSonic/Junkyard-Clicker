using System;

namespace JunkyardClicker.Core
{
    /// <summary>
    /// 입력 검증을 위한 Guard 클래스
    /// 무결성 검사를 일관되게 수행
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// null 검사
        /// </summary>
        public static T NotNull<T>(T value, string paramName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
            return value;
        }

        /// <summary>
        /// 음수 검사 (0 이상이어야 함)
        /// </summary>
        public static int NotNegative(int value, string paramName)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, "값은 0 이상이어야 합니다.");
            }
            return value;
        }

        /// <summary>
        /// 양수 검사 (0보다 커야 함)
        /// </summary>
        public static int Positive(int value, string paramName)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, "값은 0보다 커야 합니다.");
            }
            return value;
        }

        /// <summary>
        /// 범위 검사
        /// </summary>
        public static int InRange(int value, int min, int max, string paramName)
        {
            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"값은 {min}과 {max} 사이여야 합니다.");
            }
            return value;
        }

        /// <summary>
        /// float 음수 검사
        /// </summary>
        public static float NotNegative(float value, string paramName)
        {
            if (value < 0f)
            {
                throw new ArgumentOutOfRangeException(paramName, value, "값은 0 이상이어야 합니다.");
            }
            return value;
        }

        /// <summary>
        /// float 양수 검사
        /// </summary>
        public static float Positive(float value, string paramName)
        {
            if (value <= 0f)
            {
                throw new ArgumentOutOfRangeException(paramName, value, "값은 0보다 커야 합니다.");
            }
            return value;
        }

        /// <summary>
        /// 빈 문자열 검사
        /// </summary>
        public static string NotNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("문자열이 비어있거나 null입니다.", paramName);
            }
            return value;
        }

        /// <summary>
        /// 배열 null 또는 빈 배열 검사
        /// </summary>
        public static T[] NotNullOrEmpty<T>(T[] array, string paramName)
        {
            if (array == null || array.Length == 0)
            {
                throw new ArgumentException("배열이 비어있거나 null입니다.", paramName);
            }
            return array;
        }
    }
}
