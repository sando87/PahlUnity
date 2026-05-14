using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    /// <summary>
    /// 릴리즈 빌드 후 인게임상태에서 빠른 진행 위한 테스트 기능들 구현됨.
    /// 사용법:
    /// 글로벌하게 호출가능, 각각의 클래스 범위에서 아래와 같이 등록,해제하면서 사용
    /// - CheatPanelOnRelease.Instance.Register("LevelUp", () => { player.Level++; });
    /// - CheatPanelOnRelease.Instance.Unregister("LevelUp");
    /// </summary>

    public class CheatPanelOnRelease : SingletonMono<CheatPanelOnRelease>
    {
        [SerializeField] bool _DisableAlways = false;

        private const float PressDuration = 3f;

        private bool mIsShow = false;
        private GUIStyle mGuiStyle = null;
        private List<(string, System.Action)> mActions = new List<(string, System.Action)>();

        public void Register(string btnName, System.Action action)
        {
            mActions.Add((btnName, action));
        }
        public void Unregister(string btnName)
        {
            mActions.RemoveAll(x => x.Item1 == btnName);
        }

        void OnEnable()
        {
            if (_DisableAlways)
                return;

            if (mGuiStyle == null)
            {
                mGuiStyle = new GUIStyle(GUI.skin.button);
                mGuiStyle.fontSize = 30;
            }

            StartCoroutine(OnOffPanel());
        }

        // 3초간 누르고 있으면 Test패널 킨다.
        IEnumerator OnOffPanel()
        {
            float pressingTime = 0;
            while (true)
            {
                yield return new WaitUntil(() => Keyboard.current.equalsKey.wasPressedThisFrame);

                pressingTime = 0;
                while (Keyboard.current.equalsKey.isPressed && pressingTime < PressDuration)
                {
                    pressingTime += Time.deltaTime;
                    yield return null;
                }

                mIsShow = pressingTime >= PressDuration;
                yield return null;
            }
        }

        void OnGUI()
        {
            if (!mIsShow) return;

            GUILayout.BeginVertical();

            foreach (var action in mActions)
            {
                if (GUILayout.Button(action.Item1, mGuiStyle))
                {
                    action.Item2.Invoke();
                }
            }

            GUILayout.EndVertical();
        }
    }
}