# ACS Messaging

The Advanced Computing Services Messaging library allows Client/Server Communications. Each instance can either be a Server or a Client and supports TLS1.2 encryption.

The Server now supports basic Access Control Rules with some further validation via a Challenge. Challenge needs to be less than 1KB - in most instances a GUID or something short will be used.


## Getting Started

Basic sample code below, however, intellisense should provide details on usage. The sample projects also contain useful code for using some of the features.

Server Init:
```c#
// Init MessageServer Object
MessageServer MS = new MessageServer(int Port, bool IsSecure, X509Certificate Certificate);

// Attach event handlers to functions
MS.ConnectionAccepted += MS_ConnectionAccepted;
MS.ConnectionClosed += MS_ConnectionClosed;
MS.MessageReceived += MS_MessageReceived;
MS.Log += MS_Log;
```

Client Init:
```C#
// Init MessageClient Object
MessageClient MC = new MessageClient(string Server, int Port, bool IsSecure);

//Attach event handlers to functions
MC.ConnectionAccepted += MC_ConnectionAccepted;
MC.ConnectionClosed += MC_ConnectionClosed;
MC.ConnectionFailed += MC_ConnectionFailed;
MC.MessageReceived += MC_MessageReceived;

// Connect to Remote Host (Server)
MC.Connect();
