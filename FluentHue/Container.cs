using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FluentHue.Tests")]
namespace FluentHue
{
    using RestSharp;
    
    internal static class Container
    {
        internal static SimpleInjector.Container Instance { get; set; }

        static Container()
        {
            Instance = new SimpleInjector.Container();

            Instance.Register<IRestClient>(() => new RestClient(), SimpleInjector.Lifestyle.Transient);
        }
    }
}