using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace PortExplorer;

class Program
{
    private static readonly byte[] httpRequest = Encoding.UTF8.GetBytes("GET / HTTP/1.1\r\n\r\n");

    private string http = "HTTP";
    private string smtp = "200";
    private string pop3 = "+OK";

    private static volatile List<string> _collection = new();

    public static async Task Main()
    {
        var host = "e1.ru";
        var staringPort = 53;
        var portCount = 50;

        for (var i = staringPort; i <= staringPort + portCount && i < IPEndPoint.MaxPort; i++)
        {
            await Connect(host, i);
        }

        foreach (var val in _collection)
        {
            Console.WriteLine(val);
        }
    }

    private static async Task Connect(string host, int port)
    {
        var ip = (await Dns.GetHostEntryAsync(host)).AddressList.First();
        TcpConnection(ip, port);
        await UdpConnection(ip, port).ConfigureAwait(false);
    }

    private static bool TcpConnection(IPAddress host, int port)
    {
        try
        {
            using var tcpClient = new TcpClient();
            tcpClient.Client.ReceiveTimeout = 2000;

            tcpClient.Connect(host, port);
            Console.WriteLine($"Discovered port {port}");
            var stream = tcpClient.GetStream();
            stream.Write(httpRequest);
            var data = new byte[128];
            var response = stream.Read(data);
            var message = Encoding.UTF8.GetString(data, 0, 10);

            if (Regex.IsMatch(message, @"^[\+|\-][O|E][K|R]"))
                _collection.Add($"port {port}, protocol pop3, message: {message}");
            if (Regex.IsMatch(message, @"^[2-5][0-5]\d"))
                _collection.Add($"port {port}, protocol smtp, message: {message}");
            if (Regex.IsMatch(message, @"^HTTP\/"))
                _collection.Add($"port {port}, protocol http, message: {message}");

            return true;
        }
        catch (SocketException e)
        {
            return false;
        }
    }

    private static async Task<bool> UdpConnection(IPAddress host, int port)
    {
        using var udpClient = new UdpClient();
        bool returnVal;
        try
        {
            udpClient.Connect(host, port);

            udpClient.Client.ReceiveTimeout = 2000;

            var sendBytes = Encoding.ASCII.GetBytes("hello");
            udpClient.Send(sendBytes, sendBytes.Length);

            var result = udpClient.ReceiveAsync();
            returnVal = await Task.WhenAny(result, Task.Delay(2000)) != result;

            if (!result.IsCompleted) return returnVal;

            var data = result.Result.Buffer;
            if (data[4] == 0xFF)
                _collection.Add($"port {port}, protocol dns");
            else if (data[3] == 0b1)
                _collection.Add($"port {port}, protocol sntp");
            else
                _collection.Add($"port {port}, unknown protocol");

            return returnVal;
        }
        catch (SocketException e)
        {
            returnVal = e.ErrorCode switch
            {
                10054 => false,
                11001 => false,
                _ => true
            };
            if (returnVal)
                _collection.Add($"port {port}, unknown protocol");
        }

        return returnVal;
    }
}