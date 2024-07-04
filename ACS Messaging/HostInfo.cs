using System;
using System.Net.Security;
using System.Threading;

    /// <summary>
    /// Contains information about a remote host.
    /// </summary>
    public class HostInfo
    {
        /// <summary>
        /// The name of the remote host.
        /// </summary>
        private readonly string _hostName;

        /// <summary>
        /// The port number of the remote host.
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// Use a secure connection to the host.
        /// </summary>
        private readonly bool _secure;

        /// <summary>
        /// Secure stream to use.
        /// </summary>
        private readonly SslStream _securestream;

        /// <summary>
        /// External index information about the host.
        /// </summary>
        private int _index;

        /// <summary>
        /// Provides locking mechanism for writing data for this host.
        /// </summary>
        public SemaphoreSlim WriteLock = new SemaphoreSlim(1);

        /// <summary>
        /// Gets the name of the remote host.
        /// </summary>
        /// <value>
        /// A <b>String</b> containing a host name.
        /// </value>
        public string HostName
        {
            get { return _hostName; }
        }

        /// <summary>
        /// Gets the port number of the remote host.
        /// </summary>
        /// <value>
        /// An <b>Int32</b> containing a port number.
        /// </value>
        public int Port
        {
            get { return _port; }
        }

        /// <summary>
        /// Gets the secure value.
        /// </summary>
        /// <value>
        /// A <b>Boolean</b> containing a valuer.
        /// </value>
        public bool Secure
        {
            get { return _secure; }
        }

        /// <summary>
        /// Gets the secure stream.
        /// </summary>
        /// <value>
        /// An <b>SslStream</b> containing the secure stream.
        /// </value>
        public SslStream SecureStream
        {
            get { return _securestream; }
        }

        /// <summary>
        /// Gets or Sets the index number of the host.
        /// </summary>
        /// <value>
        /// An <b>Int32</b> containing an index number.
        /// </value>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        /// <summary>
        /// Gets or Sets the Challenge used for further validation.
        /// </summary>
        /// <value>
        /// A <b>String</b> containing a challenge.
        /// </value>
        public string Challenge { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="HostInfo" /> class.
        /// </summary>
        /// <param name="hostName">
        /// The name of the remote host.
        /// </param>
        /// <param name="port">
        /// The port number of the remote host.
        /// </param>
        /// <param name="secure">
        /// Use a secure connection to the remote host.
        /// </param>
        /// <param name="securestream">
        /// Secure stream.
        /// </param>
        public HostInfo(string hostName, int port, bool secure, SslStream securestream = null)
        {
            _hostName = hostName;
            _port = port;
            _secure = secure;
            _securestream = securestream;
        }

        /// <summary>
        /// Compares the current instance to another object for value equality.
        /// </summary>
        /// <param name="host">
        /// The object to which the current instance is compared.
        /// </param>
        /// <returns>
        /// <b>true</b> if both the host name and port of both instances are the same; otherwise, <b>False</b>.
        /// </returns>
        /// <remarks>
        /// The comparison of host names is case-insensitive.
        /// </remarks>
        public bool Equals(HostInfo host)
        {
            return HostName.Equals(host.HostName, StringComparison.CurrentCultureIgnoreCase) && Port == host.Port;
        }

        /// <summary>
        /// Overridden. Compares the current instance to another object for value equality.
        /// </summary>
        /// <param name="obj">
        /// The object to which the current instance is compared.
        /// </param>
        /// <returns>
        /// <b>true</b> if <i>obj</i> is a <see cref="HostInfo" /> value and both the host name and port of both instances are the same; otherwise, <b>False</b>.
        /// </returns>
        /// <remarks>
        /// The comparison of host names is case-insensitive.
        /// </remarks>
        public override bool Equals(object obj)
        {
            return obj is HostInfo && Equals((HostInfo)obj);
        }

    /// <summary>
    /// Overridden. Returns the hash code for this instance.
    /// </summary>
    /// <returns>
    /// <b>Int32</b> for a <see cref="HostInfo" /> as a hash.
    /// </returns>
    /// <remarks>
    /// Returns the hash code for this instance.
    /// </remarks>
    public override int GetHashCode()
    {
        return (_hostName, _port).GetHashCode();
    }

    /// <summary>
    /// Overridden.  Returns a string representation of the <see cref="HostInfo" /> object.
    /// </summary>
    /// <returns>
    /// A <b>String</b> containing the host name and port number, separated by a colon.
    /// </returns>
    public override string ToString()
        {
            return _hostName + ":" + _port;
        }
    }