using System;

/// <summary>
/// 시작과 끝이 반드시 쌍으로 동작해야 하는 Scope형태의 동작시 사용
/// 예로 Lock/UnLock과 같은 기능을 using 블럭으로 감싸서 사용 가능
/// using (ScopeAction())
/// {
///     ...
/// }
/// using블럭을 나올때 항상 OnDispose로 등록해 놓았던 Action을 실행한다.
/// </summary>

namespace PahlUnity
{
    public sealed class ScopeAction : IDisposable
    {
        private Action _onDispose;
        private bool _disposed;

        public ScopeAction(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _onDispose?.Invoke();
            _onDispose = null;
        }
    }
}