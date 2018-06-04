using System;
using System.Runtime.Serialization;

namespace UltimateInjector
{
    [Serializable]
    public class TypeIsNotAssignedException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public TypeIsNotAssignedException()
        {
        }

        public TypeIsNotAssignedException(string message) : base(message)
        {
        }

        public TypeIsNotAssignedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TypeIsNotAssignedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}