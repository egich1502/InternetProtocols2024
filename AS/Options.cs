using CommandLine;

namespace AS;

public class Options
{
    [Option('h', "hops", HelpText = "Максимальное число прыжков при поиске узла.", Default = 30)]
    public int Hops { get; set; }

    [Option('w', "wait", HelpText = "Таймаут каждого ответа в миллисекундах.", Default = 10000)]
    public int Wait { get; set; }

    [Value(0, Required = true, MetaName = "конечноеИмя", HelpText = "Конечное имя хоста")]
    public string Hostname { get; set; }
}