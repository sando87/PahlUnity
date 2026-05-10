using System;
using UnityEngine;

namespace PahlUnity
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MySerializableAttribute : PropertyAttribute
    {
        public bool hide = false;

        public MySerializableAttribute()
        {
            this.hide = false;
        }
        public MySerializableAttribute(bool hide)
        {
            this.hide = hide;
        }
    }
}