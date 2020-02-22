using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FluentHue.Tests")]
namespace FluentHue
{
    using AutoMapper;
    using RestSharp;

    internal static class Container
    {
        internal static SimpleInjector.Container Instance { get; set; }

        static Container()
        {
            Initialize();
        }

        internal static void Initialize()
        {
            Instance = new SimpleInjector.Container();

            Instance.Register<IRestClient>(() => new RestClient(), SimpleInjector.Lifestyle.Transient);
            Instance.Register<IMapper>(() => new AutoMapper.Mapper(Mapper.Config), SimpleInjector.Lifestyle.Singleton);
        }
    }
}