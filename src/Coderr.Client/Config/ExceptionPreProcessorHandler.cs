using Coderr.Client.Reporters;

namespace Coderr.Client.Config
{
    /// <summary>
    /// Used to be able to process exceptions before they are converted into DTOs
    /// </summary>
    /// <param name="context">context info</param>
    /// <seealso cref="CoderrConfiguration.ExceptionPreProcessor"/>
    public delegate void ExceptionPreProcessorHandler(IErrorReporterContext context);
}