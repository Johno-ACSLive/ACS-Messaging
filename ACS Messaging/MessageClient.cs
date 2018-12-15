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
    /// <summary>
    /// Initiates a conection with a server and sends ands receives text messages over that connection.
    /// </summary>
    public class MessageClient : MessageClientServerBase
    {
        #region " Fields "
        /// <summary>
        /// The client used to communicate with the server
        /// </summary>
        private readonly TcpClient client = new TcpClient();

        /// <summary>
        /// Indicates whether or not the current instance has been disposed.
        /// </summary>
        private bool isDisposed = false;

        /// <summary>
        /// The port number of the local end of the connection.
        /// </summary>
        private int _localPort;

        /// <summary>
        /// The details of the server to connect to.
        /// </summary>
        private HostInfo server;

        /// <summary>
        /// The stream over which messages are sent and received.
        /// </summary>
        private NetworkStream stream;

        /// <summary>
        /// The secure stream over which messages are sent and received.
        /// </summary>
        private SslStream securestream;
        #endregion

        #region " Properties "
        /// <summary>
        /// Gets the port number of the local end of the connection.
        /// </summary>
        /// <value>
        /// An <b>int</b> containing a port number.
        /// </value>
        public int LocalPort
        {
            get { return _localPort; }
        }
        #endregion

        #region " Constructors "
        /// <summary>
        /// Creates a new instance of the <see cref="MessageClient" /> class with the default buffer size and character encoding.
        /// </summary>
        /// <param name="hostName">
        /// The name or address of the server to connect to.
        /// </param>
        /// <param name="remotePort">
        /// The port number to connect to.
        /// </param>
        /// <param name="secure">
        /// Use a secure connection to the remote host.
        /// </param>
        public MessageClient(string hostName, int remotePort, bool secure)
        {
            Initialise(hostName, remotePort, secure);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessageClient" /> class with the default character encoding.
        /// </summary>
        /// <param name="hostName">
        /// The name or address of the server to connect to.
        /// </param>
        /// <param name="remotePort">
        /// The port number to connect to.
        /// </param>
        /// <param name="secure">
        /// Use a secure connection to the remote host.
        /// </param>
        /// <param name="bufferSize">
        /// The block size in which to read incoming messages.
        /// </param>
        public MessageClient(string hostName, int remotePort, bool secure, int bufferSize) : base(bufferSize)
        {
            Initialise(hostName, remotePort, secure);
        }
        #endregion

        #region " Events "
        /// <summary>
        /// Occurs when a connection attempt fails.
        /// </summary>
        public event ConnectionEventHandler ConnectionFailed;
        #endregion

        #region " Methods "

        #region " Public Methods "
        /// <summary>
        /// Connects to the server.
        /// </summary>
        public async void Connect()
        {
            // Connect asynchronously to the server.
            await ConnectToServer().ConfigureAwait(false);
        }

        /// <summary>
        /// Sends binary data to the server.
        /// </summary>
        /// <param name="data">
        /// The message to send.
        /// </param>
        public async void SendData(byte[] data)
        {
            await Send(data).ConfigureAwait(false);
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
                    // Close the connection and the underlying stream.
                    if (securestream != null)
                    {
                        securestream.Close();
                    }
                    if (stream != null)
                    {
                        stream.Close();
                    }
                    client.Close();
                }
                securestream = null;
                stream = null;
            }

            base.Dispose(disposing);
            isDisposed = true;
        }

        /// <summary>
        /// Raises the <see cref="ConnectionFailed" /> event.
        /// </summary>
        /// <param name="e">
        /// Contains the data for the event.
        /// </param>
        /// <remarks>
        /// The event will be raised on the thread on which the current instance was created.
        /// </remarks>
        protected virtual void OnConnectionFailed(ConnectionEventArgs e)
        {
            RaiseConnectionFailed(e);
        }
        #endregion

        #region " Private Methods "
        /// <summary>
        /// Connects asynchronously to the server.
        /// </summary>
        private async Task ConnectToServer()
        {
            try
            {
                // Connect asynchronously to the server.
                await client.ConnectAsync(server.HostName, server.Port).ConfigureAwait(false);
                // Get the port that was assigned to the local end point.
                _localPort = ((IPEndPoint)client.Client.LocalEndPoint).Port;
                // Configure the client to not delay send and receives.
                client.NoDelay = true;
                // Get the client stream
                stream = client.GetStream();
                // Create the buffer
                byte[] buffer = new byte[BufferSize];
                if (server.Secure == false)
                {
                    // Listen asynchronously for incoming messages from this server.
                    Read(buffer);
                }
                else if (server.Secure == true)
                {
                    // Create a secure stream and validate the server certificate.
                    securestream = new SslStream(stream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                    securestream.AuthenticateAsClient(server.HostName, null, SslProtocols.Tls12, false);
                    // MsgBox(securestream.CipherAlgorithm.ToString & " " & securestream.CipherStrength & " " & securestream.IsAuthenticated & " " & securestream.IsEncrypted & " " & securestream.IsSigned)
                    // Listen asynchronously for incoming messages from this server.
                    SecureRead(buffer);
                }
                // Notify any listeners that the connection was successful.
                OnConnectionAccepted(new ConnectionEventArgs(server));
            }
            catch (SocketException)
            {
                // The specified server was not found.
                OnConnectionFailed(new ConnectionEventArgs(server));
            }
            catch (Exception Ex)
            {
                OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.ToString()));
            }
        }

        /// <summary>
        /// Validates the certificate used by the server.
        /// </summary>
        /// <param name="sender">
        /// Contains the object for validation.
        /// </param>
        /// <param name="certificate">
        /// X509 certificate to validate.
        /// </param>
        /// <param name="chain">
        /// Certificate chain to validate.
        /// </param>
        /// <param name="sslPolicyErrors">
        /// Policy errors in SSL validation.
        /// </param>
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a <see cref="HostInfo" /> to represent the server.
        /// </summary>
        /// <param name="hostName">
        /// The name or address of the server.
        /// </param>
        /// <param name="remotePort">
        /// The port number of the server.
        /// </param>
        /// <param name="secure">
        /// Use a secure connection to the remote host.
        /// </param>
        private void Initialise(string hostName, int remotePort, bool secure)
        {
            server = new HostInfo(hostName, remotePort, secure);
        }

        /// <summary>
        /// Raises the <see cref="ConnectionFailed" /> event on the current thread.
        /// </summary>
        /// <param name="e">
        /// Contains the data for the event.
        /// </param>
        private void RaiseConnectionFailed(object e)
        {
            ConnectionFailed?.Invoke(this, (ConnectionEventArgs)e);
        }

        /// <summary>
        /// Receives incoming data.
        /// </summary>
        /// <param name="buffer">
        /// Buffer to use for receiving data.
        /// </param>
        private async void Read(byte[] buffer)
        {
            // The stream will be Nothing if the client has been disposed.

            int byteCount = 0;
            List<byte> Message = new List<byte>();

            do
            {
                if (stream != null)
                {
                    try
                    {
                        // Reset values.
                        byteCount = 0;
                        Message.Clear();
                        // Asynchronously read the data.
                        byteCount = await stream.ReadAsync(buffer, 0, buffer.Count()).ConfigureAwait(false);
                        Message.AddRange(buffer.Take(byteCount));
                        // Notify any listeners that data was received.
                        OnMessageReceived(new MessageReceivedEventArgs(server, Message.ToArray()));
                    }
                    catch (InvalidOperationException)
                    {
                        // The callback specified when BeginRead was called gets invoked one last time when the TcpListener is stopped.
                        // This exception is thrown when GetStream is called on a disconnected client or EndRead is called on a disposed stream.
                    }
                    catch (IOException)
                    {
                        // Ignore these as they can occurr if a non-secure connection is attempted while listener is expecting a secure connection or
                        // if the client drops out during connection.
                    }
                    catch (Exception Ex)
                    {
                        OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.ToString()));
                    }

                    // Check to see if the server has disconnected.
                    if (byteCount == 0)
                    {
                        // If there is no data when an asynchronous read completes it is because the server closed the connection.
                        OnConnectionClosed(new ConnectionEventArgs(server));
                        // Client is not there any more, so lets exit the loop.
                        break;
                    }
                }
            } while (true);
        }

        /// <summary>
        /// Receives secure incoming data.
        /// </summary>
        /// <param name="buffer">
        /// Buffer to use for receiving data.
        /// </param>
        private async void SecureRead(byte[] buffer)
        {
            // The stream will be Nothing if the client has been disposed.

            int byteCount = 0;
            List<byte> Message = new List<byte>();

            do
            {
                if (stream != null)
                {
                    try
                    {
                        // Reset values.
                        byteCount = 0;
                        Message.Clear();
                        // Asynchronously read the data.
                        byteCount = await securestream.ReadAsync(buffer, 0, buffer.Count()).ConfigureAwait(false);
                        Message.AddRange(buffer.Take(byteCount));
                        // Notify any listeners that data was received.
                        OnMessageReceived(new MessageReceivedEventArgs(server, Message.ToArray()));
                    }
                    catch (InvalidOperationException)
                    {
                        // The callback specified when BeginRead was called gets invoked one last time when the TcpListener is stopped.
                        // This exception is thrown when GetStream is called on a disconnected client or EndRead is called on a disposed stream.
                    }
                    catch (IOException)
                    {
                        // Ignore these as they can occurr if a non-secure connection is attempted while listener is expecting a secure connection or
                        // if the client drops out during connection.
                    }
                    catch (Exception Ex)
                    {
                        OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.ToString()));
                    }

                    // Check to see if the server has disconnected.
                    if (byteCount == 0)
                    {
                        // If there is no data when an asynchronous read completes it is because the server closed the connection.
                        OnConnectionClosed(new ConnectionEventArgs(server));
                        // Client is not there any more, so lets exit the loop.
                        break;
                    }
                }
            } while (true);
        }

        /// <summary>
        /// Sends binary data to the server.
        /// </summary>
        /// <param name="data">
        /// The message to send.
        /// </param>
        private async Task Send(byte[] data)
        {
            await server.WriteLock.WaitAsync().ConfigureAwait(false);
            try
            {
                NetworkStream stream = client.GetStream();
                // Send the message asynchronously.
                if (server.Secure == false)
                {
                    await stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
                    // await stream.FlushAsync.ConfigureAwait(false);
                }
                else if (server.Secure == true)
                {
                    await securestream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
                    // await securestream.FlushAsync.ConfigureAwait(false);
                }
            }
            catch (IOException)
            {
                // Ignore these as they can occurr if a non-secure connection is attempted while listener is expecting a secure connection or
                // if the client drops out during connection.
            }
            catch (Exception Ex)
            {

                // Do nothing
                OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.ToString()));
            }
            server.WriteLock.Release();
        }
        #endregion

        #endregion
    }
}