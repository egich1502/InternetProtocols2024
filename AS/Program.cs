using CommandLine;

namespace AS;

class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options => new ASTrace(options.Hostname, options.Wait, options.Hops).DoWork())
            .WithNotParsed(er => Console.WriteLine(string.Join(Environment.NewLine, er)));
    }
}