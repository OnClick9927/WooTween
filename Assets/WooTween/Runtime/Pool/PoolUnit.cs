using System;

namespace WooPool
{
    public abstract class PoolUnit : IDisposable
    {
        protected bool disposed { get; private set; }
        protected abstract void OnDispose();
        public void Dispose()
        {
            if (disposed) return;
            OnDispose();
            disposed = true;
        }
    }
}
