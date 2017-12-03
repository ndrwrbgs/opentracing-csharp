namespace BasicTracer
{
    using System.Threading;
    using OpenTracing;

    internal sealed class BasicScope : IScope
    {
        private readonly BasicScopeManager manager;
        private int refCount;
        private readonly ISpan wrapped;
        private readonly bool finishSpanOnClose;
        private readonly IScope toRestore;

        public BasicScope(BasicScopeManager manager, int refCount, ISpan wrapped, bool finishSpanOnClose)
        {
            this.manager = manager;
            this.refCount = refCount;
            this.wrapped = wrapped;
            this.finishSpanOnClose = finishSpanOnClose;
            this.toRestore = manager.tlsScope.Value;
            manager.tlsScope.Value = this;
        }

        public void Dispose()
        {
            if (this.manager.tlsScope.Value != this)
            {
                return;
            }

            if (Interlocked.Decrement(ref this.refCount) == 0 && this.finishSpanOnClose)
            {
                this.wrapped.Finish();
            }

            this.manager.tlsScope.Value = this.toRestore;
        }

        public ISpan Span
            => this.wrapped;
    }
}