using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FluentHue.Tests")]
namespace FluentHue
{
    using AutoMapper;
    using RestSharp;
    using System.Reflection;

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
            Instance.Register<IMapper>(() => new AutoMapper.Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly().FullName);
            })), SimpleInjector.Lifestyle.Singleton);
        }
    }
}