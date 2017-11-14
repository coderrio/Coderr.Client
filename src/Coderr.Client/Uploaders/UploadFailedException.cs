using System;

namespace Coderr.Client.Uploaders
{
    /// <summary>
    ///     Failed to upload an error report.
    /// </summary>
    public class UploadFailedException : CoderrClientException
    {
        public UploadFailedException(string msg, Exception inner) : base(msg, inner)
        {
        }

        public UploadFailedException(string errMsg) : base(errMsg)
        {
            
        }
    }
}