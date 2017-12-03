namespace BasicTracer
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using OpenTracing;

    internal sealed class BasicSpan : ISpan
    {
        private static long nextId;

        private readonly BasicTracer tracer;
        private BasicSpanContext context;
        internal long ParentId { get; }
        internal long Id => this.context.SpanId;
        internal DateTimeOffset StartDateTimeOffset { get; }
        private int finished;
        internal DateTimeOffset FinishDateTimeOffset { get; private set; }
        internal IDictionary<string, object> Tags { get; }
        internal IList<Tuple<DateTimeOffset, IDictionary<string, object>>> Logs { get; }
            = new List<Tuple<DateTimeOffset, IDictionary<string, object>>>();
        internal string OperationName { get; private set; }

        public BasicSpan(BasicTracer tracer, string operationName, DateTimeOffset startDateTimeOffset, IDictionary<string, object> initialTags, BasicSpanContext parent)
        {
            this.tracer = tracer;
            this.OperationName = operationName;
            this.StartDateTimeOffset = startDateTimeOffset;
            this.Tags = initialTags ?? new Dictionary<string, object>();
            if (parent == null)
            {
                // We're a root Span.
                this.context = new BasicSpanContext(
                    Interlocked.Increment(ref nextId),
                    Interlocked.Increment(ref nextId),
                    new Dictionary<string, string>());
                this.ParentId = 0;
            }
            else
            {
                // We're a child Span.
                this.context = new BasicSpanContext(
                    parent.TraceId,
                    Interlocked.Increment(ref nextId),
                    parent.Baggage);
                this.ParentId = parent.SpanId;
            }
        }

        public ISpanContext Context()
        {
            return this.context;
        }

        public ISpan SetTag(string key, string value)
        {
            this.Tags[key] = value;
            return this;
        }

        public ISpan SetTag(string key, bool value)
        {
            this.Tags[key] = value;
            return this;
        }

        public ISpan SetTag(string key, int value)
        {
            this.Tags[key] = value;
            return this;
        }

        public ISpan SetTag(string key, double value)
        {
            this.Tags[key] = value;
            return this;
        }

        public ISpan Log(IDictionary<string, object> fields)
        {
            return Log(DateTimeOffset.Now, fields);
        }

        public ISpan Log(DateTimeOffset timestamp, IDictionary<string, object> fields)
        {
            if (this.finished == 1)
            {
                throw new InvalidOperationException();
            }
            this.Logs.Add(Tuple.Create(timestamp, fields));
            return this;
        }

        public ISpan Log(string @event)
        {
            return Log(DateTimeOffset.Now, @event);
        }

        public ISpan Log(DateTimeOffset timestamp, string @event)
        {
            return Log(
                timestamp,
                new Dictionary<string, object>
                {
                    ["event"] = @event
                });
        }
        
        public ISpan SetBaggageItem(string key, string value)
        {
            if (this.finished == 1)
            {
                throw new InvalidOperationException("Adding baggage to already finished span");
            }
            this.context = this.context.WithBaggageItem(key, value);
            return this;
        }

        public string GetBaggaggeItem(string key)
        {
            return this.context.Baggage[key];
        }

        public ISpan SetOperationName(string operationName)
        {
            this.OperationName = operationName;
            return this;
        }

        public void Finish()
        {
            this.Finish(DateTimeOffset.Now);
        }

        public void Finish(DateTimeOffset finishTime)
        {
            if (Interlocked.Exchange(ref this.finished, 1) == 1)
            {
                throw new InvalidOperationException("Finishing already finished span");
            }
            this.FinishDateTimeOffset = finishTime;
            this.tracer.ReportFinishedSpan(this);
        }

        public override string ToString()
        {
            return
                $"{{"
                + $"traceId:{this.context.TraceId}"
                + $", spanId:{this.context.SpanId}"
                + $", parentId:{this.ParentId}"
                + $", operationName:\"{this.OperationName}\"}}";
        }
    }
}