using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PahlUnity.Demo
{
	public class InGameEngine : MonoBehaviour
	{
		[SerializeField] Transform _PlayerSpawnPoint = null;

		public PlayerObject Player { get; private set; }

		public async UniTask StartGame()
		{
			await UniTask.Delay(1000);

			GameObject playerPrefab = ResourceManager.Instance.GetPrefab("Player");
			GameObject player = Instantiate(playerPrefab, _PlayerSpawnPoint.position, _PlayerSpawnPoint.rotation, transform);
			Player = player.GetComponentInChildren<PlayerObject>();

			await UniTask.Delay(1000);
		}
	}
}
