using System;
using ACS.Messaging;

/// <summary>
/// Acts as a base class for both server and client types who can send text messages to each other.
/// </summary>
public abstract class MessageClientServerBase : IDisposable
{
    #region " Fields "
    /// <summary>
    /// The block size in which to read incoming messages.
    /// </summary>
    private readonly int _bufferSize = 8192;

    /// <summary>
    /// Indicates whether or not the current instance has been disposed.
    /// </summary>
    private bool isDisposed = false;

    /// <summary>
    /// Indicates the current connection status.
    /// </summary>
    private bool isconnected = false;
    #endregion

    #region " Properties "
    /// <summary>
    /// Gets the size of the blocks in which incoming messages are read.
    /// </summary>
    /// <value>
    /// An <b>int</b> containing the read block size. The default is 1024 (1 KB).
    /// </value>
    protected int BufferSize => _bufferSize;

    /// <summary>
    /// Gets the current connection status.
    /// </summary>
    /// <value>
    /// A <b>bool</b> indicating connection status.
    /// </value>
    public bool IsConnected => isconnected;
    #endregion

    #region " Constructors "
    /// <summary>
    /// Creates a new instance of the <see cref="MessageClientServerBase" /> class with the default buffer size and character encoding.
    /// </summary>
    public MessageClientServerBase()
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MessageClientServerBase" /> class with the default character encoding.
    /// </summary>
    /// <param name="bufferSize">
    /// The block size in which to read incoming messages.
    /// </param>
    public MessageClientServerBase(int bufferSize)
    {
        _bufferSize = bufferSize;
    }
    #endregion

    #region " Events "
    /// <summary>
    /// Occurs when a connection attempt is accepted.
    /// </summary>
    public event ConnectionEventHandler ConnectionAccepted;

    /// <summary>
    /// Occurs when a connection is closed.
    /// </summary>
    public event ConnectionEventHandler ConnectionClosed;

    /// <summary>
    /// Occurs when a connection attempt fails.
    /// </summary>
    public event ConnectionEventHandler ConnectionFailed;

    /// <summary>
    /// Occurs when a message is received.
    /// </summary>
    public event MessageReceivedEventHandler MessageReceived;

    /// <summary>
    /// Occurs when an error is logged.
    /// </summary>
    public event LogEventHandler Log;
    #endregion

    #region " Methods "

    #region " Public Methods "
    /// <summary>
    /// Releases all resources used by the object.
    /// </summary>
    /// <remarks>
    /// This method should be called when the object is no longer needed.
    /// </remarks>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion

    #region " Protected Methods "
    /// <summary>
    /// Releases all resources used by the object.
    /// </summary>
    /// <param name="disposing">
    /// <b>true</b> to release both managed and unmanaged resources; <b>false</b> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
            }
        }
        isDisposed = true;
        isconnected = false;
    }

    /// <summary>
    /// Raises the <see cref="ConnectionAccepted" /> event.
    /// </summary>
    /// <param name="e">
    /// Contains the data for the event.
    /// </param>
    /// <remarks>
    /// The event will be raised on the thread on which the current instance was created.
    /// </remarks>
    protected virtual void OnConnectionAccepted(ConnectionEventArgs e)
    {
        isconnected = true;
        RaiseConnectionAccepted(e);
    }

    /// <summary>
    /// Raises the <see cref="ConnectionClosed" /> event.
    /// </summary>
    /// <param name="e">
    /// Contains the data for the event.
    /// </param>
    /// <remarks>
    /// The event will be raised on the thread on which the current instance was created.
    /// </remarks>
    protected virtual void OnConnectionClosed(ConnectionEventArgs e)
    {
        isconnected = false;
        RaiseConnectionClosed(e);
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
        isconnected = false;
        RaiseConnectionFailed(e);
    }

    /// <summary>
    /// Raises the <see cref="MessageReceived" /> event.
    /// </summary>
    /// <param name="e">
    /// Contains the data for the event.
    /// </param>
    /// <remarks>
    /// The event will be raised on the thread on which the current instance was created.
    /// </remarks>
    protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
    {
        RaiseMessageReceived(e);
    }

    /// <summary>
    /// Raises the <see cref="Log" /> event.
    /// </summary>
    /// <param name="e">
    /// Contains the data for the event.
    /// </param>
    /// <remarks>
    /// The event will be raised on the thread on which the current instance was created.
    /// </remarks>
    protected virtual void OnLog(LogEventArgs e)
    {
        RaiseLog(e);
    }
    #endregion

    #region " Private Methods "
    /// <summary>
    /// Raises the <see cref="ConnectionAccepted" /> event on the current thread.
    /// </summary>
    /// <param name="e">
    /// Contains the data for the event.
    /// </param>
    private void RaiseConnectionAccepted(object e)
    {
        if (ConnectionAccepted != null)
        {
            ConnectionAccepted(this, (ConnectionEventArgs)e);
        }
    }

    /// <summary>
    /// Raises the <see cref="ConnectionClosed" /> event on the current thread.
    /// </summary>
    /// <param name="e">
    /// Contains the data for the event.
    /// </param>
    private void RaiseConnectionClosed(object e)
    {
        if (ConnectionClosed != null)
        {
            ConnectionClosed(this, (ConnectionEventArgs)e);
        }
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
    /// Raises the <see cref="MessageReceived" /> event on the current thread.
    /// </summary>
    /// <param name="e">
    /// Contains the data for the event.
    /// </param>
    private void RaiseMessageReceived(object e)
    {
        if (MessageReceived != null)
        {
            MessageReceived(this, (MessageReceivedEventArgs)e);
        }
    }

    /// <summary>
    /// Raises the <see cref="Log" /> event on the current thread.
    /// </summary>
    /// <param name="e">
    /// Contains the exception for the event.
    /// </param>
    private void RaiseLog(object e)
    {
        if (Log != null)
        {
            Log(this, (LogEventArgs)e);
        }
    }
    #endregion

    #endregion
}