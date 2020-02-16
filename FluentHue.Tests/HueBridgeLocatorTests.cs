namespace FluentHue.Tests
{
    using FluentHue.Contracts;
    using FluentHue.Exceptions;
    using GenFu.ValueGenerators.Internet;
    using Moq;
    using NUnit.Framework;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    [TestFixture]
    public sealed class HueBridgeLocatorTests
    {
        [Test]
        public void SelectFirst_RequestNotSuccessful()
        {
            HueBridgeLocator.DiscoveryClient = Mock.Of<IRestClient>();
            Mock.Get(HueBridgeLocator.DiscoveryClient)
                .Setup(c => c.ExecuteAsync<IEnumerable<HueBridgeMetadata>>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse<IEnumerable<HueBridgeMetadata>>>(
                    new RestResponse<IEnumerable<HueBridgeMetadata>>()
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        ResponseStatus = ResponseStatus.Completed,
                    }));

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(Exception), async () => await HueBridgeLocator.SelectFirstAsync().ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => HueBridgeLocator.SelectFirst());
            });
        }

        [Test]
        public void SelectFirst_HueBridgeNotFound()
        {
            HueBridgeLocator.DiscoveryClient = Mock.Of<IRestClient>();
            Mock.Get(HueBridgeLocator.DiscoveryClient)
                .Setup(c => c.ExecuteAsync<IEnumerable<HueBridgeMetadata>>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse<IEnumerable<HueBridgeMetadata>>>(
                    new RestResponse<IEnumerable<HueBridgeMetadata>>()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseStatus = ResponseStatus.Completed,
                        Data = new HueBridgeMetadata[0],
                    }));

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(HueBridgeNotFoundException), async () => await HueBridgeLocator.SelectFirstAsync().ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => HueBridgeLocator.SelectFirst());
            });
        }

        [Test]
        public async Task SelectFirst_Successful()
        {
            var data = new HueBridgeMetadata[]
            {
                new HueBridgeMetadata()
                {
                    Id = Guid.NewGuid().ToString(),
                    InternalIpAddress = NetworkAddress.IPAddress(),
                },
                new HueBridgeMetadata()
                {                    
                    Id = Guid.NewGuid().ToString(),
                    InternalIpAddress = NetworkAddress.IPAddress(),
                }
            };

            var expected = new HueBridge(data.First());

            HueBridgeLocator.DiscoveryClient = Mock.Of<IRestClient>();
            Mock.Get(HueBridgeLocator.DiscoveryClient)
                .Setup(c => c.ExecuteAsync<IEnumerable<HueBridgeMetadata>>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse<IEnumerable<HueBridgeMetadata>>>(
                    new RestResponse<IEnumerable<HueBridgeMetadata>>()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseStatus = ResponseStatus.Completed,
                        Data = data,
                    }));

            AssertBridge(expected, await HueBridgeLocator.SelectFirstAsync().ConfigureAwait(false));
            AssertBridge(expected, HueBridgeLocator.SelectFirst());
        }

        [Test]
        public void SelectWithLocalIp()
        {
            string ip = NetworkAddress.IPAddress();
            AssertBridge(new HueBridge(ip), HueBridgeLocator.SelectWithLocalIp(ip));
        }

        private void AssertBridge(IHueBridge expected, IHueBridge bridge)
        {
            Assume.That(expected != null);
            Assert.NotNull(bridge);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expected.Id, bridge.Id);
                Assert.AreEqual(expected.LocalIpAddress, bridge.LocalIpAddress);
            });
        }
    }
}