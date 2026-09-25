using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PahlUnity.Demo
{
	public class InGameManager : SingletonMono<InGameManager>
	{
		public InGameEngine Engine { get; private set; }

		public async UniTask StartGame()
		{
			ResetGame();
			await SceneSwitchManager.Instance.ChangeSceneAsync(SceneType.InGame);
			Engine = FindAnyObjectByType<InGameEngine>();
			await Engine.StartGame();
		}

		public void EndGame()
		{
			ResetGame();
			SceneSwitchManager.Instance.ChangeSceneAsync(SceneType.MainTitle).Forget();
		}

		public void PauseGame()
		{

		}

		public void ResumeGame()
		{

		}

		void ResetGame()
		{

		}

	}
}
