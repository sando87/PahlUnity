using Cysharp.Threading.Tasks;

/// <summary>
/// Text Data Load Interface
/// </summary>
namespace PahlUnity
{
    public interface ITextDataProvider
    {
        UniTask<string> LoadAsync(string key);

        UniTask SaveAsync(string key, string data);
    }
}