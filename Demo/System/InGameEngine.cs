using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PahlUnity.Demo
{
	public class InGameEngine : MonoBehaviour
	{
		[SerializeField] PlayerObject _PlayerPrefab = null;
		[SerializeField] Transform _PlayerSpawnPoint = null;
		[SerializeField] ItemObject _ItemPrefab = null;

		public PlayerObject Player { get; private set; }
		public ItemObject ItemPrefab => _ItemPrefab;

		public async UniTask StartGame()
		{
			await UniTask.Delay(1000);

			// Player = Instantiate(_PlayerPrefab, _PlayerSpawnPoint.position, _PlayerSpawnPoint.rotation, transform);

			await UniTask.Delay(1000);
		}
	}
}
