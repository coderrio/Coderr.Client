using System;

namespace Coderr.Client
{
    internal class LogicalErrorException : Exception
    {
        public LogicalErrorException(string msg, string stackTrace) : base(msg)
        {
            StackTrace = stackTrace;
        }

        /// <summary>Gets a string representation of the immediate frames on the call stack.</summary>
        /// <returns>A string that describes the immediate frames of the call stack.</returns>
        /// <filterpriority>2</filterpriority>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" PathDiscovery="*AllFiles*" />
        /// </PermissionSet>
        public override string StackTrace { get; }

        /// <summary>
        /// Used to identify this error.
        /// </summary>
        public string ErrorHashSource { get; set; }
    }
}