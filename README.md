# ACS Messaging

[![License](https://img.shields.io/github/license/Johno-ACSLive/ACS-Messaging.svg)](https://github.com/Johno-ACSLive/ACS-Messaging/blob/master/README.md#license)

The Advanced Computing Services Messaging library allows Client/Server Communications. Each instance can either be a Server or a Client and supports TLS1.2 encryption.


## History

8 years ago I found a library written by [jmcilhinney](http://www.vbforums.com/member.php?58941-jmcilhinney) who released a project [here](http://www.vbforums.com/showthread.php?587341-VB2008-NET-3-5-Asynchronous-TcpListener-amp-TcpClient). It was a great library to learn async networking in VB.NET. My immediate need was binary data, not text.

The first update was to support binary data for use in all kinds of applications. After that change, a .NET framework version upgrade meant I could update the async methods from using callback methods to using the async/await keywords.

Finally, I added support for encryption a couple of years ago (TLS1.2 is forced if secure communications are enabled). In order support migration to .NET Core / Standard, C# was needed due to VB.NET always lagging behind in terms of support from new/updated frameworks etc.

The final change made in 2016 was from VB.NET to C#. Since that time I have not made any further changes to the library as it works very well and is quite performant. There were probably other bug fixes and small improvements here and there that I have forgotten to mention.


## Getting Started

TODO: Add documentation on library usage.


## Contribute

If you're interested, check how to [Contribute](CONTRIBUTING.md)!


## License

Licensed under Apache License, Version 2.0
