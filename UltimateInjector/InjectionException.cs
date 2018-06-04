using System;
using System.Runtime.Serialization;

namespace UltimateInjector
{
    [Serializable]
    public class InjectionException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InjectionException()
        {
        }

        public InjectionException(string message) : base(message)
        {
        }

        public InjectionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InjectionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}