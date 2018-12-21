# ACS Messaging

[![Build Status](https://dev.azure.com/acslive/ACS-Messaging/_apis/build/status/Johno-ACSLIVE.ACS-Messaging?branchName=master)](https://github.com/Johno-ACSLive/ACS-Messaging/blob/master/README.md#ACS%20Messaging) [![License](https://img.shields.io/github/license/Johno-ACSLive/ACS-Messaging.svg)](https://github.com/Johno-ACSLive/ACS-Messaging/blob/master/README.md#license)

The Advanced Computing Services Messaging library allows Client/Server Communications. Each instance can either be a Server or a Client and supports TLS1.2 encryption.


## History

8 years ago I found a library written by [jmcilhinney](http://www.vbforums.com/member.php?58941-jmcilhinney) who released a project [here](http://www.vbforums.com/showthread.php?587341-VB2008-NET-3-5-Asynchronous-TcpListener-amp-TcpClient). It was a great library to learn async networking in VB.NET. My immediate need was binary data, not text.

The first update was to support binary data for use in all kinds of applications. After that change, a .NET framework version upgrade meant I could update the async methods from using callback methods to using the async/await keywords.

Finally, I added support for encryption a couple of years ago (TLS1.2 is forced if secure communications are enabled). In order support migration to .NET Core / Standard, C# was needed due to VB.NET always lagging behind in terms of support from new/updated frameworks etc.

The final change made in 2016 was from VB.NET to C#. Since that time I have not made any further changes to the library as it works very well and is quite performant. There were probably other bug fixes and small improvements here and there that I have forgotten to mention.


## Getting Started

TODO: Add documentation on library usage.

Very rough info to get you started, the library has intellisense support and should provide a lot of details + the library is very simple to use. I have provided some sample code below to initialise the library. I'll add proper doco's with full examples once I get time.

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
