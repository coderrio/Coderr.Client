using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Coderr.Client.Config;
using Coderr.Client.ContextCollections;
using Coderr.Client.Contracts;
using Coderr.Client.Processor;
using Coderr.Client.Reporters;

namespace Coderr.Client
{
    /// <summary>
    ///     Starting point for using the codeRR client.
    /// </summary>
    public class Err
    {
        private static readonly ExceptionProcessor _exceptionProcessor;

        static Err()
        {
            _exceptionProcessor = new ExceptionProcessor(Configuration);
            try
            {
                var value = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                            ?? Environment.GetEnvironmentVariable("APPLICATION_ENVIRONMENT");
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Configuration.EnvironmentName = value;
                    return;
                }

                if (Debugger.IsAttached)
                    Configuration.EnvironmentName = "Development";
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {

            }
        }

        /// <summary>
        ///     Access configuration options
        /// </summary>
        public static CoderrConfiguration Configuration { get; } = new CoderrConfiguration();

        /// <summary>
        ///     Will generate a report without uploading it.
        /// </summary>
        /// <param name="exception">Exception that you want to get reported</param>
        /// <returns>Unique identifier for this report (generated using <see cref="ReportIdGenerator" />)</returns>
        /// <exception cref="System.ArgumentNullException">exception</exception>
        /// <remarks>
        ///     <para>
        ///         A lot if context information is also included in the error report. You can configure the attached information
        ///         by
        ///         using <c>Err.Configuration.ContextProviders.Add()</c>
        ///     </para>
        ///     <para>
        ///         All library exceptions are directed to the <c>Err.ReportingFailed</c> event.
        ///         Subscribe on that event if you have trouble with reporting exceptions.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     <code>
        /// public ActionResult Activate(UserViewModel model)
        /// {
        /// 	if (!ModelState.IsValid)
        /// 		return View(model);
        /// 		
        /// 	try
        /// 	{
        /// 		var user = _repos.GetUser(model.Id);
        /// 		user.Activate(model.ActivationCode);
        /// 		_repos.Save(user);
        /// 		return RedirectToAction("Welcome");
        /// 	}
        /// 	catch (Exception exception)
        /// 	{
        /// 		Err.Report(exception);
        /// 	}
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="UploadReport" />
        public static ErrorReportDTO GenerateReport(Exception exception)
        {
            return _exceptionProcessor.Build(exception);
        }

        /// <summary>
        ///     Will generate a report without uploading it.
        /// </summary>
        /// <param name="exception">Exception that you want to get reported</param>
        /// <param name="contextData">
        ///     Context specific information which would make it easier to reproduce and correct the
        ///     exception. See <see cref="ObjectToContextCollectionConverter" /> to understand what kind of information you can
        ///     attach.
        /// </param>
        /// <returns>
        ///     Error report entity (will return <c>null</c> if <c>Configuration.ThrowExceptions</c> is false and something
        ///     failed)
        /// </returns>
        /// <exception cref="System.ArgumentNullException">exception</exception>
        /// <remarks>
        ///     <para>
        ///         A lot if context information is also included in the error report. You can configure the attached information
        ///         by
        ///         using <c>Err.Configuration.ContextProviders.Add()</c>
        ///     </para>
        ///     <para>
        ///         All library exceptions are directed to the <c>Err.ReportingFailed</c> event.
        ///         Subscribe on that event if you have trouble with reporting exceptions.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     <code>
        /// public ActionResult Activate(UserViewModel model)
        /// {
        /// 	if (!ModelState.IsValid)
        /// 		return View(model);
        /// 		
        /// 	try
        /// 	{
        /// 		var user = _repos.GetUser(model.Id);
        /// 		user.Activate(model.ActivationCode);
        /// 		_repos.Save(user);
        /// 		return RedirectToAction("Welcome");
        /// 	}
        /// 	catch (Exception exception)
        /// 	{
        /// 		Err.Report(exception);
        /// 	}
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="UploadReport" />
        public static ErrorReportDTO GenerateReport(Exception exception, object contextData)
        {
            try
            {
                return _exceptionProcessor.Build(exception, contextData);
            }
            catch
            {
                if (Configuration.ThrowExceptions)
                    throw;
                return null;
            }
        }

        /// <summary>
        ///     Generate an error report
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>generated report</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ErrorReportDTO GenerateReport(IErrorReporterContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            ErrorReporterContext.MoveCollectionsInException(context.Exception, context.ContextCollections);
            return _exceptionProcessor.Build(context);
        }

        /// <summary>
        ///     A user have written information about what he/she did when the exception was thrown; or the user want to get
        ///     information about the bug fixing progress (i.e. want to know when the error is corrected).
        /// </summary>
        /// <param name="errorId">Id generated by this library. Returned when you invoke <see cref="Report(System.Exception)" />.</param>
        /// <param name="emailAddress">user want to receive status updates</param>
        /// <param name="stepsToReproduce">user specified what he/she did when the exception occurred.</param>
        public static void LeaveFeedback(string errorId, string emailAddress, string stepsToReproduce)
        {
            if (errorId == null) throw new ArgumentNullException("errorId");

            try
            {
                Configuration.Uploaders.Upload(new FeedbackDTO
                {
                    Description = emailAddress,
                    EmailAddress = stepsToReproduce,
                    ReportId = errorId
                });
            }
            catch
            {
                if (Configuration.ThrowExceptions)
                    throw;
            }
        }

        /// <summary>
        ///     Report an exception using a custom context
        /// </summary>
        /// <param name="reporterContext">Context used to be able to collect context information</param>
        /// <param name="errorContextModel">Extra context collection(s).</param>
        public static void Report(IErrorReporterContext reporterContext, object errorContextModel)
        {
            if (reporterContext == null) throw new ArgumentNullException(nameof(reporterContext));
            ErrorReporterContext.MoveCollectionsInException(reporterContext.Exception,
                reporterContext.ContextCollections);
            var collection = errorContextModel.ToContextCollection();
            reporterContext.ContextCollections.Add(collection);
            _exceptionProcessor.Process(reporterContext);
        }

        /// <summary>
        ///     Report an exception directly.
        /// </summary>
        /// <param name="exception">Exception that you want to get reported</param>
        /// <returns>Unique identifier for this report (generated using <see cref="ReportIdGenerator" />)</returns>
        /// <exception cref="System.ArgumentNullException">exception</exception>
        /// <remarks>
        ///     <para>
        ///         A lot if context information is also included in the error report. You can configure the attached information
        ///         by
        ///         using <c>Err.Configuration.ContextProviders.Add()</c>
        ///     </para>
        ///     <para>
        ///         All library exceptions are directed to the <c>Err.ReportingFailed</c> event.
        ///         Subscribe on that event if you have trouble with reporting exceptions.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     <code>
        /// public ActionResult Activate(UserViewModel model)
        /// {
        /// 	if (!ModelState.IsValid)
        /// 		return View(model);
        /// 		
        /// 	try
        /// 	{
        /// 		var user = _repos.GetUser(model.Id);
        /// 		user.Activate(model.ActivationCode);
        /// 		_repos.Save(user);
        /// 		return RedirectToAction("Welcome");
        /// 	}
        /// 	catch (Exception exception)
        /// 	{
        /// 		Err.Report(exception);
        /// 	}
        /// }
        /// </code>
        /// </example>
        public static void Report(Exception exception)
        {
            try
            {
                _exceptionProcessor.Process(exception);
            }
            catch
            {
                if (Configuration.ThrowExceptions)
                    throw;
            }
        }

        /// <summary>
        ///     Report an exception directly.
        /// </summary>
        /// <param name="exception">Exception that you want to get reported</param>
        /// <param name="contextData">
        ///     Context specific information which would make it easier to reproduce and correct the
        ///     exception. See <see cref="ObjectToContextCollectionConverter" /> to understand what kind of information you can
        ///     attach.
        /// </param>
        /// <returns>Unique identifier for this report (generated using <see cref="ReportIdGenerator" />)</returns>
        /// <exception cref="System.ArgumentNullException">exception</exception>
        /// <remarks>
        ///     <para>
        ///         Context information will be collected and included in the error report. You can configure the attached
        ///         information
        ///         by
        ///         using <c>Err.Configuration.ContextProviders</c>
        ///     </para>
        ///     <para>
        ///         All library exceptions are directed to the <c>Err.ReportingFailed</c> event.
        ///         Subscribe on that event if you have trouble with reporting exceptions.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     <code>
        /// public ActionResult Activate(UserViewModel model)
        /// {
        /// 	if (!ModelState.IsValid)
        /// 		return View(model);
        /// 		
        /// 	try
        /// 	{
        /// 		var user = _repos.GetUser(model.Id);
        /// 		user.Activate(model.ActivationCode);
        /// 		_repos.Save(user);
        /// 		return RedirectToAction("Welcome");
        /// 	}
        /// 	catch (Exception exception)
        /// 	{
        /// 		Err.Report(exception, model);
        /// 	}
        /// }
        /// </code>
        /// </example>
        public static void Report(Exception exception, object contextData)
        {
            if (exception == null) throw new ArgumentNullException("exception");

            try
            {
                _exceptionProcessor.Process(exception, contextData);
            }
            catch
            {
                if (Configuration.ThrowExceptions)
                    throw;
            }
        }

        /// <summary>
        ///     Report an exception directly.
        /// </summary>
        /// <param name="errorReporterContext">custom reporting context, will be used to be able to pick up context collections</param>
        /// <returns>Unique identifier for this report (generated using <see cref="ReportIdGenerator" />)</returns>
        /// <exception cref="System.ArgumentNullException">exception</exception>
        /// <remarks>
        ///     <para>
        ///         Context information will be collected and included in the error report. You can configure the attached
        ///         information
        ///         by
        ///         using <c>Err.Configuration.ContextProviders</c>
        ///     </para>
        ///     <para>
        ///         All library exceptions are directed to the <c>Err.ReportingFailed</c> event.
        ///         Subscribe on that event if you have trouble with reporting exceptions.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     <code>
        /// public ActionResult Activate(UserViewModel model)
        /// {
        /// 	if (!ModelState.IsValid)
        /// 		return View(model);
        /// 		
        /// 	try
        /// 	{
        /// 		var user = _repos.GetUser(model.Id);
        /// 		user.Activate(model.ActivationCode);
        /// 		_repos.Save(user);
        /// 		return RedirectToAction("Welcome");
        /// 	}
        /// 	catch (Exception exception)
        /// 	{
        /// 		Err.Report(exception, model);
        /// 	}
        /// }
        /// </code>
        /// </example>
        public static void Report(IErrorReporterContext errorReporterContext)
        {
            if (errorReporterContext == null) throw new ArgumentNullException(nameof(errorReporterContext));

            try
            {
                _exceptionProcessor.Process(errorReporterContext);
            }
            catch
            {
                if (Configuration.ThrowExceptions)
                    throw;
            }
        }

        /// <summary>
        ///     Report a logical error to Coderr.
        /// </summary>
        /// <param name="errorMessage">Explains the error (the reason to why you reported it)</param>
        /// <param name="contextData">
        ///     Context information. Either an array of <see cref="ContextCollectionDTO" /> objects or a
        ///     single object (like a view model)
        /// </param>
        /// <param name="errorId">
        ///     Define your own unique identifier for this error. The message + the calling message are otherwise
        ///     used to generate an hash identifier for this method
        /// </param>
        /// <remarks>
        ///     <para>
        ///         Logical errors are bugs where you expected a specific state in your application whole you received something
        ///         else. Your method can still continue and deliver the expected result. However, it can lead
        ///         to other bugs in the future so you still want to bring it to attention.
        ///     </para>
        ///     <para>
        ///         Logical errors are reported to Coderr with the tag "logical-error" so that you can easily find them. If you are
        ///         using Coderr they also recieve a lower priority by the prioritization feature.
        ///     </para>
        /// </remarks>
        public static void ReportLogicError(string errorMessage, object contextData = null, string errorId = null)
        {
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));

            var collections = new List<ContextCollectionDTO>();
            if (contextData != null) AppendCustomContextData(contextData, collections);
            collections.AddTag("logical-error");


            string callerName = "NotSupported";
            string trace = "NotSupported";
#if NETSTANDARD2_0
            var method = new StackFrame(1).GetMethod();
            callerName = method.DeclaringType?.FullName + ":" + method.Name;
            trace = new StackTrace().ToString();
            var pos = trace.IndexOf("\r\n", StringComparison.Ordinal);
            if (pos == -1)
            {
                pos = trace.IndexOf("\n", StringComparison.Ordinal);
                if (pos != -1) trace = trace.Remove(0, pos + 1);
            }
            else
            {
                trace = trace.Remove(0, pos + 2);
            }
#endif
            var logicException =
                new LogicalErrorException(errorMessage, trace)
                {
                    ErrorHashSource = errorId ?? $"{errorMessage}:{callerName}"
                };
            Report(logicException, collections);
        }

        /// <summary>
        ///     Upload an error report.
        /// </summary>
        /// <param name="dto">Typically generated by <see cref="GenerateReport(System.Exception)" />.</param>
        public static void UploadReport(ErrorReportDTO dto)
        {
            if (dto == null) throw new ArgumentNullException("dto");


            try
            {
                EnsureApplicationVersion(dto);
                Configuration.Uploaders.Upload(dto);
            }
            catch
            {
                if (Configuration.ThrowExceptions)
                    throw;
            }
        }

        private static void AppendCustomContextData(object contextData, IList<ContextCollectionDTO> contextInfo)
        {
            if (contextData is IEnumerable<ContextCollectionDTO> dtos)
            {
                var arr = dtos;
                foreach (var dto in arr)
                    contextInfo.Add(dto);
            }
            else
            {
                var col = contextData.ToContextCollection();
                contextInfo.Add(col);
            }
        }

        private static void EnsureApplicationVersion(ErrorReportDTO dto)
        {
            foreach (var collection in dto.ContextCollections)
            {
                if (collection.Properties.TryGetValue(ExceptionProcessor.AppAssemblyVersion, out var version))
                    return;
            }

            _exceptionProcessor.AddAddemblyVersion(dto.ContextCollections);
        }
    }
}