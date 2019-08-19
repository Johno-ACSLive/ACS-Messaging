using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ACS.Messaging
{
    public class MessageServer : MessageClientServerBase
    {
        #region " Types "
        ///// <summary>
        ///// Contains application-specific state data for an asynchronous read.
        ///// </summary>
        //private class ReadAsyncState
        //{
        //    /// <summary>
        //    /// The client receiving the data.
        //    /// </summary>
        //    public TcpClient Client;

        //    /// <summary>
        //    /// A <b>Byte</b> array the data is read into.
        //    /// </summary>
        //    public byte[] Buffer;

        //    /// <summary>
        //    /// An <b>SslStream</b> stream for secure clients.
        //    /// </summary>
        //    public SslStream SecureStream;
        //}
        #endregion

        #region " Fields "
        /// <summary>
        /// The connected clients and the corresponding remote host information.
        /// </summary>
        private readonly Dictionary<TcpClient, HostInfo> clients = new Dictionary<TcpClient, HostInfo>();

        /// <summary>
        /// Provides locking object for access to clients.
        /// </summary>
        private object clientslock = new object();

        /// <summary>
        /// Indicates whether or not the current instance has been disposed.
        /// </summary>
        private bool isDisposed = false;

        /// <summary>
        /// The port on which the server is listening for connections.
        /// </summary>
        private int _port;

        /// <summary>
        /// Use a secure connection by the host.
        /// </summary>
        private bool _secure;

        /// <summary>
        /// The certificate to be used by the host.
        /// </summary>
        private X509Certificate servercertificate;

        /// <summary>
        /// The server listening for connections.
        /// </summary>
        private TcpListener server;

        /// <summary>
        /// The maximum number of clients that can be queued for TcpListener to accept.
        /// </summary>
        private int listenerbacklog = 10;
        #endregion

        #region " Properties "
        /// <summary>
        /// Gets the port number on which the server is listening.
        /// </summary>
        /// <value>
        /// An <b>int</b> containing a port number.
        /// </value>
        public int Port
        {
            get { return _port; }
        }

        /// <summary>
        /// Gets the secure value.
        /// </summary>
        /// <value>
        /// A <b>bool</b> containing a valuer.
        /// </value>
        public bool Secure
        {
            get { return _secure; }
        }

        /// <summary>
        /// Gets a list of remote hosts connected to the server.
        /// </summary>
        /// <value>
        /// An array of <see cref="HostInfo" /> objects containing the names and port numbers of the remote hosts.
        /// </value>
        /// <remarks>
        /// The value of this property is generated ad hoc so the property value must be retrieved each and every time current data is required.
        /// </remarks>
        public HostInfo[] Hosts
        {
            get { return clients.Values.ToArray(); }
        }
        #endregion

        #region " Constructors "
        /// <summary>
        /// Creates a new instance of the <see cref="MessageServer" /> class with the default buffer size and character encoding listening on any local IP address and a random port.
        /// </summary>
        /// <param name="secure">
        /// Use a secure connection by the host.
        /// </param>
        /// <param name="Certificate">
        /// The X509 Certificate required by the secure connection.
        /// </param>
        /// <param name="backlog">
        /// The maximum number of clients that can be queued for TcpListener to accept.
        /// </param>
        public MessageServer(bool secure, X509Certificate Certificate = null, int backlog = 10)
        {
            Initialise(IPAddress.Any, 0, secure, Certificate, backlog);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessageServer" /> class with the default buffer size and character encoding listening on a random port.
        /// </summary>
        /// <param name="address">
        /// The IP address or host name on which to listen.
        /// </param>
        /// <param name="secure">
        /// Use a secure connection by the host.
        /// </param>
        /// <param name="Certificate">
        /// The X509 Certificate required by the secure connection.
        /// </param>
        /// <param name="backlog">
        /// The maximum number of clients that can be queued for TcpListener to accept.
        /// </param>
        public MessageServer(string address, bool secure, X509Certificate Certificate = null, int backlog = 10)
        {
            Initialise(ParseAddress(address), 0, secure, Certificate, backlog);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessageServer" /> class with the default buffer size and character encoding listening on a random port.
        /// </summary>
        /// <param name="address">
        /// The IP address on which to listen.
        /// </param>
        /// <param name="secure">
        /// Use a secure connection by the host.
        /// </param>
        /// <param name="Certificate">
        /// The X509 Certificate required by the secure connection.
        /// </param>
        /// <param name="backlog">
        /// The maximum number of clients that can be queued for TcpListener to accept.
        /// </param>
        public MessageServer(IPAddress address, bool secure, X509Certificate Certificate = null, int backlog = 10)
        {
            Initialise(address, 0, secure, Certificate, backlog);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessageServer" /> class with the default buffer size and character encoding listening on any local IP address.
        /// </summary>
        /// <param name="port">
        /// The port on which to listen for incoming connections.
        /// </param>
        /// <param name="secure">
        /// Use a secure connection by the host.
        /// </param>
        /// <param name="Certificate">
        /// The X509 Certificate required by the secure connection.
        /// </param>
        /// <param name="backlog">
        /// The maximum number of clients that can be queued for TcpListener to accept.
        /// </param>
        public MessageServer(int port, bool secure, X509Certificate Certificate = null, int backlog = 10)
        {
            Initialise(IPAddress.Any, port, secure, Certificate, backlog);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessageServer" /> class with the default character encoding listening on any local IP address.
        /// </summary>
        /// <param name="port">
        /// The port on which to listen for incoming connections.
        /// </param>
        /// <param name="bufferSize">
        /// The block size in which to read incoming messages.
        /// </param>
        /// <param name="secure">
        /// Use a secure connection by the host.
        /// </param>
        /// <param name="Certificate">
        /// The X509 Certificate required by the secure connection.
        /// </param>
        /// <param name="backlog">
        /// The maximum number of clients that can be queued for TcpListener to accept.
        /// </param>
        public MessageServer(int port, int bufferSize, bool secure, X509Certificate Certificate = null, int backlog = 10) : base(bufferSize)
        {
            Initialise(IPAddress.Any, port, secure, Certificate, backlog);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessageServer" /> class with the default buffer size and character encoding.
        /// </summary>
        /// <param name="address">
        /// The IP address or host name on which to listen.
        /// </param>
        /// <param name="port">
        /// The port on which to listen for incoming connections.
        /// </param>
        /// <param name="secure">
        /// Use a secure connection by the host.
        /// </param>
        /// <param name="Certificate">
        /// The X509 Certificate required by the secure connection.
        /// </param>
        /// <param name="backlog">
        /// The maximum number of clients that can be queued for TcpListener to accept.
        /// </param>
        public MessageServer(string address, int port, bool secure, X509Certificate Certificate = null, int backlog = 10)
        {
            Initialise(ParseAddress(address), port, secure, Certificate, backlog);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessageServer" /> class with the default buffer size and character encoding.
        /// </summary>
        /// <param name="address">
        /// The IP address on which to listen.
        /// </param>
        /// <param name="port">
        /// The port on which to listen for incoming connections.
        /// </param>
        /// <param name="secure">
        /// Use a secure connection by the host.
        /// </param>
        /// <param name="Certificate">
        /// The X509 Certificate required by the secure connection.
        /// </param>
        /// <param name="backlog">
        /// The maximum number of clients that can be queued for TcpListener to accept.
        /// </param>
        public MessageServer(IPAddress address, int port, bool secure, X509Certificate Certificate = null, int backlog = 10)
        {
            Initialise(address, port, secure, Certificate, backlog);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessageServer" /> class with the default character encoding.
        /// </summary>
        /// <param name="address">
        /// The IP address or host name on which to listen.
        /// </param>
        /// <param name="port">
        /// The port on which to listen for incoming connections.
        /// </param>
        /// <param name="bufferSize">
        /// The block size in which to read incoming messages.
        /// </param>
        /// <param name="secure">
        /// Use a secure connection by the host.
        /// </param>
        /// <param name="Certificate">
        /// The X509 Certificate required by the secure connection.
        /// </param>
        /// <param name="backlog">
        /// The maximum number of clients that can be queued for TcpListener to accept.
        /// </param>
        public MessageServer(string address, int port, int bufferSize, bool secure, X509Certificate Certificate = null, int backlog = 10) : base(bufferSize)
        {
            Initialise(ParseAddress(address), port, secure, Certificate, backlog);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessageServer" /> class with the default character encoding.
        /// </summary>
        /// <param name="address">
        /// The IP address on which to listen.
        /// </param>
        /// <param name="port">
        /// The port on which to listen for incoming connections.
        /// </param>
        /// <param name="bufferSize">
        /// The block size in which to read incoming messages.
        /// </param>
        /// <param name="secure">
        /// Use a secure connection by the host.
        /// </param>
        /// <param name="Certificate">
        /// The X509 Certificate required by the secure connection.
        /// </param>
        /// <param name="backlog">
        /// The maximum number of clients that can be queued for TcpListener to accept.
        /// </param>
        public MessageServer(IPAddress address, int port, int bufferSize, bool secure, X509Certificate Certificate = null, int backlog = 10) : base(bufferSize)
        {
            Initialise(address, port, secure, Certificate, backlog);
        }
        #endregion

        #region " Methods "

        #region " Public Methods "
        /// <summary>
        /// Sends binary data to the client.
        /// </summary>
        /// <param name="hostName">
        /// The name or address of the client to send the data to.
        /// </param>
        /// <param name="port">
        /// The port number of the client to send the data to.
        /// </param>
        /// <param name="data">
        /// A <b>byte</b> containing the binary data to send.
        /// </param>
        public void SendData(string hostName, int port, byte[] data)
        {
            try
            {
                TcpClient client = clients.Select(c => new { client = c, host = c.Value }).Where(x => x.host.HostName == hostName && x.host.Port == port).Select(x => x.client.Key).First();
                Send(client, data, clients[client].SecureStream).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                // Don't know what the hell happened, lets log the exception.
                // It was probably due to some idiot trying to send data to a client that has disconnected
                OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.ToString()));
            }
        }

        /// <summary>
        /// Sends binary data to the specified client.
        /// </summary>
        /// <param name="host">
        /// The remote client to send the data to.
        /// </param>
        /// <param name="data">
        /// A <b>byte</b> containing the binary data to send.
        /// </param>
        public void SendData(HostInfo host, byte[] data)
        {
            try
            {
                TcpClient client = clients.Select(c => new { client = c, host = c.Value }).Where(x => x.host.Equals(host)).Select(x => x.client.Key).First();
                Send(client, data, host.SecureStream).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                // Don't know what the hell happened, lets log the exception.
                // It was probably due to some idiot trying to send data to a client that has disconnected
                OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.ToString()));
            }
        }

        /// <summary>
        /// Sends binary data to all connected clients.
        /// </summary>
        /// <param name="data">
        /// A <b>byte</b> containing the binary data to send.
        /// </param>
        public void SendData(byte[] data)
        {
            try
            {
                foreach (TcpClient client in clients.Keys)
                {
                    Send(client, data, clients[client].SecureStream).ConfigureAwait(false);
                }
            }
            catch (Exception Ex)
            {
                // Don't know what the hell happened, lets log the exception.
                // It was probably due to some idiot trying to send data to a client that has disconnected
                OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.ToString()));
            }
        }

        /// <summary>
        /// Updates certifcate used when accepting new connections.
        /// If secure is disabled this function has no effect.
        /// </summary>
        /// <param name="Certificate">
        /// The new certificate to use by the host.
        /// </param>
        public void UpdateCertificate(X509Certificate Certificate)
        {
            // Only update if secure connections enabled
            if (_secure)
            {
                // Assign certificate for secure connections
                servercertificate = Certificate;
            }
        }
        #endregion

        #region " Protected Methods "
        /// <summary>
        /// Releases all resources used by the object.
        /// </summary>
        /// <param name="disposing">
        /// <b>true</b> to release both managed and unmanaged resources; <b>false</b> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // Stop listening for connection requests.
                    server.Stop();

                    lock (clientslock)
                    {
                        // Close all remaining connections
                        for (int i = 0; i < clients.Count; i++)
                        {
                            try
                            {
                                clients.Keys.ElementAt(i).GetStream().Close();
                                clients.Keys.ElementAt(i).Close();
                            }
                            catch (Exception)
                            {
                                // Ignore, client has already been disposed of.
                            }
                        }
                        clients.Clear();
                    }
                }
            }

            base.Dispose(disposing);
            isDisposed = true;
        }
        #endregion

        #region " Private Methods "
        /// <summary>
        /// Accepts incoming connection requests asynchronously.
        /// </summary>
        private async void AcceptTcpClient()
        {
            TcpClient client = default(TcpClient);
            SslStream securestream = default(SslStream);
            NetworkStream stream = default(NetworkStream);
            do
            {
                try
                {
                    client = await server.AcceptTcpClientAsync().ConfigureAwait(false);
                    try
                    {
                        // Configure the client to not delay send and receives.
                        client.NoDelay = true;
                        // Get the client stream.
                        stream = client.GetStream();
                        // Create a secure stream.
                        securestream = null;
                        // Create the buffer.
                        byte[] buffer = new byte[BufferSize];

                        // Determine if a secure connection was requested.
                        if (Secure == false)
                        {
                            // Remember the client and its host information.
                            AddClient(client);
                            // Listen asynchronously for incoming messages from this client.
                            Read(buffer, client, stream);
                        }
                        else if (Secure == true)
                        {
                            // Create a secure stream and authenticate the server certificate.
                            securestream = new SslStream(client.GetStream(), false);
                            securestream.AuthenticateAsServer(servercertificate, false, SslProtocols.Tls12, true);
                            // Remember the client and its host information.
                            AddClient(client, securestream);
                            // Listen asynchronously for incoming messages from this client.
                            SecureRead(buffer, client, securestream);
                        }
                    }
                    catch (IOException Ex)
                    {
                        // If the connecton drops out or something, close the connection.
                        OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.ToString()));
                        client.GetStream().Close();
                        client.Close();
                    }
                    catch (AuthenticationException Ex)
                    {
                        // If the connection doesn't have an SSL stream or wrong SSL Protocol etc., close the connection.
                        OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.ToString()));
                        client.GetStream().Close();
                        client.Close();
                    }
                    catch (Exception Ex)
                    {
                        // Don't know what the hell happened, lets close the connection and log the exception.
                        client.GetStream().Close();
                        client.Close();
                        OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.ToString()));
                    }
                }
                catch (ObjectDisposedException)
                {
                    // We are not listening anymore, so lets exit the loop.
                    break;
                }
                catch (Exception Ex)
                {
                    // Don't know what the hell happened, lets log the exception.
                    OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.ToString()));
                }
            } while (true);
        }

        /// <summary>
        /// Creates the server and starts listening for incoming connection requests.
        /// </summary>
        /// <param name="ipAddress">
        /// The IP address or host name on which to listen.
        /// </param>
        /// <param name="port">
        /// The port to listen on.
        /// </param>
        /// <param name="secure">
        /// Use a secure connection by the host.
        /// </param>
        /// <param name="Certificate">
        /// The certificate to use by the host.
        /// </param>
        /// <param name="backlog">
        /// The maximum number of clients that can be queued for TcpListener to accept.
        /// </param>
        private void Initialise(IPAddress ipAddress, int port, bool secure, X509Certificate Certificate, int backlog)
        {
            // Set backlog variable
            listenerbacklog = backlog;
            // Listen on the first IPv4 address assigned to the local machine.
            server = new TcpListener(ipAddress, port);
            server.Start(listenerbacklog);

            // Get the port number from the server in case a random port was used.
            _port = ((IPEndPoint)server.LocalEndpoint).Port;

            // Set secure property so I know if I should use secure connections for clients
            _secure = secure;

            // Assign certificate for secure connections
            servercertificate = Certificate;

            // Start listen asynchronously.
            AcceptTcpClient();
        }

        /// <summary>
        /// Converts an address string to an <see cref="IPAddress" /> instance.
        /// </summary>
        /// <param name="address">
        /// The address to parse.
        /// </param>
        /// <returns>
        /// An <b>IPAddress</b> corresponding to the specified address.
        /// </returns>
        /// <remarks>
        /// If <i>address</i> is null or empty or is equal to the machine name then <see cref="IPAddress.Any">Any</see> is returned.
        /// If <i>address</i> is equal to "localhost" then <see cref="IPAddress.Loopback">Loopback</see> is returned.
        /// In each case comparisons are case-insensitive.
        /// </remarks>
        private IPAddress ParseAddress(string address)
        {
            IPAddress result = default(IPAddress);

            if (string.IsNullOrEmpty(address) || address.Equals(Environment.MachineName, StringComparison.CurrentCultureIgnoreCase))
            {
                result = IPAddress.Any;
            }
            else if (address.Equals("localhost", StringComparison.CurrentCultureIgnoreCase))
            {
                result = IPAddress.Loopback;
            }
            else
            {
                result = IPAddress.Parse(address);
            }
            return result;
        }

        /// <summary>
        /// Reads incoming messages.
        /// </summary>
        /// Param <param name="buffer">
        /// Buffer to use for receiving data.
        /// </param>
        /// Param <param name="client">
        /// The TcpClient used for the connection.
        /// </param>
        /// Param <param name="stream">
        /// The network stream to use for receiving data.
        /// </param>
        private async void Read(byte[] buffer, TcpClient client, NetworkStream stream)
        {
            int byteCount = 0;
            List<byte> Message = new List<byte>();

            do
            {
                try
                {
                    // Reset values.
                    byteCount = 0;
                    Message.Clear();
                    // Asynchronously read the data.
                    byteCount = await stream.ReadAsync(buffer, 0, buffer.Count()).ConfigureAwait(false);

                    // Check to see if the client has disconnected.
                    if (byteCount == 0)
                    {
                        // If there is no data when an asynchronous read completes it is because the client closed the connection.
                        RemoveClient(client);
                        // Client is not there any more, so lets exit the loop.
                        break;
                    }

                    Message.AddRange(buffer.Take(byteCount));
                    // Notify any listeners that a message was received.
                    OnMessageReceived(new MessageReceivedEventArgs(clients[client], Message.ToArray()));
                }
                catch (InvalidOperationException)
                {
                    // The callback specified when BeginRead was called gets invoked one last time when the TcpListener is stopped.
                    // This exception is thrown when GetStream is called on a disconnected client or EndRead is called on a disposed stream.
                }
                catch (IOException)
                {
                    // Ignore these, connection has gone nuts.
                }
                catch (Exception Ex)
                {
                    // Don't know what the hell happened, lets log the exception.
                    OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.ToString()));
                }

            } while (true);
        }

        /// <summary>
        /// Reads incoming messages securely.
        /// </summary>
        /// Param <param name="buffer">
        /// Buffer to use for receiving data.
        /// </param>
        /// Param <param name="client">
        /// The TcpClient used for the connection.
        /// </param>
        /// Param <param name="securestream">
        /// The SSL stream to use for receiving data.
        /// </param>
        private async void SecureRead(byte[] buffer, TcpClient client, SslStream securestream)
        {
            int byteCount = 0;
            List<byte> Message = new List<byte>();

            do
            {
                try
                {
                    // Reset values.
                    byteCount = 0;
                    Message.Clear();
                    // Asynchronously read the data.
                    byteCount = await securestream.ReadAsync(buffer, 0, buffer.Count()).ConfigureAwait(false);

                    // Check to see if the client has disconnected.
                    if (byteCount == 0)
                    {
                        // If there is no data when an asynchronous read completes it is because the client closed the connection.
                        RemoveClient(client);
                        // Client is not there any more, so lets exit the loop.
                        break;
                    }

                    Message.AddRange(buffer.Take(byteCount));
                    // Notify any listeners that a message was received.
                    OnMessageReceived(new MessageReceivedEventArgs(clients[client], Message.ToArray()));
                }
                catch (InvalidOperationException)
                {
                    // The callback specified when BeginRead was called gets invoked one last time when the TcpListener is stopped.
                    // This exception is thrown when GetStream is called on a disconnected client or EndRead is called on a disposed stream.
                }
                catch (IOException)
                {
                    // Ignore these, connection has gone nuts.
                }
                catch (Exception Ex)
                {
                    // Don't know what the hell happened, lets log the exception.
                    OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.ToString()));
                }
            } while (true);
        }

        /// <summary>
        /// Adds a client to the list of connected clients.
        /// </summary>
        /// <param name="client">
        /// The client to Add.
        /// </param>
        /// <param name="securestream">
        /// Optional Secure Stream object.
        /// </param>
        private void AddClient(TcpClient client, SslStream securestream = null)
        {
            HostInfo host = default(HostInfo);

            lock (clientslock)
            {
                if (!clients.ContainsKey(client))
                {
                    try
                    {
                        // Create a new host record
                        IPEndPoint HostAddress = (IPEndPoint)client.Client.RemoteEndPoint;
                        host = new HostInfo(HostAddress.Address.ToString(), HostAddress.Port, Secure, securestream);
                        // Remember the client and its host information.
                        clients.Add(client, host);
                        // Notify any listeners that a connection has been made.
                        OnConnectionAccepted(new ConnectionEventArgs(host));
                    }
                    catch (Exception)
                    {
                        // Ignore, client may have transient connectivity issues.
                    }
                }
            }
        }

        /// <summary>
        /// Removes a client from the list of connected clients.
        /// </summary>
        /// <param name="client">
        /// The client to remove.
        /// </param>
        private void RemoveClient(TcpClient client)
        {
            HostInfo host = default(HostInfo);

            lock (clientslock)
            {
                if (clients.ContainsKey(client))
                {
                    try
                    {
                        host = clients[client];
                        clients.Remove(client);
                        client.GetStream().Close();
                        client.Close();
                    }
                    catch (Exception)
                    {
                        // Ignore, client has already been disposed of.
                    }
                    // Notify any listeners that the host has disconnected.
                    OnConnectionClosed(new ConnectionEventArgs(host));
                }
            }
        }

        /// <summary>
        /// Sends binary data to a client.
        /// </summary>
        /// <param name="client">
        /// The client to send the message to.
        /// </param>
        /// <param name="data">
        /// The message to send.
        /// </param>
        /// <param name="securestream">
        /// The SSL stream to use for sending data.
        /// </param>
        private async Task Send(TcpClient client, byte[] data, SslStream securestream = null)
        {
            await clients[client].WriteLock.WaitAsync().ConfigureAwait(false);
            try
            {
                NetworkStream stream = client.GetStream();
                // Send the message asynchronously.
                if (_secure == false)
                {
                    await stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
                    // Await stream.FlushAsync.ConfigureAwait(False)
                }
                else if (_secure == true)
                {
                    await securestream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
                    // Await securestream.FlushAsync.ConfigureAwait(False)
                }
            }
            catch (IOException)
            {
                // Ignore these, connection has gone nuts.
            }
            catch (Exception Ex)
            {
                // Don't know what the hell happened, lets log the exception.
                OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.ToString()));
            }
            clients[client].WriteLock.Release();
        }
        #endregion

        #endregion
    }
}