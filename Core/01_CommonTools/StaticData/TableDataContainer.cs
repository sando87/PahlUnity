using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity
{
    // 클래스 개념
    // ITableRecord 인터페이스와 호환되는 데이터 관리, 전역적으로 데이터 접근 함수 제공
    //
    // 사용법 예시
    // [System.Serializable]
    // public class CharactorInfo : ITableRecord
    // {
    //     public int number;
    //     public string name;
    //     public int hp;

    //     public int RowIndex { get; set; }
    //     public long ID { get { return (long)number; } }
    // }
    //
    // 위와 같이 ID를 가지는 구조체 정의 후 아래와 같이 초기화 및 데이터 접근
    // TableDataContainer<CharactorInfo>.Instance.InitDataList(T[] dataList);
    // T info = TableDataContainer<CharactorInfo>.Instance.GetInfo(id);

    public class TableDataContainer<T> : Singleton<TableDataContainer<T>> where T : ITableRecord
    {
        private T[] mInfos = null;
        private Dictionary<long, T> mTable = new Dictionary<long, T>();

        public void InitDataList(T[] infos)
        {
            if (mTable.Count > 0)
                return;

            mInfos = infos;
            for (int i = 0; i < mInfos.Length; ++i)
            {
                T info = mInfos[i];
                info.RowIndex = i;
                info.OnLoad();
                long key = info.ID;
                mTable[key] = info;
            }
        }

        public bool HasInfo(long id)
        {
            return mTable.ContainsKey(id);
        }
        public T GetRandomItem()
        {
            int randIndex = UnityEngine.Random.Range(0, mInfos.Length);
            return mInfos[randIndex];
        }
        public T GetInfo(long id)
        {
            return mTable[id];
        }
        public T GetInfo(string stringID)
        {
            long id = ITableRecord.ToID(stringID);
            return mTable[id];
        }
        public T GetInfoOfIndex(int index)
        {
            return mInfos[index];
        }
        public T[] GetAllInfo()
        {
            return new List<T>(mTable.Values).ToArray();
        }
        public long[] GetAllIDs()
        {
            List<long> ids = new List<long>();
            foreach (T info in mInfos)
                ids.Add(info.ID);
            return ids.ToArray();
        }
        public IEnumerable<T> Enums()
        {
            foreach (var item in mTable)
                yield return item.Value;
        }
    }

    // 실제 구조체에서 아래 항목을 override해서 사용
    public interface ITableRecord
    {
        long ID { get { return -1; } } // 데이터 접근을 위한 id값
        int RowIndex { get; set; } // 전체 csv 테이블상에서 각 Row의 인덱스정보
        static long ToID(string stringID) { return stringID.GetHashCode(); } // 데이터 접근을 위한 id값
        void OnLoad() { } // 추가로 초기화 할 데이터 있으면 여기서 처리
    }
}