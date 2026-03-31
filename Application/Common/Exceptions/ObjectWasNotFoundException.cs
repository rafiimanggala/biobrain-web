using System;

namespace Biobrain.Application.Common.Exceptions
{
    public class ObjectWasNotFoundException : Exception
    {
        public ObjectWasNotFoundException(string objectName, object id)
        {
            ObjectName = objectName;
            Id = id;
        }

        public string ObjectName { get; }
        public object Id { get; }
    }
}