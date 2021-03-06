﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace IKW.Contropolus.Prism.CastleWindsor.WPF.ExceptionResolution
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ResolutionFailedException : Exception
    {
        public ResolutionFailedException(Type typeRequested,
                                         string nameRequested,
                                         string message,
                                         Exception innerException = null) : base(message, innerException)
        {
            Type type = typeRequested;

            if ((object) type == null)
                throw new ArgumentNullException(nameof (typeRequested));

            this.TypeRequested = type.GetTypeInfo().Name;
            this.NameRequested = nameRequested;

            this.RegisterSerializationHandler();
        }

        public string TypeRequested { get; private set; }

        public string NameRequested { get; private set; }

        private void RegisterSerializationHandler()
        {
            this.SerializeObjectState += (EventHandler<SafeSerializationEventArgs>) ((s, e) =>
                e.AddSerializedState(
                    (ISafeSerializationData) new ResolutionFailedException.ResolutionFailedExceptionSerializationData(
                        this.TypeRequested, this.NameRequested)));
        }

        [Serializable]
        private struct ResolutionFailedExceptionSerializationData : ISafeSerializationData
        {
            private readonly string _typeRequested;
            private readonly string _nameRequested;

            public ResolutionFailedExceptionSerializationData(string typeRequested, string nameRequested)
            {
                this._typeRequested = typeRequested;
                this._nameRequested = nameRequested;
            }

            public void CompleteDeserialization(object deserialized)
            {
                ResolutionFailedException resolutionFailedException = (ResolutionFailedException) deserialized;
                resolutionFailedException.TypeRequested = this._typeRequested;
                resolutionFailedException.NameRequested = this._nameRequested;
            }
        }

        protected ResolutionFailedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }

        public ResolutionFailedException()
        {
        }

        public ResolutionFailedException(string message) : base(message)
        {
        }

        public ResolutionFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
