namespace BasicTracer
{
    using System;
    using System.Linq;
    using OpenTracing;
    using OpenTracing.Propagation;

    public sealed class BasicTracer : ITracer
    {
        public IScopeManager ScopeManager { get; } = new BasicScopeManager();
        
        public ISpanBuilder BuildSpan(string operationName)
        {
            return new BasicSpanBuilder(this, operationName);
        }
        
        public void Inject<T>(ISpanContext spanContext, IFormat<T> format, T carrier)
        {
            throw new NotImplementedException();
        }
        
        public ISpanContext Extract<T>(IFormat<T> format, T carrier)
        {
            throw new NotImplementedException();
        }

        internal void ReportFinishedSpan(BasicSpan basicSpan)
        {
            Console.WriteLine(basicSpan + $"\t{basicSpan.StartDateTimeOffset.Ticks} - {basicSpan.FinishDateTimeOffset.Ticks}");
            foreach (var basicSpanTag in basicSpan.Tags)
            {
                Console.WriteLine($"[{basicSpan.Id}]\t{basicSpanTag.Key} = {basicSpanTag.Value}");
            }
            foreach (var basicSpanLog in basicSpan.Logs)
            {
                Console.WriteLine($"[{basicSpan.Id}]\t({basicSpanLog.Item1})\t{string.Join(", ", basicSpanLog.Item2.Select(kvp => $"{kvp.Key} = {kvp.Value}"))}");
            }
            foreach (var keyValuePair in basicSpan.Context().BaggageItems())
            {
                Console.WriteLine($"[{basicSpan.Id}]\t{keyValuePair.Key} = {keyValuePair.Value}");
            }
        }
    }
}