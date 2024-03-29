﻿using ACS.Messaging;
using System.Text;

ManualResetEventSlim exitevent = new ManualResetEventSlim();
MessageClient mc = new MessageClient("localhost", 5000, false); // Create a new instance of MessageClient. Configure to connect to localhost on port 5000 and for the sample we won't be using encryption

Console.CancelKeyPress += (sender, EventArgs) =>
{
    EventArgs.Cancel = true;
    exitevent.Set();
};

Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Client", "Connecting!"));

try
{
    mc.ConnectionAccepted += ConnectionAccepted; // Setup event handler for Connection Accepted
    mc.ConnectionClosed += ConnectionClosed; // Setup event handler for Connection Closed
    mc.ConnectionFailed += ConnectionFailed; // Setup event handler for Connection Failed
    mc.MessageReceived += MessageReceived; // Setup event handler for Message Received
    mc.Log += Log; // Setup event handler for Log
    mc.Connect();
}
catch (Exception)
{
    Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Client", "Unable to Listen on Port: 5000"));
    Console.WriteLine(string.Format("{0} - {1} - {2}: {3}", DateTime.UtcNow, "Sample Client", "Is Connected", mc.IsConnected));
}

string? message = Console.ReadLine();
if (message != null)
{
    mc.SendData(Encoding.UTF8.GetBytes(message));
}

// Console waits here for termination signal
exitevent.Wait();

Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Client", "Disconnecting!"));

if (mc != null)
{
    mc.Dispose();
}

Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Client", "Disconnected!"));
exitevent.Dispose();

void ConnectionAccepted(object sender, ConnectionEventArgs e)
{
    Console.WriteLine(string.Format("{0} - {1} - {2} - {3}", DateTime.UtcNow, "Sample Client", "Connection Accepted", e.Host.ToString()));
    Console.WriteLine(string.Format("{0} - {1} - {2}: {3}", DateTime.UtcNow, "Sample Client", "Is Connected", mc.IsConnected));
}

void ConnectionClosed(object sender, ConnectionEventArgs e)
{
    Console.WriteLine(string.Format("{0} - {1} - {2} - {3}", DateTime.UtcNow, "Sample Client", "Connection Closed", e.Host.ToString()));
    Console.WriteLine(string.Format("{0} - {1} - {2}: {3}", DateTime.UtcNow, "Sample Client", "Is Connected", mc.IsConnected));
}

void ConnectionFailed(object sender, ConnectionEventArgs e)
{
    Console.WriteLine(string.Format("{0} - {1} - {2} - {3}", DateTime.UtcNow, "Sample Client", "Connection Failed", e.Host.ToString()));
    Console.WriteLine(string.Format("{0} - {1} - {2}: {3}", DateTime.UtcNow, "Sample Client", "Is Connected", mc.IsConnected));
    Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Client", "Waiting 5 seconds to retry..."));
    Task.Delay(5000).Wait();
    Console.WriteLine(string.Format("{0} - {1} - {2}", DateTime.UtcNow, "Sample Client", "Connecting!"));
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