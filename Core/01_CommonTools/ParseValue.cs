using System;
using System.Globalization;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;

namespace PahlUnity
{
    /// <summary>
    /// 스펙 테이블 데이터에서 범위 초기값 + (스텝 * 포인트) 형태의 데이터 포멧을 하나의 타입으로 다루기 편하게 하기 위함
    /// 입력 string 포멧 : "3~5@InOutQuad+0.2P+0.3L"
    /// </summary>
    public class ParseValue
    {
        static readonly char[] StepOperators = { '+', '-' };

        public float mBaseMin;
        public float mBaseMax;
        public Ease mEase = Ease.Linear;
        public float mStepByPoint;
        public float mStepByLevel;

        public ParseValue(float _min, float _max, float _stepByPoint, float _stepByLevel, Ease _ease)
        {
            mBaseMin = _min;
            mBaseMax = _max;
            mStepByPoint = _stepByPoint;
            mStepByLevel = _stepByLevel;
            mEase = _ease;
        }

        // 입력 string 예시: "3~5@InOutQuad+0.2P+0.3L"
        public static ParseValue Parse(string str)
        {
            if (String.IsNullOrEmpty(str))
                return default;

            float min = 0f, max = 0f, stepByPoint = 0f, stepByLevel = 0f;
            Ease ease = Ease.Linear;

            string work = str;

            // -------------------------------
            // 1) Step Per Point 파싱: "+0.2P" or "-0.2P"
            // -------------------------------
            int pointIdx = work.LastIndexOf('P');
            if (pointIdx > 0)
            {
                int stepIdx = work.LastIndexOfAny(StepOperators);
                string stepStr = work.Substring(stepIdx, pointIdx - stepIdx);
                stepByPoint = float.Parse(stepStr, CultureInfo.InvariantCulture);
                work = work.Substring(0, stepIdx); // 나머지 부분만 유지
            }

            // -------------------------------
            // 2) Step Per Level 파싱: "+0.2L" or "-0.2L"
            // -------------------------------
            int levelIdx = work.LastIndexOf('L');
            if (levelIdx > 0)
            {
                int stepIdx = work.LastIndexOfAny(StepOperators);
                string stepStr = work.Substring(stepIdx, levelIdx - stepIdx);
                stepByLevel = float.Parse(stepStr, CultureInfo.InvariantCulture);
                work = work.Substring(0, stepIdx); // 나머지 부분만 유지
            }

            // -------------------------------
            // 3) EASE 파싱: "@InOutQuad"
            // -------------------------------
            int atIdx = work.IndexOf('@');
            if (atIdx >= 0)
            {
                string easeStr = work.Substring(atIdx + 1);
                if (Enum.TryParse(easeStr, out Ease parsedEase))
                    ease = parsedEase;

                work = work.Substring(0, atIdx); // 범위 부분만 남기기
            }

            // -------------------------------
            // 4) RANGE / SINGLE VALUE 파싱
            // -------------------------------
            if (work.Contains("~"))
            {
                var parts = work.Split('~');
                if (parts.Length != 2)
                    throw new FormatException("잘못된 범위 형식입니다.");

                float from = float.Parse(parts[0], CultureInfo.InvariantCulture);
                float to = float.Parse(parts[1], CultureInfo.InvariantCulture);
                min = Math.Min(from, to);
                max = Math.Max(from, to);
            }
            else
            {
                // 범위가 없으면 단일값
                min = float.Parse(work, CultureInfo.InvariantCulture);
                max = min;
            }

            return new ParseValue(min, max, stepByPoint, stepByLevel, ease);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public float GetValue()
        {
            return mBaseMin;
        }
        public float GetValueByPoint(int points)
        {
            float result = mBaseMin + (mStepByPoint * points);
            return result;
        }
        public float GetValueByLevel(int level)
        {
            float result = mBaseMin + (mStepByLevel * level);
            return result;
        }
        public float GetValueByBoth(int point, int level)
        {
            float result = mBaseMin + (mStepByPoint * point) + (mStepByLevel * level);
            return result;
        }
        public float GetFloatInRange(float normalizedTime)
        {
            float t = TransferTime(normalizedTime);
            float val = Lerp(mBaseMin, mBaseMax, t);
            return val;
        }
        public int GetIntInRange(float normalizedTime)
        {
            float t = TransferTime(normalizedTime);
            float val = Lerp(mBaseMin, mBaseMax, t);
            return Mathf.RoundToInt(val);
        }
        private float Lerp(float a, float b, float t) => a + (b - a) * t;

        private float TransferTime(float time)
        {
            float trTime = 0;
            if (mEase != Ease.Unset)
            {
                trTime = EaseManager.Evaluate(mEase, null, (float)time, 1, 1, 1);
            }
            else
            {
                trTime = time;
            }

            // 0~1구간의 값들을 0.0 0.1 0.2 ... 0.9 1.0의 계단 형태로 값을 절삭시킨다.
            trTime = (int)(trTime / 0.091f) * 0.1f;
            return trTime;
        }
    }
}