using CommandLine;

namespace AsTrace;

class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options => new AsTrace(options.Hostname, options.Wait, options.Hops).DoWork())
            .WithNotParsed(er => Console.Write(Environment.NewLine));
    }
}