namespace FluentHue.Tests
{
    using Moq;
    using NUnit.Framework;
    using RestSharp;

    [TestFixture]
    public abstract class TestBase
    {
        protected IRestClient Client;

        [SetUp]
        public virtual void SetUp()
        {
            Client = Mock.Of<IRestClient>();

            Container.Instance = new SimpleInjector.Container();
            Container.Instance.Register<IRestClient>(() => Client, SimpleInjector.Lifestyle.Singleton);
        }
    }
}