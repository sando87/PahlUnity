using System;

public class LoaderGoogleSheet : ILoaderInterface
{
    // ex) "1pe25syvJ-AiuEs4kVEwtqGZD7TwsUvlOXe8mqmPkXn8";
    private string mGoogleSheetUrlId;
    // ex) "playerSpec";
    private string mSheetName;

    public LoaderGoogleSheet(string spreadsheetId, string sheetName)
    {
        mGoogleSheetUrlId = spreadsheetId;
        mSheetName = sheetName;
    }

    public string Load()
    {
        return LoadGoogleSheetData(mGoogleSheetUrlId, mSheetName);
    }

    public static string LoadGoogleSheetData(string urlId, string sheetName)
    {
        string url = $"https://docs.google.com/spreadsheets/d/{urlId}/gviz/tq?tqx=out:csv&sheet={sheetName}";

        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(url);
        var operation = www.SendWebRequest();
        while (!operation.isDone)
            System.Threading.Thread.Sleep(100);

        if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            throw new Exception($"Failed to load google sheet: {www.error}");
        }

        string rawDataCsvFormat = www.downloadHandler.text;
        return rawDataCsvFormat;
    }
}
