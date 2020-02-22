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

    public sealed class HueBridgeLocatorTests : TestBase
    {
        [Test]
        public void SelectFirst_RequestNotSuccessful()
        {
            Mock.Get(Client)
                .Setup(c => c.ExecuteAsync<IEnumerable<HueBridgeContract>>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse<IEnumerable<HueBridgeContract>>>(
                    new RestResponse<IEnumerable<HueBridgeContract>>()
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
            Mock.Get(Client)
                .Setup(c => c.ExecuteAsync<IEnumerable<HueBridgeContract>>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse<IEnumerable<HueBridgeContract>>>(
                    new RestResponse<IEnumerable<HueBridgeContract>>()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseStatus = ResponseStatus.Completed,
                        Data = new HueBridgeContract[0],
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
            var data = new HueBridgeContract[]
            {
                new HueBridgeContract()
                {
                    Id = Guid.NewGuid().ToString(),
                    InternalIpAddress = NetworkAddress.IPAddress(),
                },
                new HueBridgeContract()
                {                    
                    Id = Guid.NewGuid().ToString(),
                    InternalIpAddress = NetworkAddress.IPAddress(),
                }
            };

            var expected = new HueBridge(data.First());

            Mock.Get(Client)
                .Setup(c => c.ExecuteAsync<IEnumerable<HueBridgeContract>>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse<IEnumerable<HueBridgeContract>>>(
                    new RestResponse<IEnumerable<HueBridgeContract>>()
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