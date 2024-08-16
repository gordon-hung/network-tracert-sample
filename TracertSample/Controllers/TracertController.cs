using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

using Microsoft.AspNetCore.Mvc;

using TracertSample.Models;

namespace TracertSample.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TracertController : ControllerBase
{
    [HttpGet("{domain}")]
    public async IAsyncEnumerable<TracertEntry> GetTracertAsync(
        [FromServices] Ping ping,
        string domain = "google.com",
        [FromQuery] int maxHops = 30)
    {
        var ipAddress = (Dns.GetHostAddresses(domain).FirstOrDefault()?.ToString())
            ?? throw new ArgumentException(string.Format("{0} is not a valid IP address.", domain));

        // Ensure that the argument address is valid.
        if (!IPAddress.TryParse(ipAddress, out IPAddress? address))
            throw new ArgumentException(string.Format("{0} is not a valid IP address.", ipAddress));

        var _defaultSendBuffer = new byte[32];
        for (int i = 0; i < 32; i++)
            _defaultSendBuffer[i] = (byte)((int)'a' + i % 23);

        PingOptions pingOptions = new PingOptions(1, true);
        Stopwatch pingReplyTime = new Stopwatch();
        PingReply reply;
        do
        {
            pingReplyTime.Start();
            reply = await ping.SendPingAsync(address, timeout: TimeSpan.FromSeconds(5), buffer: null, options: pingOptions, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);
            pingReplyTime.Stop();

            string hostname = string.Empty;
            if (reply.Address != null && reply.Address.ToString() != IPAddress.Any.ToString() && reply.Address.ToString() != IPAddress.IPv6Any.ToString())
            {
                try
                {
                    hostname = (await Dns.GetHostEntryAsync(reply.Address).ConfigureAwait(false)).HostName;    // Retrieve the hostname for the replied address.
                }
                catch (SocketException) { /* No host available for that address. */ }
            }

            // Return out TracertEntry object with all the information about the hop.
            yield return new TracertEntry()
            {
                HopID = pingOptions.Ttl,
                Address = reply.Address == null ? "N/A" : reply.Address.ToString(),
                Hostname = hostname,
                ReplyTime = pingReplyTime.ElapsedMilliseconds,
                ReplyStatus = reply.Status
            };

            pingOptions.Ttl++;
            pingReplyTime.Reset();
        }
        while (reply.Status != IPStatus.Success && pingOptions.Ttl <= maxHops);
    }
}
