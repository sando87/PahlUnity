using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전역 싱글톤으로 사용할 MonoBehaviour 가 있다면 이 클래스를 상속하여 사용
/// 선언예) public class SomethingManager : SingletonMono<SomethingManager> {}
/// 사용법) SomethingManager.Inst.DoSomething();
/// </summary>

namespace PahlUnity
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T mInstance;
        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    //Scene상에 없을경우 객채 생성 후 참조
                    mInstance = new GameObject().AddComponent<T>();
                    mInstance.name = typeof(T).Name;

                    DontDestroyOnLoad(mInstance.gameObject);
                }
                return mInstance;
            }
        }

        protected virtual void Awake()
        {
            T current = this as T;
            if (mInstance == null)
            {
                mInstance = current;
                DontDestroyOnLoad(gameObject);
            }
            else if (mInstance != current)
            {
                // 싱글톤 객체가 두번 생성되었으니 해제
                Destroy(gameObject);
            }
        }

    }
}