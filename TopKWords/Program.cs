using CircuitBreaker;
using ClientFactory;
using Common.Config;
using ContentExtraction;
using EssaysProvider.EssaysList;
using EssaysProvider.SingleEssay;
using Logger;
using SimpleInjector;
using TopKWords;
using TopKWordsConfigProvider;
using WordValidation;

internal class Program
{
    private static readonly Container _container = new Container();

    public static async Task Main(string[] args)
    {
        //TODO: provide default config
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
        var configProvider = new TopKWordsConfigFileConfigProvider(configFilePath);

        _container.RegisterInstance<ITopKWordsConfigProvider>(configProvider);
        _container.RegisterInstance<IEssaysProviderConfigProvider>(configProvider);
        _container.RegisterInstance<IHttpClientFactoryConfigProvider>(configProvider);
        _container.RegisterInstance<IWordsBankConfigProvider>(configProvider);

        _container.Register<ILogger, SeriLogger>(Lifestyle.Singleton);
        _container.Register<IEssaysListProvider, GoogleDriveEssaysListProvider>(Lifestyle.Singleton);
        _container.Register<ISingleEssayProvider, HttpEssayContentProvider>(Lifestyle.Singleton);
        _container.Register<IHttpClientFactory, HttpClientFactory>(Lifestyle.Singleton);
        _container.Register<IContentExtractor, EngadgetContentExtractor>(Lifestyle.Singleton);
        _container.Register<IWordsValidator, AtLeastThreeAlphaBeticCharsWordsValidator>(Lifestyle.Singleton);
        _container.Register<IWordsBank, WordsBank>(Lifestyle.Singleton);
        _container.Register<ICircuitBreaker, SimpleCircuitBreaker>(Lifestyle.Singleton);

        _container.Register<TopKWordsFinder>(Lifestyle.Transient);
        _container.Verify();
    }
}