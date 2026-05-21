using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

/// <summary>
/// Text Data Load Interface
/// </summary>
namespace PahlUnity
{
    public interface ITextDataProvider
    {
        bool IsExist(string key);

        UniTask<string> LoadAsync(string key);

        UniTask SaveAsync(string key, string data);
    }
}