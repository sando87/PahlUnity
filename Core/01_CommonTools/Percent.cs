using System;
using System.Globalization;
using UnityEngine;

namespace PahlUnity
{
    /// <summary>
    /// 일반퍼센트의 개념
    /// == 사용법 예시 ==
    /// Percent a = new Percent(100);
    /// float b = 10 * a; // b는 10
    /// Percent c = new Percent(50);
    /// float d = 10 * c; // d는 5
    [System.Serializable]
    public struct Percent : IComparable<Percent>
    {
        [SerializeField]
        private float mPercentVal; // [%]

        public float PercentValue => mPercentVal;

        public float Multiplier => mPercentVal * 0.01f;

        public static Percent Zero => new Percent(0);
        public static Percent One => new Percent(100);

        public Percent(float percent)
        {
            mPercentVal = percent;
        }
        public void SetZero()
        {
            mPercentVal = 0;
        }

        public static Percent Parse(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new FormatException("Input string is null or empty.");

            str = str.Trim();

            bool hasPercent = str.EndsWith("%");
            if (hasPercent)
                str = str.Substring(0, str.Length - 1).Trim();

            // 파싱 시도 (문화권 무시하고 '.'로 고정)
            if (!float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out float val))
                throw new FormatException($"Invalid percent format: {str}");

            return new Percent(val);
        }

        // ToString
        public override string ToString()
        {
            return $"{PercentValue}%";
        }

        // ---- 암시적 변환 ----
        public static explicit operator float(Percent p) => p.mPercentVal;
        public static explicit operator Percent(float v) => new Percent(v);
        // public static implicit operator Percent(float v) => new Percent(v);
        // public static implicit operator Percent(int v) => new Percent(v);

        // ---- Percent끼리 연산 ----
        public static Percent operator +(Percent a, Percent b) => new Percent(a.mPercentVal + b.mPercentVal);
        public static Percent operator -(Percent a, Percent b) => new Percent(a.mPercentVal - b.mPercentVal);
        // public static Percent operator *(Percent a, Percent b) => new Percent(a.mPercentVal * b.mPercentVal);
        // public static Percent operator /(Percent a, Percent b) => new Percent(a.mPercentVal / b.mPercentVal);

        // ---- 숫자와 Percent 연산 (양방향) ----
        public static float operator *(Percent a, float b) => a.Multiplier * b;
        public static float operator *(float a, Percent b) => a * b.Multiplier;

        // ---- 비교 연산 ----
        // public static bool operator ==(Percent a, Percent b) => a.mPercentVal == b.mPercentVal;
        // public static bool operator !=(Percent a, Percent b) => a.mPercentVal != b.mPercentVal;
        public static bool operator >(Percent a, Percent b) => a.mPercentVal > b.mPercentVal;
        public static bool operator <(Percent a, Percent b) => a.mPercentVal < b.mPercentVal;
        public static bool operator >=(Percent a, Percent b) => a.mPercentVal >= b.mPercentVal;
        public static bool operator <=(Percent a, Percent b) => a.mPercentVal <= b.mPercentVal;

        public override int GetHashCode() => mPercentVal.GetHashCode();

        public int CompareTo(Percent other) => mPercentVal.CompareTo(other.mPercentVal);
    }
}