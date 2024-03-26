using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace AS;

public class ASTrace
{
    private readonly int timeout;
    private readonly int maxTtl;
    private readonly string hostname;

    public ASTrace(string hostname, int timeout, int maxTtl)
    {
        this.timeout = timeout;
        this.maxTtl = maxTtl;
        this.hostname = hostname;
    }

    public void DoWork()
    {
        IPHostEntry hostEntry = null!;
        try
        {
            hostEntry = Dns.GetHostEntry(hostname);
        }
        catch (Exception)
        {
            Console.WriteLine($"Не удается разрешить системное имя узла {hostname}");
            Environment.Exit(0);
        }

        Console.WriteLine($"Трассировка автономных систем к {hostEntry.HostName} [{hostEntry.AddressList.First()}]");
        Console.WriteLine($"с максимальным числом прыжков: {maxTtl}:");
        
        foreach (var ipAddress in GetTraceRoute(hostEntry.AddressList.First()))
        {
            var response = PerformRequestToApi(ipAddress).Split('\n').Skip(1).First();
            Console.WriteLine(response);
        }
    }

    private string PerformRequestToApi(IPAddress ipAddresses)
    {
        using var client = new TcpClient("whois.cymru.com", 43);
        var request = Encoding.UTF8.GetBytes($"begin\nverbose\n{ipAddresses}\nend");
        var stream = client.GetStream();
        stream.Write(request, 0, request.Length);

        var data = new byte[client.ReceiveBufferSize];
        stream.Read(data, 0, client.ReceiveBufferSize);

        return Encoding.UTF8.GetString(data);
    }

    private IEnumerable<IPAddress> GetTraceRoute(IPAddress host)
    {
        const int bufferSize = 32;

        var buffer = new byte[bufferSize];
        new Random().NextBytes(buffer);

        using var pinger = new Ping();
        for (var ttl = 1; ttl <= maxTtl; ttl++)
        {
            var options = new PingOptions(ttl, true);
            var reply = pinger.Send(host, timeout, buffer, options);

            if (reply.Status is IPStatus.Success or IPStatus.TtlExpired)
                yield return reply.Address;

            if (reply.Status != IPStatus.TtlExpired && reply.Status != IPStatus.TimedOut)
                break;
        }
    }
}