#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PahlUnity
{
    public class EditorUtil
    {
        [MenuItem("PahlUnity/Open PersistentDataPath")]
        public static void OpenPersistentDataPath()
        {
            string path = Application.persistentDataPath;
            System.Diagnostics.Process.Start(path);
        }
    }
}
#endif
