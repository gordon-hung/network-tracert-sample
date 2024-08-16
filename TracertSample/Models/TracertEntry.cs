using System.Net.NetworkInformation;

namespace TracertSample.Models;

public class TracertEntry
{
    /// <summary>
    /// The hop id. Represents the number of the hop.
    /// </summary>
    public int HopID { get; set; }

    /// <summary>
    /// The IP address.
    /// </summary>
    public string Address { get; set; } = default!;

    /// <summary>
    /// The hostname
    /// </summary>
    public string Hostname { get; set; } = default!;

    /// <summary>
    /// The reply time it took for the host to receive and reply to the request in milliseconds.
    /// </summary>
    public long ReplyTime { get; set; }

    /// <summary>
    /// The reply status of the request.
    /// </summary>
    public IPStatus ReplyStatus { get; set; }

    public override string ToString()
    {
        return string.Format("{0} | {1} | {2}",
            HopID,
            string.IsNullOrEmpty(Hostname) ? Address : Hostname + "[" + Address + "]",
            ReplyStatus == IPStatus.TimedOut ? "Request Timed Out." : ReplyTime.ToString() + " ms"
            );
    }
}
