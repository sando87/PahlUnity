using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity
{
    public static class TemporaryList<T>
    {
        // 어떤 배열을 임시적으로 담기위한 전역 리스트
        // GC 할당을 최적화 하기 위한 장치
        // 템플릿 방식으로 어떤 형태의 리스트든지 편하게 대응 가능
        // 전역적으로 사용되는 리스트이기 때문에 반드시 임시적으로만 사용할것.
        // (멤버변수에 할당하거나 따로 다른 변수에 할당 금지.. 하나의 함수 시작,반환주기 안에서 정리할 것)
        // 주 사용처 : Physics관련 콜라이더 정보들 가져오는 함수들, Dictionary에서 Keys값들 리스트로 임시로 담을때 사용 등등
        private static readonly List<T> StaticTempList = new List<T>();

        public static List<T> GetTempList()
        {
            LOG.errorif(StaticTempList.Count > 0);
            return StaticTempList;
        }
    }
}