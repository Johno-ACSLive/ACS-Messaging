/// <summary>
/// Contains data for the <see cref="MessageClientServerBase.MessageReceived">MessageReceived</see> event.
/// </summary>
public class MessageReceivedEventArgs : ConnectionEventArgs
{
    /// <summary>
    /// The data that was received.
    /// </summary>
    private readonly byte[] _data;

    /// <summary>
    /// Gets the data that was received.
    /// </summary>
    /// <value>
    /// A <b>byte</b> containing binary data.
    /// </value>
    public byte[] Data
    {
        get { return _data; }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MessageReceivedEventArgs" /> class.
    /// </summary>
    /// <param name="host">
    /// The remote host.
    /// </param>
    /// <param name="data">
    /// The data that was received.
    /// </param>
    public MessageReceivedEventArgs(HostInfo host, byte[] data) : base(host)
    {
        _data = data;
    }
}