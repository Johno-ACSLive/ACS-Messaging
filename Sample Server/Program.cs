using ACS.Messaging;
using System.Text;

// This enables the console to wait until the server ends - useful for containerised services also
ManualResetEventSlim exitevent = new ManualResetEventSlim();
MessageServer ms = new MessageServer(5000, false); // Create a new instance of MessageServer. Configure to listen on port 5000 and for the sample we won't be using encryption

Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Server", "Starting!"));
Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Server", "Attempting to Listen on Port: 5000"));

Console.CancelKeyPress += (sender, EventArgs) =>
{
    EventArgs.Cancel = true;
    exitevent.Set();
};

try
{
    ms.ConnectionAccepted += ConnectionAccepted; // Setup event handler for Connection Accepted
    ms.ConnectionClosed += ConnectionClosed; // Setup event handler for Connection Closed
    ms.MessageReceived += MessageReceived; // Setup event handler for Message Received
    ms.Log += Log; // Setup event handler for Log
    Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Server", "Listening on Port: 5000"));
}
catch (Exception)
{
    Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Server", "Unable to Listen on Port: 5000"));
}

Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Server", "Started!"));

// Console waits here for termination signal
exitevent.Wait();

Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Server", "Stopping!"));

if (ms != null)
{
    ms.Dispose();
}

Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Server", "Stopped!"));
exitevent.Dispose();

void ConnectionAccepted(object sender, ConnectionEventArgs e)
{
    Console.WriteLine(string.Format("{0} - {1} - {2} - {3}", DateTime.UtcNow, "Sample Server", "Connection Accepted", e.Host.ToString()));
}

void ConnectionClosed(object sender, ConnectionEventArgs e)
{
    Console.WriteLine(string.Format("{0} - {1} - {2} - {3}", DateTime.UtcNow, "Sample Server", "Connection Closed", e.Host.ToString()));
}

void MessageReceived(object sender, MessageReceivedEventArgs e)
{
    // e.Data will provide a byte array. NOTE: this is not the complete message as the library currently doesn't handle message framing.
    // You will need to stitch the message together yourself. This example will generally work with short text messages.
    Console.WriteLine(string.Format("{0} - {1} - {2} - {3} - {4}", DateTime.UtcNow, "Sample Server", "Message Received", e.Host.ToString(), Encoding.UTF8.GetString(e.Data)));
}

void Log(object sender, LogEventArgs e)
{
    Console.WriteLine(string.Format("{0} - {1} - {2} - {3}", e.LogDateTime, "Sample Server", e.LogType, e.LogMessage));
}