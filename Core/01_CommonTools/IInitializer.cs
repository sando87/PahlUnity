
using Cysharp.Threading.Tasks;

namespace PahlUnity
{
    public interface IInitializer
    {
        InitializingState InitState { get { return InitializingState.NotUse; } }
        float Progress { get { return -1; } }

        InitializingState Initialize(object param) { return InitializingState.NotUse; }
        UniTask<InitializingState> InitializeAsync(object param, float timeout) { return UniTask.FromResult(InitializingState.NotUse); }
    }

    public enum InitializingState
    {
        NotUse,
        NotInitialized,
        Initializing,
        InitializedSuccess,
        InitializedFailed,
    }
}