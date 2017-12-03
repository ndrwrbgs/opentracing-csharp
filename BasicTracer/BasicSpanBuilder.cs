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

namespace BasicTracer
{
    using System;
    using System.Collections.Generic;
    using OpenTracing;

    internal sealed class BasicSpanBuilder : ISpanBuilder
    {
        private readonly string operationName;
        private readonly BasicTracer tracer;
        private BasicSpanContext firstParent;
        private bool ignoringActiveSpan;
        private readonly IDictionary<string, object> initialTags = new Dictionary<string, object>();
        private DateTimeOffset startDateTimeOffset;

        public BasicSpanBuilder(BasicTracer tracer, string operationName)
        {
            this.tracer = tracer;
            this.operationName = operationName;
        }

        public ISpanBuilder AsChildOf(ISpanContext parent)
        {
            return this.AddReference(References.ChildOf, parent);
        }

        public ISpanBuilder AsChildOf(ISpan parent)
        {
            return this.AddReference(References.ChildOf, parent.Context());
        }

        public ISpanBuilder AddReference(string referenceType, ISpanContext referencedContext)
        {
            if (this.firstParent == null
                && (referenceType.Equals(References.ChildOf) || referenceType.Equals(References.FollowsFrom)))
            {
                this.firstParent = (BasicSpanContext) referencedContext;
            }
            return this;
        }

        public ISpanBuilder IgnoreActiveSpan()
        {
            this.ignoringActiveSpan = true;
            return this;
        }

        public ISpanBuilder WithTag(string key, string value)
        {
            this.initialTags[key] = value;
            return this;
        }

        public ISpanBuilder WithTag(string key, bool value)
        {
            this.initialTags[key] = value;
            return this;
        }

        public ISpanBuilder WithTag(string key, int value)
        {
            this.initialTags[key] = value;
            return this;
        }

        public ISpanBuilder WithTag(string key, double value)
        {
            this.initialTags[key] = value;
            return this;
        }

        public ISpanBuilder WithStartTimestamp(DateTimeOffset startTime)
        {
            this.startDateTimeOffset = startTime;
            return this;
        }

        public IScope StartActive()
        {
            return this.tracer.ScopeManager.Activate(this.StartManual());
        }

        public IScope StartActive(bool finishSpanOnClose)
        {
            return this.tracer.ScopeManager.Activate(this.StartManual(), finishSpanOnClose);
        }

        public ISpan StartManual()
        {
            if (this.startDateTimeOffset == default(DateTimeOffset))
            {
                this.startDateTimeOffset = DateTimeOffset.Now;
            }

            if (this.firstParent == null && !this.ignoringActiveSpan)
            {
                IScope activeScope = this.tracer.ScopeManager.Active;
                if (activeScope != null)
                {
                    this.firstParent = (BasicSpanContext) activeScope.Span.Context();
                }
            }

            return new BasicSpan(
                this.tracer,
                this.operationName,
                this.startDateTimeOffset,
                this.initialTags,
                this.firstParent);
        }

        public ISpan Start()
        {
            return this.StartManual();
        }
    }
}