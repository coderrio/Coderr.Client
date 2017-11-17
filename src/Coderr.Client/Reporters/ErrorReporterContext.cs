using System;
using System.Collections.Generic;
using codeRR.Client.Contracts;
using Newtonsoft.Json;

namespace codeRR.Client.Reporters
{
    /// <summary>
    ///     Context supplied by error reports
    /// </summary>
    /// <remarks>
    ///     Used to be able to provide application specific context information (for instance HTTP applications can provide the HTTP
    ///     context)
    /// </remarks>
    public class ErrorReporterContext : IErrorReporterContext2
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ErrorReporterContext" /> class.
        /// </summary>
        /// <param name="reporter">The reporter.</param>
        /// <param name="exception">The exception.</param>
        public ErrorReporterContext(object reporter, Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            Reporter = reporter;
            ContextCollections = new List<ContextCollectionDTO>();

            MoveCollectionsInException(exception, ContextCollections);
        }

        /// <summary>
        ///     Gets class which is sending the report
        /// </summary>
        public object Reporter { get; }

        /// <summary>
        ///     Gets caught exception
        /// </summary>
        public Exception Exception { get; }

        /// <inheritdoc />
        public IList<ContextCollectionDTO> ContextCollections { get; }

        /// <summary>
        ///     Can be used to copy collections from an exception to a collection collection ;)
        /// </summary>
        /// <param name="exception">Exception that might contain collections</param>
        /// <param name="destination">target</param>
        public static void MoveCollectionsInException(Exception exception, IList<ContextCollectionDTO> destination)
        {
            var keysToRemove = new List<object>();
            foreach (var key in exception.Data.Keys)
            {
                var keyStr = key?.ToString();
                if (key == null)
                    continue;

                if (keyStr.Equals("ErrCollections") || keyStr.StartsWith("ErrCollections"))
                {
                    keysToRemove.Add(key);
                    var value = exception.Data[key];
                    if (!(value is string valueStr))
                        continue;

                    var data = JsonConvert.DeserializeObject(valueStr, typeof(object), new JsonSerializerSettings{TypeNameHandling = TypeNameHandling.Auto});
                    if (data is IEnumerable<ContextCollectionDTO> cols)
                    {
                        foreach (var col in cols)
                        {
                            destination.Add(col);
                        }
                    }
                }
                else
                {
                    keysToRemove.Add(key);
                    var value = exception.Data[key];
                    if (!(value is string valueStr))
                        continue;

                    var col = JsonConvert.DeserializeObject(valueStr) as ContextCollectionDTO;
                    destination.Add(col);
                }
            }

            foreach (var key in keysToRemove)
                exception.Data.Remove(key);

            if (exception.InnerException != null)
                MoveCollectionsInException(exception.InnerException, destination);
        }
    }
}