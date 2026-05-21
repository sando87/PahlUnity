using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity.Demo
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
            yield return SceneSwtichManager.Instance.ChangeSceneAsync(destScene).ToCoroutine();
            Engine = FindAnyObjectByType<InGameEngine>();
        }
    }
}
