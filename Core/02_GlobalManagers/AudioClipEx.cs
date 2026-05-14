using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PahlUnity
{
    public class AudioClipEx : ScriptableObject
    {
        public AudioClip[] clips = null; // 재생시킬 오디오 클립(다수면 랜덤재생)
        [Range(0, 1)]
        public float amplification = 1.0f; // 소리 크기 조절

        public string ID { get { return this.name; } }

#if UNITY_EDITOR
        [MenuItem("CONTEXT/AudioClip/Create AudioClipEx")]
        public static void CreateAudioClipEx(MenuCommand command)
        {
            // AudioClip clip = (AudioClip)command.context;
            // Debug.Log(clip.name);

            AudioClip[] clips = GetSelectedClips();
            CreateAudioClipEx(clips);
        }
        [MenuItem("Assets/Create/PahlUnity/Create AudioClipEx")]
        public static void CreateAudioClipEx()
        {
            AudioClip[] clips = GetSelectedClips();
            CreateAudioClipEx(clips);
        }

        public static void CreateAudioClipEx(AudioClip[] clips)
        {
            AudioClipEx asset = ScriptableObject.CreateInstance<AudioClipEx>();
            asset.clips = clips;

            string path = GetCurrentPath();
            string filename = GetAssetName();

            string assetPath = AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.Combine(path, filename));

            AssetDatabase.CreateAsset(asset, assetPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = asset;
        }

        static string GetAssetName()
        {
            if (Selection.activeObject != null)
            {
                AudioClip clip = Selection.activeObject as AudioClip;
                if (clip != null)
                {
                    string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                    string filename = System.IO.Path.GetFileNameWithoutExtension(path);
                    return filename + ".asset";
                }
            }

            return "NewAudioClipEx.asset";
        }

        static string GetCurrentPath()
        {
            string path = "Assets";

            if (Selection.activeObject != null)
            {
                path = AssetDatabase.GetAssetPath(Selection.activeObject);

                if (System.IO.File.Exists(path))
                {
                    path = System.IO.Path.GetDirectoryName(path);
                }
            }

            return path;
        }

        static AudioClip[] GetSelectedClips()
        {
            List<AudioClip> clips = new List<AudioClip>();
            foreach (Object obj in Selection.objects)
            {
                AudioClip clip = obj as AudioClip;
                if (clip != null)
                    clips.Add(clip);
            }
            return clips.ToArray();
        }
#endif
    }
}