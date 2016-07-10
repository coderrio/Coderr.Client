using System;
using System.Collections.Generic;
using System.Threading;

namespace OneTrueError.Client.Uploaders
{
    /// <summary>
    ///     Purpose of this class is to take care of DTO queing to be able to upload reports in a structured manner.
    /// </summary>
    /// <typeparam name="T">Type of entity to queue</typeparam>
    public class UploadQueue<T> : IDisposable where T : class
    {
        private readonly Queue<T> _queue = new Queue<T>();
        private readonly ManualResetEvent _quitEvent = new ManualResetEvent(false);
        private readonly Action<T> _uploadAction;
        private int _attemptNumber;

        /// <summary>
        ///     Create a new instance of <see cref="UploadQueue{T}" />.
        /// </summary>
        /// <param name="uploadAction">
        ///     Action to invoke for the DTO that should be uploaded. Thrown exceptions are used to indicate
        ///     that a retry should be made.
        /// </param>
        public UploadQueue(Action<T> uploadAction)
        {
            if (uploadAction == null) throw new ArgumentNullException("uploadAction");
            _uploadAction = uploadAction;
            MaxQueueSize = 10;
            MaxAttempts = 3;
            RetryInterval = TimeSpan.FromSeconds(5);
        }

        /// <summary>
        ///     Max number of upload attempts per report.
        /// </summary>
        /// <value>Default is 3.</value>
        public int MaxAttempts { get; set; }

        /// <summary>
        ///     Max number of items that may wait in queue to get uploaded.
        /// </summary>
        /// <value>
        ///     Default is 10.
        /// </value>
        public int MaxQueueSize { get; set; }

        /// <summary>
        ///     An action to run in the background thread to check wether an upload can be made.
        /// </summary>
        /// <remarks>
        ///     Typically used to check for connectivity.
        /// </remarks>
        public Func<bool> PreConditionAction { get; set; }

        /// <summary>
        ///     Amount of time to wait between each attempt.
        /// </summary>
        /// <value>
        ///     Default is 5 seconds.
        /// </value>
        public TimeSpan RetryInterval { get; set; }

        /// <summary>
        ///     Dispose queue
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Add a new item to the queue. Will be uploaded directly if the queue is empty.
        /// </summary>
        /// <param name="item">item to enqueue</param>
        /// <exception cref="ArgumentNullException">item</exception>
        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (_queue.Count >= MaxQueueSize)
            {
                if (UploadFailed != null)
                    UploadFailed(this, new UploadReportFailedEventArgs(new Exception("Too large queue"), item));
                return;
            }


            lock (_queue)
            {
                _queue.Enqueue(item);
                if (_queue.Count == 1)
                    ThreadPool.QueueUserWorkItem(TryUploadItem);
            }
        }

        /// <summary>
        ///     Thread safe conditional add
        /// </summary>
        /// <param name="dto">DTO to add to the queue</param>
        /// <returns><c>true</c> if item was added to the queue; otherwise <c>false</c></returns>
        public bool AddIfNotEmpty(T dto)
        {
            if (dto == null) throw new ArgumentNullException("dto");

            lock (_queue)
            {
                if (_queue.Count == 0)
                    return false;

                _queue.Enqueue(dto);
            }

            return true;
        }

        /// <summary>
        ///     Failed to deliver DTO within the given parameters.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public event EventHandler<UploadReportFailedEventArgs> UploadFailed;

        /// <summary>
        ///     Dispose pattern
        /// </summary>
        /// <param name="isFromDispose">invoked from Dispose() method.</param>
        protected virtual void Dispose(bool isFromDispose)
        {
            _quitEvent.Set();
        }

        private void TryUploadItem(object state)
        {
            while (true)
            {
                try
                {
                    if (!PreConditionAction())
                    {
                        if (_quitEvent.WaitOne(RetryInterval))
                            return;
                        continue;
                    }

                    T item;
                    lock (_queue)
                    {
                        item = _queue.Peek();
                    }

                    _uploadAction(item);
                    _attemptNumber = 0;
                    lock (_queue)
                    {
                        _queue.Dequeue();
                    }
                }
                catch (Exception ex)
                {
                    _attemptNumber++;
                    if (_attemptNumber >= MaxAttempts)
                    {
                        object failedItem;
                        lock (_queue)
                        {
                            failedItem = _queue.Dequeue();
                        }
                        _attemptNumber = 0;
                        if (UploadFailed != null)
                            UploadFailed(this, new UploadReportFailedEventArgs(ex, failedItem));
                    }
                    else
                    {
                        if (_quitEvent.WaitOne(RetryInterval))
                            return;
                    }
                }
            }
        }
    }
}