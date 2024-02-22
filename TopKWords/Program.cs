using Logger;
using SimpleInjector;
using TopKWords;
using TopKWordsConfigProvider;

internal class Program
{
    private static readonly Container _container = new Container();

    public static async Task Main(string[] args)
    {
        if(args.Length < 1 || args[0] == null || args[0] == string.Empty)
        {
            Console.WriteLine("Usage: TopKWords configFilePath");
            return;
        }

        InitializeContainer(args[0]);
        
        var service = _container.GetInstance<TopKWordsFinder>();
        await service.ExecuteAsync();
    }

    private static void InitializeContainer(string configFilePath)
    {
        _container.RegisterInstance<IConfigProvider>(new FileConfigProvider(configFilePath));
        _container.Register<ILogger, SeriLogger>(Lifestyle.Singleton);
        _container.Register<TopKWordsFinder>(Lifestyle.Transient);
        _container.Verify();
    }
}