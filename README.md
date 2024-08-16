# network-tracert-sample
ASP.NET Core 8.0 Tracert

This diagnostic tool determines the path taken to a destination by sending Internet Control Message Protocol (ICMP) echo Request or ICMPv6 messages to the destination with incrementally increasing time to live (TTL) field values. Each router along the path is required to decrement the TTL in an IP packet by at least 1 before forwarding it. Effectively, the TTL is a maximum link counter. When the TTL on a packet reaches 0, the router is expected to return an ICMP time Exceeded message to the source computer.

This command determines the path by sending the first echo Request message with a TTL of 1 and incrementing the TTL by 1 on each subsequent transmission until the target responds or the maximum number of hops is reached. The maximum number of hops is 30 by default and can be specified using the /h parameter.

The path is determined by examining the ICMP time Exceeded messages returned by intermediate routers and the echo Reply message returned by the destination. However, some routers don't return time Exceeded messages for packets with expired TTL values and are invisible to the tracert command. In this case, a row of asterisks (*) is displayed for that hop. The path displayed is the list of near/side router interfaces of the routers in the path between a source host and a destination. The near/side interface is the interface of the router that is closest to the sending host in the path.

```sh
tracert [/d] [/h <maximumhops>] [/j <hostlist>] [/w <timeout>] [/R] [/S <srcaddr>] [/4][/6] <targetname>
```

## example

```sh
tracert google.com
```

## Custom ping payloads on Linux

### Source
- [Breaking change: Custom ping payloads on Linux - .NET | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/core/compatibility/networking/7.0/ping-custom-payload-linux) - Custom ping payloads on Linux

### Previous behavior
In previous versions, the ping packet payload was silently ignored (that is, it wasn't sent) on non-privileged Linux processes.

### New behavior
Starting in .NET 7, a PlatformNotSupportedException is thrown if you attempt to send a custom ping packet payload when running in non-privileged Linux process.

### Version introduced
.NET 7

### Type of breaking change
This change can affect binary compatibility.

### Reason for change
It's better to signal to the user that the operation cannot be performed instead of silently dropping the payload.

### Recommended action
If a ping payload is necessary, run the application as root, or grant the cap_net_raw capability using the setcap utility.

Otherwise, use an overload of `${Ping.SendPingAsync}` that does not accept a custom payload, or pass in an empty array.

