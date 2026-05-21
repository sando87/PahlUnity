using System;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Local File IO
/// 
/// How to use
/// string fullPath = "Save/player.json";
/// LocalFileIO fileIO = new LocalFileIO();
/// await fileIO.SaveAsync(fullPath, "{ \"gold\": 100 }");
/// string json = await fileIO.LoadAsync(fullPath);
/// 
/// </summary>
namespace PahlUnity
{
    public class LocalFileIO : ITextDataProvider
    {
        public bool IsExist(string fullPath)
        {
            return File.Exists(fullPath);
        }

        // =========================================================
        // LOAD
        // =========================================================

        public async UniTask<string> LoadAsync(string fullPath)
        {
            if (!IsExist(fullPath))
            {
                throw new FileNotFoundException($"File not found : {fullPath}");
            }

            using FileStream fs =
                new FileStream(
                    fullPath,
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

        public async UniTask SaveAsync(string fullPath, string data)
        {
            string dir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using FileStream fs =
                new FileStream(
                    fullPath,
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