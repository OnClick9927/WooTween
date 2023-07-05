using System;

namespace WooPool
{
    public interface IPoolObject : IDisposable
    {
        void OnAllocate();
        void OnGet();
        void OnSet();
    }
}
