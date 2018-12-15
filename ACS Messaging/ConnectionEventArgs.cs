using System;

    /// <summary>
    /// Contains data for connection-related events.
    /// </summary>
    public class ConnectionEventArgs : EventArgs
    {
        /// <summary>
        /// Information about the remote host.
        /// </summary>
        private readonly HostInfo _host;

        /// <summary>
        /// Gets information about the remote host.
        /// </summary>
        /// <value>
        /// A <see cref="HostInfo" /> object representing the remote host.
        /// </value>
        public HostInfo Host
        {
            get { return _host; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConnectionEventArgs" /> class.
        /// </summary>
        /// <param name="host">
        /// The remote host.
        /// </param>
        public ConnectionEventArgs(HostInfo host)
        {
            _host = host;
        }
    }