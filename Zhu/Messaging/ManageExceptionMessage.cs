using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.Messaging
{
    /// <summary>
    /// Used to broadcast an exception
    /// </summary>
    public class ManageExceptionMessage : MessageBase
    {
        /// <summary>
        /// The unhandled exception
        /// </summary>
        public readonly Exception UnHandledException;

        /// <summary>
        /// Initialize a new instance of ManageExceptionMessage class
        /// </summary>
        /// <param name="unHandledException">The exception to broadcast</param>
        public ManageExceptionMessage(Exception unHandledException)
        {
            UnHandledException = unHandledException;
        }
    }
}
