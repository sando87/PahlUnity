using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity
{
    public static class InGameUtils
    {
        public static BaseObject ExGetBase(this MonoBehaviour mono)
        {
            return mono.GetComponentInParent<BaseObject>();
        }
        public static T ExGetCompInBase<T>(this MonoBehaviour mono) where T : MonoBehaviour
        {
            BaseObject baseObj = mono.GetComponentInParent<BaseObject>();
            if (baseObj != null)
            {
                return baseObj.GetComponentInChildren<T>();
            }
            return null;
        }
        public static BaseObject ExGetBase(this Collider2D col)
        {
            return col.GetComponentInParent<BaseObject>();
        }
        public static T ExGetCompInBase<T>(this Collider2D col) where T : MonoBehaviour
        {
            BaseObject baseObj = col.GetComponentInParent<BaseObject>();
            if (baseObj != null)
            {
                return baseObj.GetComponentInChildren<T>();
            }
            return null;
        }
    }
}