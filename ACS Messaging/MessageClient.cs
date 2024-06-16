using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using static ACS.Messaging.MessageServer;

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
        public int LocalPort => _localPort;

        /// <summary>
        /// Gets or Sets the Challenge for further validation.
        /// </summary>
        /// <value>
        /// A <b>String</b> containing challenge response to be validated.
        /// </value>
        public string Challenge { get; set; }

        /// <summary>
        /// Gets or Sets the flag for enabling or disabling the Challenge validation response.
        /// </summary>
        /// <value>
        /// A <b>Boolean</b> indicating if Challenge responses are required.
        /// </value>
        public bool IsChallengeEnabled { get; set; } // Note: this is not ideal and we will have better integration of Challenge in future.
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

                bool isaccesscontrolpassed = ProcessAccessControl(client);

                if (isaccesscontrolpassed is false)
                {
                    client.GetStream().Close();
                    client.Close();
                    return;
                }

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
        /// Checks is Access Control is required to be processed.
        /// </summary>
        private bool ProcessAccessControl(TcpClient client, bool SkipChallenge = false)
        {
            // This is not ideal as we don't have good integration.
            // We need to re-architect and have full integration via message framing.
            // Raw connections will not support Challenge in future (application logic will need to handle an equivalent if required).
            bool isaccesscontrolpassed = false;

            if (IsChallengeEnabled is true)
            {
                ChallengeRequest request;
                ChallengeResponse response = new ChallengeResponse();
                MemoryStream ms;
                byte[] buffer = new byte[1024 + 2];
                int bytecount = 0;
                ushort size = 0;
                List<byte> data = new List<byte>();

                // Backup the current timeout value
                int receivetimeout = client.ReceiveTimeout;
                // 30 seconds should be enough time for a client connection to respond to the challenge request
                client.ReceiveTimeout = 30000;

                try
                {
                    if (server.Secure is false)
                    {
                        bytecount = stream.ReadAsync(buffer, 0, buffer.Count()).Result;
                    }
                    else if (server.Secure is true)
                    {
                        securestream = new SslStream(stream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                        securestream.AuthenticateAsClient(server.HostName, null, SslProtocols.Tls12, false);
                        bytecount = securestream.ReadAsync(buffer, 0, buffer.Count()).Result;
                    }

                    if (bytecount == 0) { return isaccesscontrolpassed; }
                    size = BitConverter.ToUInt16(buffer, 0);
                    if (bytecount != size) { return isaccesscontrolpassed; }
                    ms = new MemoryStream(buffer, 2, size - 2);
                    request = (ChallengeRequest)Json.DeserializeAsync(ms).Result;
                }
                catch (Exception Ex)
                {
                    OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.Message));
                    return isaccesscontrolpassed;
                }
                
                try
                {
                    response.ID = request.ID;
                    response.Challenge = Challenge;

                    ms = new MemoryStream();
                    Json.SerializeAsync(ms, request).Wait();

                    if (ms.Length > 1024)
                    {
                        OnLog(new LogEventArgs(DateTime.Now, "ERROR", "Serialised Challenge Response exceeds 1024 bytes."));
                        return isaccesscontrolpassed;
                    };

                    size = (ushort)(2 + ms.Length);
                    data.AddRange(BitConverter.GetBytes(size));
                    data.AddRange(ms.GetBuffer());
                    buffer = data.ToArray();

                    if (server.Secure is false)
                    {
                        stream.WriteAsync(buffer, 0, buffer.Length).Wait();
                        stream.FlushAsync().Wait();
                        bytecount = stream.ReadAsync(buffer, 0, buffer.Count()).Result;
                    }
                    else if (server.Secure is true)
                    {
                        securestream.WriteAsync(buffer, 0, buffer.Length).Wait();
                        securestream.FlushAsync().Wait();
                        bytecount = securestream.ReadAsync(buffer, 0, buffer.Count()).Result;
                    }

                    // Restore the timeout value
                    client.ReceiveTimeout = receivetimeout;
                    if (bytecount == 0) { return isaccesscontrolpassed; }
                    size = BitConverter.ToUInt16(buffer, 0);
                    if (bytecount != size) { return isaccesscontrolpassed; }
                    ms = new MemoryStream(buffer, 2, size - 2);
                    request = (ChallengeRequest)Json.DeserializeAsync(ms).Result;
                }
                catch (Exception Ex)
                {
                    OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.Message));
                    return isaccesscontrolpassed;
                }
            }

            isaccesscontrolpassed = true;
            return isaccesscontrolpassed;
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
                OnLog(new LogEventArgs(DateTime.Now, "ERROR", Ex.ToString()));
            }
            server.WriteLock.Release();
        }
        #endregion

        #endregion
    }
}