using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using EditorGUITable;
using UnityEngine;

// How to use Sameple Code

// [System.Serializable]
// public class SampleInfo
// {
//     public int sampleID;
//     public string sampleName;
// }

// [CreateAssetMenu(fileName = "SampleTable", menuName = "Scriptable Object Asset/SampleTable")]
// public class SampleTable : ScriptableObjectTable<SampleInfo>
// {
// }

namespace PahlUnity
{
    public class ScriptableObjectTable<T> : ScriptableObject
    {
        [ReorderableTable]
        public List<T> DataList = new List<T>();
    }
}