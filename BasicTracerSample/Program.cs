// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractTag.cs">
//   Copyright 2017-2018 The OpenTracing Authors
//   
//   Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except
//   in compliance with the License. You may obtain a copy of the License at
//   
//   http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software distributed under the License
//   is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
//   or implied. See the License for the specific language governing permissions and limitations under
//   the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BasicTracerSample
{
    using BasicTracer;
    using System;
    using OpenTracing;

    internal static class Program
    {
        private static void Main()
        {
            var tracer = new BasicTracer();
            IScope scope;
            using (scope = tracer.BuildSpan("name").StartActive())
            {
                scope.Span.SetTag("tag", "value");
                scope.Span.SetBaggageItem("currentName", "name");

                for (int i = 0; i < 5; i++)
                {
                    using (var s = tracer.BuildSpan($"i {i}").AsChildOf(scope.Span).StartActive())
                    {
                        s.Span.SetBaggageItem("currentName", $"i {i}");
                    }
                }
            }
            using (var s2 = tracer.BuildSpan("name2").StartActive()) { }

            // Reactivate?? That's not allowed... what's the purpose of Activate outside of StartActive?
            //using (tracer.ScopeManager.Activate(scope.Span))
            //{
                
            //}
        }
    }
}