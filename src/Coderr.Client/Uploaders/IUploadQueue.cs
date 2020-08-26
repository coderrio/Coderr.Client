using System;

namespace Coderr.Client.Uploaders
{
    /// <summary>
    /// Queue used to upload reports to an analyser.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUploadQueue<in T> where T : class
    {
        ///// <summary>
        /////     Max number of upload attempts per report.
        ///// </summary>
        ///// <value>Default is 3.</value>
        //int MaxAttempts { get; set; }

        ///// <summary>
        /////     Max number of items that may wait in queue to get uploaded.
        ///// </summary>
        ///// <value>
        /////     Default is 10.
        ///// </value>
        //int MaxQueueSize { get; set; }

        ///// <summary>
        /////     An action to run in the background thread to check whether an upload can be made.
        ///// </summary>
        ///// <remarks>
        /////     Typically used to check for connectivity.
        ///// </remarks>
        //Func<bool> PreConditionAction { get; set; }

        ///// <summary>
        /////     Amount of time to wait between each attempt.
        ///// </summary>
        ///// <value>
        /////     Default is 5 seconds.
        ///// </value>
        //TimeSpan RetryInterval { get; set; }

        /// <summary>
        ///     Dispose queue
        /// </summary>
        void Dispose();

        /// <summary>
        ///     Add a new item to the queue. Will be uploaded directly if the queue is empty.
        /// </summary>
        /// <param name="item">item to enqueue</param>
        /// <exception cref="ArgumentNullException">item</exception>
        void Enqueue(T item);

        /// <summary>
        ///     Enqueue DTO if the queue is empty; otherwise invoke the delegate.
        /// </summary>
        /// <param name="dto">DTO to add to the queue</param>
        /// <param name="uploadTask">Task to invoke if queue is empty</param>
        /// <returns><c>true</c> if item was added to the queue; otherwise <c>false</c></returns>
        bool EnqueueIfNotEmpty(T dto, Action uploadTask);

        /// <summary>
        ///     Failed to deliver DTO within the given parameters.
        /// </summary>
        /// <remarks>
        /// </remarks>
        event EventHandler<UploadReportFailedEventArgs> UploadFailed;
    }
}