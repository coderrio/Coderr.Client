using System;
using System.Collections.Generic;
using System.Threading;

namespace OneTrueError.Client.Uploaders
{
    /// <summary>
    ///     <para>
    ///         Temp data is used to store reports in memory for at most 20 minutes.
    ///     </para>
    ///     <para>
    ///         The purpose is
    ///         to be able to keep data for instance in web applications where different http requests is
    ///         required to collect information.
    ///     </para>
    /// </summary>
    public class TempData : IDisposable
    {
        private readonly Dictionary<string, TempItem> _items = new Dictionary<string, TempItem>();
        private Timer _cleanUpTimer;

        /// <summary>
        ///     Creates a new instance of <see cref="TempData" />.
        /// </summary>
        public TempData()
        {
            _cleanUpTimer = new Timer(OnCleanup, null, 5000, 5000);
        }

        /// <summary>
        ///     Access an item (items are automatically deleted when retrieved)
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Item or <c>null</c></returns>
        public object this[string id]
        {
            get
            {
                lock (_items)
                {
                    TempItem item;
                    if (!_items.TryGetValue(id, out item))
                        return null;

                    _items.Remove(id);
                    return item.Value;
                }
            }
            set
            {
                lock (_items)
                {
                    _items[id] = new TempItem(value);
                }
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Dispose pattern.
        /// </summary>
        /// <param name="isDisposing">Invoked from Dispose() method.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_cleanUpTimer != null)
            {
                _cleanUpTimer.Dispose();
                _cleanUpTimer = null;
            }
        }

        private void Cleanup()
        {
            lock (_items)
            {
                var idsToRemove = new Queue<string>();
                foreach (var kvp in _items)
                    if (kvp.Value.Expired)
                        idsToRemove.Enqueue(kvp.Key);

                while (idsToRemove.Count > 0)
                    _items.Remove(idsToRemove.Dequeue());
            }
        }

        private void OnCleanup(object state)
        {
            try
            {
                Cleanup();
            }
            catch
            {
                //TODO: Invoke internal event.
            }
        }

        private class TempItem
        {
            public TempItem(object value)
            {
                CreatedAtUtc = DateTime.UtcNow;
                Value = value;
            }

            public bool Expired => DateTime.UtcNow.Subtract(CreatedAtUtc).TotalMinutes > 20;

            public object Value { get; }

            private DateTime CreatedAtUtc { get; }
        }
    }
}