using System.Net;
using MoreLinq.Extensions;

namespace ConsoleApp1;

public class More
{
    public static void ain()
    {
        var dns = Dns.GetHostEntry("google.com");
        dns.Aliases.ForEach(i=>Console.WriteLine(i));
        Console.WriteLine("govno");
        dns.AddressList.ForEach(i=>Console.WriteLine(i));
        Console.WriteLine(dns.HostName);
    }
}