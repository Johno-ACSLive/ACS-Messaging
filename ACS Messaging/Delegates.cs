using ACS.Messaging;
        
/// <summary>
    /// Represents the method that will handle the <see cref="MessageClientServerBase.ConnectionAccepted">ConnectionAccepted</see> and
    /// <see cref="MessageClientServerBase.ConnectionClosed">ConnectionClosed</see> events of a <see cref="MessageClientServerBase" />.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// A <see cref="ConnectionEventArgs" /> that contains the event data.
    /// </param>
    public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs e);

    /// <summary>
    /// Represents the method that will handle the <see cref="MessageClientServerBase.MessageReceived">MessageReceived</see> event of a <see cref="MessageClientServerBase" />.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// A <see cref="MessageReceivedEventArgs" /> that contains the event data.
    /// </param>
    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    /// <summary>
    /// Represents the method that will handle the <see cref="MessageClientServerBase.Log">Log</see> event of a <see cref="MessageClientServerBase" />.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// A <see cref="LogEventArgs" /> that contains the event data.
    /// </param>
    public delegate void LogEventHandler(object sender, LogEventArgs e);