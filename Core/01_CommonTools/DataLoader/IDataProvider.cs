using Cysharp.Threading.Tasks;

/// <summary>
/// Data Loader Interface
/// </summary>
namespace PahlUnity
{
    public interface IDataProvider
    {
        UniTask<string> LoadAsync();

        UniTask SaveAsync(string data);
    }
}