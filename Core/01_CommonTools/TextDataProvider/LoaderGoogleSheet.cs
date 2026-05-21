using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace PahlUnity
{
    public class LoaderGoogleSheet : ITextDataProvider
    {
        // ex) "1pe25syvJ-AiuEs4kVEwtqGZD7TwsUvlOXe8mqmPkXn8";
        private string mGoogleSheetUrlId;

        public LoaderGoogleSheet(string spreadsheetId)
        {
            mGoogleSheetUrlId = spreadsheetId;
        }

        // ex) key is sheetName like "playerSpec";
        public async UniTask<string> LoadAsync(string sheetName)
        {
            return await LoadGoogleSheetData(mGoogleSheetUrlId, sheetName);
        }

        private static async UniTask<string> LoadGoogleSheetData(string urlId, string sheetName)
        {
            string url = $"https://docs.google.com/spreadsheets/d/{urlId}/gviz/tq?tqx=out:csv&sheet={sheetName}";

            using UnityWebRequest www = UnityWebRequest.Get(url);

            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Failed to load google sheet: {www.error}");
            }

            return www.downloadHandler.text;
        }

        public UniTask SaveAsync(string key, string data)
        {
            throw new NotImplementedException();
        }

        public bool IsExist(string key)
        {
            throw new NotImplementedException();
        }
    }
}