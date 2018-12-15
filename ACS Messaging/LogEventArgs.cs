using System;

namespace ACS.Messaging
{
    /// <summary>
    /// Contains data for the <see cref="MessageClientServerBase.Log">Exception</see> event.
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        /// <summary>
        /// Date and Time for the log.
        /// </summary>
        private readonly DateTime Time;

        /// <summary>
        /// The type of log information.
        /// </summary>
        private readonly string Type;

        /// <summary>
        /// Message to log.
        /// </summary>
        private readonly string Message;

        /// <summary>
        /// Gets information about the Log date and time.
        /// </summary>
        /// <value>
        /// A <see cref="DateTime" /> object.
        /// </value>
        public DateTime LogDateTime
        {
            get { return Time; }
        }

        /// <summary>
        /// Gets information about the log type.
        /// </summary>
        /// <value>
        /// A <see cref="string" /> object.
        /// </value>
        public string LogType
        {
            get { return Type; }
        }

        /// <summary>
        /// Gets information about the log message.
        /// </summary>
        /// <value>
        /// A <see cref="string" /> object.
        /// </value>
        public string LogMessage
        {
            get { return Message; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="LogEventArgs" /> class.
        /// </summary>
        /// <param name="LogDateTime">
        /// Date and Time for the log.
        /// </param>
        /// <param name="LogType">
        /// The type of log information.
        /// </param>
        /// <param name="LogMessage">
        /// Message to log.
        /// </param>
        public LogEventArgs(DateTime LogDateTime, string LogType, string LogMessage)
        {
            Time = LogDateTime;
            Type = LogType;
            Message = LogMessage;
        }
    }
}