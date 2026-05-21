using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class InGameManager : SingletonMono<InGameManager>
    {
        public InGameEngine Engine { get; private set; } = null;
        public int DestWarpID { get; private set; } = 0;

        protected override void Awake()
        {
            base.Awake();
            Engine = FindAnyObjectByType<InGameEngine>();
        }

        public void StartScene(string destScene, int destWarpID)
        {
            StartCoroutine(CoStartScene(destScene, destWarpID));
        }
        IEnumerator CoStartScene(string destScene, int destWarpID)
        {
            DestWarpID = destWarpID;
            // SceneSwtichManager.Instance.LoadSceneImmediately((int)destScene);
            yield return new WaitUntil(() => SceneSwtichManager.Instance.IsLoaded);
            Engine = FindAnyObjectByType<InGameEngine>();
        }
    }
}
