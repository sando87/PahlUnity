using System;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Local File IO
/// 
/// How to use
/// LocalFileIO fileIO = new LocalFileIO("Save/player.json");
/// await fileIO.SaveAsync("{ \"gold\": 100 }");
/// string json = await fileIO.LoadAsync();
/// 
/// </summary>
namespace PahlUnity
{
    public class LocalFileIO : IDataProvider
    {
        private string mFilePath;

        public LocalFileIO(string relativePath)
        {
            mFilePath = Path.Combine(Application.persistentDataPath, relativePath);
        }

        // =========================================================
        // LOAD
        // =========================================================

        public async UniTask<string> LoadAsync()
        {
            if (!File.Exists(mFilePath))
            {
                throw new FileNotFoundException($"File not found : {mFilePath}");
            }

            using FileStream fs =
                new FileStream(
                    mFilePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    4096,
                    true);

            using StreamReader reader = new StreamReader(fs, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

        // =========================================================
        // SAVE
        // =========================================================

        public async UniTask SaveAsync(string data)
        {
            string dir = Path.GetDirectoryName(mFilePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using FileStream fs =
                new FileStream(
                    mFilePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    4096,
                    true);

            using StreamWriter writer = new StreamWriter(fs, Encoding.UTF8);
            await writer.WriteAsync(data);
        }
    }
}