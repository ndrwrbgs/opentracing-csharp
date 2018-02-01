﻿using System.Collections.Generic;

namespace OpenTracing.Propagation
{
    /// <summary>
    /// <see cref="TextMap"/> is a built-in carrier for <see cref="ITracer.Inject{TCarrier}"/> and
    /// <see cref="ITracer.Extract{TCarrier}"/>. TextMap implementations allows Tracers to read and write key:value String pairs from
    /// arbitrary underlying sources of data.
    /// </summary>
    /// <seealso cref="ITracer.Inject{TCarrier}"/>
    /// <seealso cref="ITracer.Extract{TCarrier}"/>
    public interface TextMap : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>Puts a key:value pair into the TextMapWriter's backing store.</summary>
        /// <param name="key">A string, possibly with constraints dictated by the particular Format this TextMap is paired with</param>
        /// <param name="value">A string, possibly with constraints dictated by the particular Format this TextMap is paired with</param>
        /// <seealso cref="ITracer.Inject{TCarrier}"/>
        /// <seealso cref="BuiltinFormats.TextMap"/>
        /// <seealso cref="BuiltinFormats.HttpHeaders"/>
        void Set(string key, string value);
    }
}