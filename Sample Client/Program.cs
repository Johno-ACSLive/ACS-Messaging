using ACS.Messaging;
using System.Text;

ManualResetEventSlim exitevent = new ManualResetEventSlim();
// Configure to connect to localhost on port 5000 and for the sample we won't be using encryption
string host = "localhost";
int port = 5000;
bool issecure = false;
bool ischallengeenabled = false;
string challenge = "bob";
// Create a new instance of MessageClient.
MessageClient mc = new MessageClient(host, port, issecure); 

Console.CancelKeyPress += (sender, EventArgs) =>
{
    EventArgs.Cancel = true;
    exitevent.Set();
};

Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Client", "Connecting!"));

try
{
    RegisterEventHandlers();
    mc.Challenge = challenge;
    mc.IsChallengeEnabled = ischallengeenabled;
    mc.Connect();
}
catch (Exception)
{
    Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Client", "Unable to Listen on Port: 5000"));
    Console.WriteLine(string.Format("{0} - {1} - {2}: {3}", DateTime.UtcNow, "Sample Client", "Is Connected", mc.IsConnected));
}

bool isrunning = true;

while (isrunning)
{
    string? command = Console.ReadLine();
    switch (command)
    {
        case "SendMessage":
            string? message = Console.ReadLine();
            if (message != null) { mc.SendData(Encoding.UTF8.GetBytes(message)); }
            break;
        case "EnableChallenge":
            ischallengeenabled = true;
            break;
        case "DisableChallenge":
            ischallengeenabled = false;
            break;
        case "SetChallengeBob":
            challenge = "bob";
            break;
        case "SetChallengeBob1":
            challenge = "bob1";
            break;
        case "Exit":
            isrunning = false;
            break;
    }
}

// Console waits here for termination signal
exitevent.Wait();

Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Client", "Disconnecting!"));

if (mc != null)
{
    UnregisterEventHandlers();
    mc.Dispose();
}

Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Client", "Disconnected!"));
exitevent.Dispose();

void RegisterEventHandlers()
{
    mc.ConnectionAccepted += ConnectionAccepted; // Register event handler for Connection Accepted
    mc.ConnectionClosed += ConnectionClosed; // Register event handler for Connection Closed
    mc.ConnectionFailed += ConnectionFailed; // Register event handler for Connection Failed
    mc.MessageReceived += MessageReceived; // Register event handler for Message Received
    mc.Log += Log; // Register event handler for Log
}

void UnregisterEventHandlers()
{
    mc.ConnectionAccepted -= ConnectionAccepted; // Unregister event handler for Connection Accepted
    mc.ConnectionClosed -= ConnectionClosed; // Unregister event handler for Connection Closed
    mc.ConnectionFailed -= ConnectionFailed; // Unregister event handler for Connection Failed
    mc.MessageReceived -= MessageReceived; // Unregister event handler for Message Received
    mc.Log -= Log; // Unregister event handler for Log
}

void ConnectionAccepted(object sender, ConnectionEventArgs e)
{
    Console.WriteLine(string.Format("{0} - {1} - {2} - {3}", DateTime.UtcNow, "Sample Client", "Connection Accepted", e.Host.ToString()));
    Console.WriteLine(string.Format("{0} - {1} - {2}: {3}", DateTime.UtcNow, "Sample Client", "Is Connected", mc.IsConnected));
}

void ConnectionClosed(object sender, ConnectionEventArgs e)
{
    Console.WriteLine(string.Format("{0} - {1} - {2} - {3}", DateTime.UtcNow, "Sample Client", "Connection Closed", e.Host.ToString()));
    Console.WriteLine(string.Format("{0} - {1} - {2}: {3}", DateTime.UtcNow, "Sample Client", "Is Connected", mc.IsConnected));
    Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Client", "Waiting 5 seconds to retry..."));
    UnregisterEventHandlers();
    mc.Dispose();
    Task.Delay(5000).Wait();
    Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Client", "Connecting!"));
    mc = new MessageClient(host, port, issecure);
    RegisterEventHandlers();
    mc.Challenge = challenge;
    mc.IsChallengeEnabled = ischallengeenabled;
    mc.Connect();
}

void ConnectionFailed(object sender, ConnectionEventArgs e)
{
    Console.WriteLine(string.Format("{0} - {1} - {2} - {3}", DateTime.UtcNow, "Sample Client", "Connection Failed", e.Host.ToString()));
    Console.WriteLine(string.Format("{0} - {1} - {2}: {3}", DateTime.UtcNow, "Sample Client", "Is Connected", mc.IsConnected));
    Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Client", "Waiting 5 seconds to retry..."));
    UnregisterEventHandlers();
    mc.Dispose();
    Task.Delay(5000).Wait();
    Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Client", "Connecting!"));
    mc = new MessageClient(host, port, issecure);
    RegisterEventHandlers();
    mc.Challenge = challenge;
    mc.IsChallengeEnabled = ischallengeenabled;
    mc.Connect();
}

void MessageReceived(object sender, MessageReceivedEventArgs e)
{
    // e.Data will provide a byte array. NOTE: this is not the complete message as the library currently doesn't handle message framing.
    // You will need to stitch the message together yourself. This example will generally work with short text messages.
    Console.WriteLine(string.Format("{0} - {1} - {2} - {3} - {4}", DateTime.UtcNow, "Sample Client", "Message Received", e.Host.ToString(), Encoding.UTF8.GetString(e.Data)));
}

void Log(object sender, LogEventArgs e)
{
    Console.WriteLine(string.Format("{0} - {1} - {2} - {3}", e.LogDateTime, "Sample Client", e.LogType, e.LogMessage));
}