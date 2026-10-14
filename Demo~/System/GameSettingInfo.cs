using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PahlUnity.Demo
{
	public static class GameSettingInfo
	{
		private static UserSettingData mData = null;

		public static float BGMVolume { get => mData.BGMVolume; set { mData.BGMVolume = value; Save(); } }
		public static float SFXVolume { get => mData.SFXVolume; set { mData.SFXVolume = value; Save(); } }
		public static bool IsVSync { get => mData.IsVSync; set { mData.IsVSync = value; Save(); } }

		public static async UniTask LoadAsync()
		{
			IInitializer manager = SaveManager<UserSettingData>.Instance as IInitializer;
			string filename = typeof(UserSettingData).Name + ".json";
			string fullPath = Path.Combine(Application.persistentDataPath, filename);
			InitializingState state = await manager.InitializeAsync((new LocalFileIO(), fullPath), 10);
			LOG.trace(state);
			if (state == InitializingState.InitializedSuccess)
			{
				mData = SaveManager<UserSettingData>.Instance.SaveData;
			}
		}

		private static void Save()
		{
			SaveManager<UserSettingData>.Instance.SaveImmediate();
		}

		public static void ApplySettingsToSystem()
		{
			// AudioManager.Instance.SetBGMVolume(BGMVolume);
			// AudioManager.Instance.SetSFXVolume(SFXVolume);

			QualitySettings.vSyncCount = IsVSync ? 1 : 0;
		}
	}
}
