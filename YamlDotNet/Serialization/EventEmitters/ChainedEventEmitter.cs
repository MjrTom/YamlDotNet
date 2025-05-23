﻿// This file is part of YamlDotNet - A .NET library for YAML.
// Copyright (c) Antoine Aubry and contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using YamlDotNet.Core;

namespace YamlDotNet.Serialization.EventEmitters
{
    /// <summary>
    /// Provided the base implementation for an IEventEmitter that is a
    /// decorator for another IEventEmitter.
    /// </summary>
    public abstract class ChainedEventEmitter : IEventEmitter
    {
        protected readonly IEventEmitter nextEmitter;

        protected ChainedEventEmitter(IEventEmitter nextEmitter)
        {
            this.nextEmitter = nextEmitter ?? throw new ArgumentNullException(nameof(nextEmitter));
        }

        /// <summary>
        /// Emits the.
        /// </summary>
        /// <param name="eventInfo">The event info.</param>
        /// <param name="emitter">The emitter.</param>
        public virtual void Emit(AliasEventInfo eventInfo, IEmitter emitter)
        {
            nextEmitter.Emit(eventInfo, emitter);
        }

        /// <summary>
        /// Emits the.
        /// </summary>
        /// <param name="eventInfo">The event info.</param>
        /// <param name="emitter">The emitter.</param>
        public virtual void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
        {
            nextEmitter.Emit(eventInfo, emitter);
        }

        /// <summary>
        /// Emits the.
        /// </summary>
        /// <param name="eventInfo">The event info.</param>
        /// <param name="emitter">The emitter.</param>
        public virtual void Emit(MappingStartEventInfo eventInfo, IEmitter emitter)
        {
            nextEmitter.Emit(eventInfo, emitter);
        }

        /// <summary>
        /// Emits the.
        /// </summary>
        /// <param name="eventInfo">The event info.</param>
        /// <param name="emitter">The emitter.</param>
        public virtual void Emit(MappingEndEventInfo eventInfo, IEmitter emitter)
        {
            nextEmitter.Emit(eventInfo, emitter);
        }

        /// <summary>
        /// Emits the.
        /// </summary>
        /// <param name="eventInfo">The event info.</param>
        /// <param name="emitter">The emitter.</param>
        public virtual void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter)
        {
            nextEmitter.Emit(eventInfo, emitter);
        }

        /// <summary>
        /// Emits the.
        /// </summary>
        /// <param name="eventInfo">The event info.</param>
        /// <param name="emitter">The emitter.</param>
        public virtual void Emit(SequenceEndEventInfo eventInfo, IEmitter emitter)
        {
            nextEmitter.Emit(eventInfo, emitter);
        }
    }
}
