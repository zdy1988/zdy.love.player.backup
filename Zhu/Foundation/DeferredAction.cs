﻿namespace Zhu.Foundation
{
    using System;
    using System.Threading;
    using System.Windows;

    /// <summary>
    /// Represents a timer which performs an action on the UI thread when time elapses.  Rescheduling is supported.
    /// Original code from here: https://www.codeproject.com/Articles/32426/Deferring-ListCollectionView-filter-updates-for-a
    /// </summary>
    public sealed class DeferredAction : IDisposable
    {
        private bool IsDiposed = false;
        private Timer DeferTimer = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeferredAction"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        private DeferredAction(Action action) => DeferTimer = new Timer(
            new TimerCallback(s => Application.Current?.Dispatcher?.Invoke(action)));

        /// <summary>
        /// Creates a new DeferredAction.
        /// </summary>
        /// <param name="action">
        /// The action that will be deferred.  It is not performed until after <see cref="Defer"/> is called.
        /// </param>
        /// <returns>The Deferred Action</returns>
        public static DeferredAction Create(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return new DeferredAction(action);
        }

        /// <summary>
        /// Defers performing the action until after time elapses.  Repeated calls will reschedule the action
        /// if it has not already been performed.
        /// </summary>
        /// <param name="delay">
        /// The amount of time to wait before performing the action.
        /// </param>
        public void Defer(TimeSpan delay)
        {
            // Fire action when time elapses (with no subsequent calls).
            DeferTimer.Change(delay, TimeSpan.FromMilliseconds(-1));
        }

        #region IDisposable Implementation

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="asloManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool asloManaged)
        {
            if (IsDiposed) return;
            IsDiposed = true;

            if (DeferTimer != null)
                DeferTimer.Dispose();

            DeferTimer = null;
        }

        #endregion
    }
}
