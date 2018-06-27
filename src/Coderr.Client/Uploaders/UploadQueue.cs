using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Coderr.Client.Uploaders
{
    /// <summary>
    ///     Purpose of this class is to take care of DTO queing to be able to upload reports in a structured manner.
    /// </summary>
    /// <typeparam name="T">Type of entity to queue</typeparam>
    public class UploadQueue<T> : IDisposable where T : class
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private readonly ManualResetEvent _quitEvent = new ManualResetEvent(false);
        private readonly Action<T> _uploadAction;
        private readonly ManualResetEventSlim _waitForCompletion = new ManualResetEventSlim(false);
        private int _attemptNumber;
        private Func<bool> _preConditionAction;
        private int _queueTasks;

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
            PreConditionAction = () => true;
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
        ///     An action to run in the background thread to check whether an upload can be made.
        /// </summary>
        /// <remarks>
        ///     Typically used to check for connectivity.
        /// </remarks>
        public Func<bool> PreConditionAction
        {
            get => _preConditionAction;
            set { _preConditionAction = value ?? (() => true); }
        }

        /// <summary>
        ///     Amount of time to wait between each attempt.
        /// </summary>
        /// <value>
        ///     Default is 5 seconds.
        /// </value>
        public TimeSpan RetryInterval { get; set; }

        internal bool ActivateSync { get; set; }

        internal bool TaskWasInvoked { get; set; }

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

            _queue.Enqueue(item);
            if (Interlocked.CompareExchange(ref _queueTasks, 1, 0) == 0)
            {
                if (ActivateSync)
                    _quitEvent.Reset();
                ThreadPool.QueueUserWorkItem(TryUploadItem);
            }
        }

        /// <summary>
        ///     Add report queue if the queue is empty; otherwise invoke the delegate.
        /// </summary>
        /// <param name="dto">DTO to add to the queue</param>
        /// <param name="uploadTask">Task to invoke if queue is empty</param>
        /// <returns><c>true</c> if item was added to the queue; otherwise <c>false</c></returns>
        public bool AddIfNotEmpty(T dto, Action uploadTask)
        {
            if (dto == null) throw new ArgumentNullException("dto");


            if (Interlocked.CompareExchange(ref _queueTasks, 1, 1) == 1)
            {
                _queue.Enqueue(dto);
                return true;
            }

            uploadTask();
            return false;
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

        internal void Wait(int ms)
        {
            _waitForCompletion.Wait(ms);
        }

        private void TryUploadItem(object state)
        {
            if (ActivateSync)
                TaskWasInvoked = true;

            while (true)
                try
                {
                    if (!PreConditionAction())
                    {
                        if (_quitEvent.WaitOne(RetryInterval))
                            break;
                        continue;
                    }

                    T item;
                    if (!_queue.TryPeek(out item))
                        break;


                    _uploadAction(item);
                    _attemptNumber = 0;
                    _queue.TryDequeue(out item);
                }
                catch (Exception ex)
                {
                    _attemptNumber++;
                    if (_attemptNumber >= MaxAttempts)
                    {
                        T failedItem;
                        _queue.TryDequeue(out failedItem);
                        _attemptNumber = 0;
                        if (UploadFailed != null)
                            UploadFailed(this, new UploadReportFailedEventArgs(ex, failedItem));
                    }
                    else
                    {
                        if (_quitEvent.WaitOne(RetryInterval))
                            break;
                    }
                }
            Interlocked.Exchange(ref _queueTasks, 0);

            if (ActivateSync)
                _waitForCompletion.Set();
        }
    }
}