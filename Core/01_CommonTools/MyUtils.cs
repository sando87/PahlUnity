using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;

#endif

// Util Class ================================================
namespace PahlUnity
{
    public class MyUtils
    {
        // System =====================================
        public static int EnumCount<T>() where T : Enum
        {
            return System.Enum.GetValues(typeof(T)).Length;
        }
        public static IEnumerable EnumForeach<T>() where T : Enum
        {
            foreach (T item in System.Enum.GetValues(typeof(T)))
                yield return item;
        }
        public static T EnumRandomPick<T>(bool exceptFirst = false, bool exceptLast = false) where T : Enum
        {
            var enumValues = System.Enum.GetValues(typeof(T));
            int firstIdx = exceptFirst ? 1 : 0;
            int count = exceptLast ? enumValues.Length - 1 : enumValues.Length;
            int curIdx = UnityEngine.Random.Range(firstIdx, count);
            return (T)enumValues.GetValue(curIdx);
        }
        public static bool IsPercentHit(int percent)
        {
            return (UnityEngine.Random.Range(0, 1000) % 100) < percent;
        }
        public static string[] GetFileNames(string folderName, string ext) // ext like this... ".png" ".json" ".asset" etc...
        {
            List<string> filenames = new List<string>();
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(folderName);
            foreach (System.IO.FileInfo f in di.GetFiles())
            {
                if (f.Extension.ToLower().CompareTo(ext.ToLower()) == 0)
                {
                    // String strInFileName = di.FullName + "\\" + f.Name;
                    string name = f.Name.Replace(f.Extension, "");
                    filenames.Add(name);
                }
            }
            return filenames.ToArray();
        }

        // world좌표계 너비를 실제 장비 기준 해상도 좌표계 너비로 변환
        public static Vector2 WorldSizeToScreenSize(Vector2 size, Camera mainCamera)
        {
            // 실제 장비 해상도 기준의 스크린 좌표값
            Vector2 screenA = mainCamera.WorldToScreenPoint(new Vector2(0, 0));
            Vector2 screenB = mainCamera.WorldToScreenPoint(new Vector2(size.x, size.y));
            return screenB - screenA;
        }
        // 스크린 크기를 월드공간의 크기로 변환
        public static Vector2 ScreenSizeToWorldSize(Vector2 size, Camera mainCamera)
        {
            Vector3 startPos = mainCamera.ScreenToWorldPoint(Vector3.zero);
            Vector3 endPos = mainCamera.ScreenToWorldPoint(new Vector3(size.x, size.y, 0));
            return new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);
        }
        // 실제 장비 해상도를 inspector에 들어가는 값기준으로 변경(캔버스 스캐일링 된..)
        public static Vector2 ScreenSizeToCavausScaledSize(Vector2 size, CanvasScaler scaler)
        {
            // 유니티상에 설정된 레퍼런스 해상도 기준의 좌표값으로 변환 작업
            float wRatio = scaler.referenceResolution.x / Screen.width;
            float hRatio = scaler.referenceResolution.y / Screen.height;
            float ratio = wRatio * (1f - scaler.matchWidthOrHeight)
                        + hRatio * (scaler.matchWidthOrHeight);
            // 현재 UI RectTransform의 Inspector상에 들어가는 width 최종값
            return size * ratio;
        }


        public static int RandomInt(int minInclusive, int maxExclusive)
        {
            return UnityEngine.Random.Range(minInclusive, maxExclusive);
        }
        public static float RandomFloat(float minInclusive, float maxInclusive)
        {
            return UnityEngine.Random.Range(minInclusive, maxInclusive);
        }
        public static float RandomRate()
        {
            return UnityEngine.Random.value;
        }
        public static Vector3 Random(Vector3 pos, float range)
        {
            Vector3 ret = pos;
            ret.x += UnityEngine.Random.Range(-range, range);
            ret.y += UnityEngine.Random.Range(-range, range);
            return ret;
        }
        public static Vector3 Random(Rect rect)
        {
            Vector3 ret = rect.center;
            Vector2 halfSize = rect.size * 0.5f;
            ret.x += UnityEngine.Random.Range(-halfSize.x, halfSize.x);
            ret.y += UnityEngine.Random.Range(-halfSize.y, halfSize.y);
            return ret;
        }
        public static Vector3 Random(Bounds bounds, float scaleRate = 1.0f)
        {
            Vector3 ret = bounds.center;
            ret.x += UnityEngine.Random.Range(-bounds.extents.x, bounds.extents.x) * scaleRate;
            ret.y += UnityEngine.Random.Range(-bounds.extents.y, bounds.extents.y) * scaleRate;
            ret.z += UnityEngine.Random.Range(-bounds.extents.z, bounds.extents.z) * scaleRate;
            return ret;
        }

        public static void DisableAllChilds(GameObject parent)
        {
            int cnt = parent.transform.childCount;
            for (int i = 0; i < cnt; ++i)
            {
                parent.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        public static T FindUIOnPosition<T>(Vector2 worldPos) where T : MonoBehaviour
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = worldPos;
            List<RaycastResult> rets = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, rets);
            foreach (RaycastResult ret in rets)
            {
                T recv = ret.gameObject.GetComponent<T>();
                if (recv != null)
                    return recv;
            }
            return default(T);
        }
        public static T FindObjectOnPosition<T>(Vector2 worldPos) where T : MonoBehaviour
        {
            //Vector2 worldPos = Camera.main.ScreenToWorldPoint(transform.position);
            Collider2D[] mapObjects = Physics2D.OverlapPointAll(worldPos);
            foreach (Collider2D col in mapObjects)
            {
                T recv = col.GetComponent<T>();
                if (recv != null)
                {
                    return recv;
                }
            }
            return default(T);
        }
        public static bool IsPointerOverUI()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return EventSystem.current.IsPointerOverGameObject(-1); //mouse click
#elif UNITY_ANDROID || UNITY_IPHONE
        return Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId); //finger touch 
#else
        return false;
#endif
        }
        public static Vector2 GetPointerScreenPosition()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return Input.mousePosition;
#elif UNITY_ANDROID || UNITY_IPHONE
        return Input.touchCount > 0 ? Input.GetTouch(0).position : Vector2.zero;
#else
        return Vector2.zero;
#endif
        }

        public static bool IsSubType<Tsub, Tbase>()
        {
            return typeof(Tsub).IsSubclassOf(typeof(Tbase));
        }
        public static bool IsSubType(Type subTyp, Type baseType)
        {
            return subTyp.IsSubclassOf(baseType);
        }



        public static string SecondsToMinSec(int playTime)
        {
            TimeSpan t = new TimeSpan(0, 0, playTime);
            return String.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }
        public static string ToCommaEveryThree(int amount)
        {
            // 123456789 -> 123,456,789 로 표기
            return String.Format("{0:#,0}", amount);
        }
        public static string CurrentStyleToInt(string money)
        {
            // 123,456,789 -> 123456789 로 표기
            return money.Replace(",", "");
        }

        public static bool IsIncludeLayer(int layer, LayerMask layermask)
        {
            return layermask == (layermask | (1 << layer));
        }

        public static Vector3 SnapToClose(Vector3 pos, float step)
        {
            float x = (int)((pos.x + (step * 0.5f)) / step) * step;
            float y = (int)((pos.y + (step * 0.5f)) / step) * step;
            // float z = (int)((tr.position.z + (step * 0.5f)) / step) * step;
            return new Vector3(x, y, pos.z);
        }
        public static Vector3Int SnapIndexToClose(Vector3 worldPos, float step)
        {
            int x = (int)((worldPos.x + (step * 0.5f)) / step);
            int y = (int)((worldPos.y + (step * 0.5f)) / step);
            int z = (int)((worldPos.z + (step * 0.5f)) / step);
            return new Vector3Int(x, y, z);
        }

        public static bool RaycastFromTo(Vector3 start, Vector3 end, out RaycastHit hit, int layerMask)
        {
            Vector3 dir = end - start;
            Ray ray = new Ray(start, dir);
            return Physics.Raycast(ray, out hit, dir.magnitude, layerMask);
        }
        public static bool RaycastFromTo(Vector3 start, Vector3 end, int layerMask)
        {
            Vector3 dir = end - start;
            Ray ray = new Ray(start, dir);
            return Physics.Raycast(ray, dir.magnitude, layerMask);
        }

        public static bool IsCooltimeOver(float prevTime, float cooltime)
        {
            double interval = Time.time - prevTime;
            return interval > cooltime;
        }

        public static int Min(params int[] values)
        {
            int retVal = int.MaxValue;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] < retVal)
                    retVal = values[i];
            }
            return retVal;
        }
        public static int Max(params int[] values)
        {
            int retVal = int.MinValue;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > retVal)
                    retVal = values[i];
            }
            return retVal;
        }
        public static float Min(params float[] values)
        {
            float retVal = float.MaxValue;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] < retVal)
                    retVal = values[i];
            }
            return retVal;
        }
        public static float Max(params float[] values)
        {
            float retVal = float.MinValue;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > retVal)
                    retVal = values[i];
            }
            return retVal;
        }

        // 특정 타겟을 바라보는 방향으로 천천히 방향 전환, 유도탄 같은 궤적에서 사용(rotate 회전 lerp toward) - 3D용
        // Quaternion기반, 순수 Vector3 회전 기반, 2D기반(x,y축사용)
        public static IEnumerator CoRotateTowardsLerp(Transform me, Transform target, float rotateSpeed)
        {
            Vector3 lastTargetPos = Vector3.zero;
            while (true)
            {
                Vector3 curTargetPos = (target == null) ? lastTargetPos : target.position;
                Vector3 targetDirection = curTargetPos - me.transform.position;
                lastTargetPos = curTargetPos;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                me.transform.rotation = Quaternion.RotateTowards(me.transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
                yield return null;
            }
        }
        public static IEnumerator CoRotateTowardsVectorLerp(Transform me, Transform target, float rotateSpeed)
        {
            Vector3 lastTargetPos = Vector3.zero;
            while (true)
            {
                Vector3 curTargetPos = (target == null) ? lastTargetPos : target.position;
                Vector3 targetDirection = curTargetPos - me.transform.position;
                lastTargetPos = curTargetPos;
                float singleStep = rotateSpeed * Time.deltaTime;
                Vector3 newDirection = Vector3.RotateTowards(me.transform.forward, targetDirection, singleStep, 0.0f);
                Debug.DrawRay(me.transform.position, newDirection, Color.red);
                me.transform.rotation = Quaternion.LookRotation(newDirection);
                yield return null;
            }
        }
        public static Vector3 Reflect(Vector3 enterVector, Vector3 normal)
        {
            return Vector3.Reflect(enterVector, normal);
        }
        public static IEnumerator CoRotateTowards2DLerp(Transform me, Transform target, float rotateRadianPerSec)
        {
            Vector3 lastTargetPos = Vector3.zero;
            while (true)
            {
                Vector3 curTargetPos = (target == null) ? lastTargetPos : target.position;
                Vector3 targetDirection = curTargetPos - me.transform.position;
                lastTargetPos = curTargetPos;
                float singleStep = rotateRadianPerSec * Time.deltaTime;
                Vector3 newDirection = Vector3.RotateTowards(me.transform.right, targetDirection, singleStep, 0.0f);

                Debug.DrawRay(me.transform.position, newDirection, Color.red);

                float degree = Vector3.SignedAngle(newDirection, Vector3.right, Vector3.back);
                me.transform.rotation = Quaternion.Euler(0, 0, degree);
                yield return null;
            }
        }

        // 소수점 표기방식이 국가별로 다르기 때문에 그것에 대응하기 위한 옵션을 적용하면서 float를 파싱해야 한다
        public static float ParseFloat(string val)
        {
            if (float.TryParse(val, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float valFloat))
            {
                return valFloat;
            }
            else
            {
                Debug.LogWarning("Error ParseFloat");
            }
            return 0;
        }

        public static string[] FindAllFiles(string rootdir)
        {
            // 루트 디렉터리와 모든 하위 디렉터리에 있는 파일 목록을 가져옵니다.
            string[] files = Directory.GetFiles(rootdir, "*", SearchOption.AllDirectories);

            // // 디렉토리 및 하위 디렉토리 목록을 가져옵니다.
            // string[] dirs = Directory.GetDirectories(rootdir, "*", SearchOption.AllDirectories);
            // Console.WriteLine(String.Join(Environment.NewLine, dirs));

            return files;
        }

        public static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        public static void OpenPersistentDataPath()
        {
            string path = Application.persistentDataPath;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN   // Windows
            // 경로가 공백 포함해도 열리도록 쌍따옴표 추가
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true, // 매우 중요: explorer로 경로를 정상 처리하게 함
                Verb = "open" // Windows에서는 무시될 수도 있지만 넣어둠
            };
            System.Diagnostics.Process.Start(psi);

#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX // macOS
        // macOS Finder에서 폴더 열기
        System.Diagnostics.Process.Start("open", path);

#else
        Debug.LogWarning("지원되지 않는 플랫폼입니다.");
#endif
        }

        #region UNITY Editor Area
#if UNITY_EDITOR

        //SerializedProperty 에서 실제 객체 TYPE의 오브젝트로 반환
        public static object GetValueFromProp(SerializedProperty property, string path)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetField(path);
            return fi.GetValue(property.serializedObject.targetObject);
        }
        //SerializedProperty 에서 실제 객체 TYPE의 오브젝트값을 세팅
        public static void SetValueToProp(SerializedProperty property, object value)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);//this FieldInfo contains the type.
            fi.SetValue(property.serializedObject.targetObject, value);
        }
        ////SerializedProperty 에서 실제 객체의 Type을 반환
        public static System.Type GetTypeFromProp(SerializedProperty property)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);
            return fi.FieldType;
        }
        // guid값에서 실제 Asset 리소스 객체 가져옴
        public static UnityEngine.Object GetAssetFromGUID(string guid, Type assetType)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath(path, assetType);
        }

        // prefab으로부터 guid정보를 가져온다
        public static string GetGUIDFromPrefab(UnityEngine.GameObject prefab)
        {
            UnityEngine.Object oriPrefab = GetOriginPrefab(prefab);
            if (oriPrefab == null)
                return "";

            string path = AssetDatabase.GetAssetPath(oriPrefab);
            string guid = AssetDatabase.AssetPathToGUID(path);
            return guid;
        }

        // 실제 ProjectView에 존재하는 원본 prefab파일에 접근한다
        public static UnityEngine.Object GetOriginPrefab(UnityEngine.Object selection)
        {
            if (selection == null)
                return null;

            // 참고사항...
            // LOG.trace(PrefabUtility.IsPartOfRegularPrefab(gameObject)); // 어떤 프리팹 하위에 존재하는 일반 객체(프리팹Stage상태에서 확인됨)
            // LOG.trace(PrefabUtility.IsAnyPrefabInstanceRoot(gameObject)); // 다른 프리팹을 끌어다가 놓은 복사본인 경우(프리팹Stage상태에서 확인됨)

            // IsPersistent는 디스크에 파일 형태로 존재하는지 여부 파악(오리지널 프리팹을 확인하는 방법)
            if (EditorUtility.IsPersistent(selection))
                return selection;

            PrefabAssetType type = PrefabUtility.GetPrefabAssetType(selection);
            if (type == PrefabAssetType.Regular)
            {
                // 대응되는 원본 프리팹을 참조한다
                UnityEngine.Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(selection);
                return prefab;
            }

            PrefabStage prefabStage = PrefabStageUtility.GetPrefabStage(selection as GameObject);
            if (prefabStage != null)
            {
                return AssetDatabase.LoadAssetAtPath<GameObject>(prefabStage.assetPath);
            }

            return null;

            // There are totally 3 distinct type/state of prefab. And they must be handled differently.
            // Prefab Asset (The one that sits in Asset folder)
            // Prefab Instance (The one that dragged into scene to become blue game object, or nested inside another prefab)
            // Prefab Stage (Editing prefab intermediate in UnityEditor prefab edit mode)

            // For example: from getting assetPath alone, these 3 cases will need different API
            // Prefab Asset => use UnityEditor.AssetDatabase.GetAssetPath()
            // Prefab Instance => PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot()
            // Prefab Stage => PrefabStageUtility.GetCurrentPrefabStage then editorPrefabStage.prefabAssetPath

            // Finding root of game object for each case also need different API
            // Prefab Asset => use gameObject.transform.root.gameObject()
            // Prefab Instance => PrefabUtility.GetNearestPrefabInstanceRoot()
            // Prefab Stage => PrefabStageUtility.GetCurrentPrefabStage then editorPrefabStage.prefabContentsRoot
            // (See deprecate message in old PrefabUtility.FindPrefabRoot function, it is very useful.)

            // Im not sure which case you are on.
            // But for PrefabUitility API example, you could look into my demo on https://github.com/wappenull/UnityPrefabTester
            // It is editor script demo.
        }
        // guid값에서 실제 Asset 리소스 객체 가져옴
        public static string GetGUIDFromAsset(UnityEngine.Object asset)
        {
            // asset이 prefabView 모드나 scene에 instantiate된 객체일 경우 assetPath를 반환하지 않는다.
            // asset이 리소스일 경우만 동작
            string path = AssetDatabase.GetAssetPath(asset);
            string guid = AssetDatabase.AssetPathToGUID(path);
            return guid;
        }
        public static long GUIDToLong(string guid)
        {
            return StableHash.ToInt64(guid);
        }


        public static string[] GetAnimatorStateNames(Animator animator)
        {
            List<string> names = new List<string>();
            AnimatorController ac = animator.runtimeAnimatorController as AnimatorController;
            if (ac == null)
            {
                AnimatorOverrideController aoc = animator.runtimeAnimatorController as AnimatorOverrideController;
                ac = aoc.runtimeAnimatorController as AnimatorController;
            }

            foreach (AnimatorControllerLayer layer in ac.layers)
            {
                ChildAnimatorState[] states = layer.stateMachine.states;
                foreach (ChildAnimatorState s in states)
                {
                    names.Add(layer.name + "." + s.state.name);
                }
            }
            return names.ToArray();
        }
        static public void CreateScriptableObjectFileAsset<T>(string path) where T : ScriptableObject
        {
            string filename = typeof(T).Name;

            ScriptableObject asset = ScriptableObject.CreateInstance<T>();

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + filename + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            // EditorUtility.FocusProjectWindow();
            // Selection.activeObject = asset;
        }
        static public void SaveScriptableObjectFileAsset(ScriptableObject so)
        {
            UnityEditor.EditorUtility.SetDirty(so);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            // EditorUtility.FocusProjectWindow();
            // Selection.activeObject = asset;
        }
        public static void AddParameterAnimatorController(AnimatorController ac, string name, UnityEngine.AnimatorControllerParameterType type, object defaultValue)
        {
            AnimatorControllerParameter pp = new AnimatorControllerParameter();
            pp.name = ac.MakeUniqueParameterName(name);
            pp.type = type;
            switch (type)
            {
                case AnimatorControllerParameterType.Float: pp.defaultFloat = (float)defaultValue; break;
                case AnimatorControllerParameterType.Int: pp.defaultInt = (int)defaultValue; break;
                case AnimatorControllerParameterType.Bool: pp.defaultBool = (bool)defaultValue; break;
                case AnimatorControllerParameterType.Trigger: break;
            }

            ac.AddParameter(pp);
        }

#endif
        #endregion

    }
}
