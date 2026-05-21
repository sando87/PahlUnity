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
    public class PlayerPrefsIO : ITextDataProvider
    {
        public bool IsExist(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        // =========================================================
        // LOAD
        // =========================================================

        public async UniTask<string> LoadAsync(string key)
        {
            await UniTask.CompletedTask;

            if (!PlayerPrefs.HasKey(key))
            {
                return string.Empty;
            }

            return PlayerPrefs.GetString(key);
        }

        // =========================================================
        // SAVE
        // =========================================================

        public async UniTask SaveAsync(string key, string data)
        {
            await UniTask.CompletedTask;

            PlayerPrefs.SetString(
                key,
                data ?? string.Empty);

            PlayerPrefs.Save();
        }
    }
}