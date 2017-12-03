namespace BasicTracer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using OpenTracing;

    internal sealed class BasicSpanContext : ISpanContext
    {
        public long TraceId { get; }
        public IDictionary<string, string> Baggage { get; }
        public long SpanId { get; }

        public BasicSpanContext(long traceId, long spanId, IDictionary<string, string> baggage)
        {
            this.Baggage = baggage;
            this.TraceId = traceId;
            this.SpanId = spanId;
        }

        public IEnumerable<KeyValuePair<string, string>> BaggageItems()
        {
            return this.Baggage;
        }

        // TODO: Should use some sort of watermark method to avoid having to copy the dictionary
        internal BasicSpanContext WithBaggageItem(string key, string val)
        {
            IDictionary<string, string> newBaggage = new Dictionary<string, string>(this.Baggage);
            newBaggage[key] = val;
            return new BasicSpanContext(this.TraceId, this.SpanId, newBaggage);
        }
    }
}