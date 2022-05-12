# ACS Messaging

[![Build Status](https://dev.azure.com/acslive/ACS-Messaging/_apis/build/status/Johno-ACSLIVE.ACS-Messaging?branchName=master)](https://github.com/Johno-ACSLive/ACS-Messaging/blob/master/README.md) [![License](https://img.shields.io/github/license/Johno-ACSLive/ACS-Messaging.svg)](https://github.com/Johno-ACSLive/ACS-Messaging/blob/master/README.md#license)

The Advanced Computing Services Messaging library allows Client/Server Communications. Each instance can either be a Server or a Client and supports TLS1.2 encryption.


## History

In 2010 I found a library written by [jmcilhinney](http://www.vbforums.com/member.php?58941-jmcilhinney) who released a project [here](http://www.vbforums.com/showthread.php?587341-VB2008-NET-3-5-Asynchronous-TcpListener-amp-TcpClient). It was a great library to learn async networking in VB.NET. My immediate need was binary data, not text.

Various updates were made, such as:
* Support binary data for use in all kinds of applications.
* Bump in .NET framework version meant I could switch from using callback methods to the async/await keywords.
* Added support for encryption (TLS1.2 is forced if secure communications are enabled).
* Migration from VB.NET to C# (to migrate to .NET Core / Standard, C# was needed as VB.NET consistently lagged behind in terms of support in new version of .NET etc.).


## Getting Started

TODO: Add documentation on library usage.

Very rough info to get you started, the library has intellisense support and should provide a lot of details as well as the library being very simple to use. Basic sample code below as well as sample server and client projects.

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
```


## Contribute

If you're interested, check how to [Contribute](CONTRIBUTING.md)!


## License

Licensed under Apache License, Version 2.0
