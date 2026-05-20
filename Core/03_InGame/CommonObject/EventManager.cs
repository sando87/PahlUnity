using System;
using System.Collections.Generic;
using UnityEngine;


namespace PahlUnity
{
    public interface IEventParam { }

    public class EventManager : MonoBehaviour
    {
        private readonly Dictionary<Type, Delegate> mEventMap = new Dictionary<Type, Delegate>();

        public void Register<T>(Action<T> action) where T : struct, IEventParam
        {
            Type type = typeof(T);

            if (mEventMap.TryGetValue(type, out Delegate callback))
            {
                mEventMap[type] = Delegate.Combine(callback, action);
            }
            else
            {
                mEventMap.Add(type, action);
            }
        }

        public void UnRegister<T>(Action<T> action) where T : struct, IEventParam
        {
            Type type = typeof(T);

            if (mEventMap.TryGetValue(type, out Delegate callback) == false)
            {
                return;
            }

            Delegate removed = Delegate.Remove(callback, action);

            if (removed == null)
            {
                mEventMap.Remove(type);
            }
            else
            {
                mEventMap[type] = removed;
            }
        }

        public void InvokeEvent<T>(T param) where T : struct, IEventParam
        {
            Type type = typeof(T);

            if (mEventMap.TryGetValue(type, out Delegate callback))
            {
                ((Action<T>)callback)?.Invoke(param);
            }
        }

        public void Clear()
        {
            mEventMap.Clear();
        }
    }
}
