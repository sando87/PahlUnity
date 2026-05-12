using System;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// PlayerPrefs IO
/// 
/// How to use
// PlayerPrefsIO prefsIO = new PlayerPrefsIO("PlayerSave");
// await prefsIO.SaveAsync("{\"gold\":100}");
// string json = await prefsIO.LoadAsync();
/// 
/// </summary>
namespace PahlUnity
{
    public class PlayerPrefsIO : IDataProvider
    {
        private string mKey;

        public PlayerPrefsIO(string key)
        {
            mKey = key;
        }

        // =========================================================
        // LOAD
        // =========================================================

        public async UniTask<string> LoadAsync()
        {
            await UniTask.CompletedTask;

            if (!PlayerPrefs.HasKey(mKey))
            {
                return string.Empty;
            }

            return PlayerPrefs.GetString(mKey);
        }

        // =========================================================
        // SAVE
        // =========================================================

        public async UniTask SaveAsync(string data)
        {
            await UniTask.CompletedTask;

            PlayerPrefs.SetString(
                mKey,
                data ?? string.Empty);

            PlayerPrefs.Save();
        }
    }
}