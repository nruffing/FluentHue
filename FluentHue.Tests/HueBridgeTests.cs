namespace FluentHue.Tests
{
    using FluentHue.Contracts;
    using GenFu.ValueGenerators.Internet;
    using Moq;
    using NUnit.Framework;
    using RestSharp;
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class HueBridgeTests : TestBase
    {
        [Test]
        public void Construction_NullContract()
            => Assert.Throws<ArgumentNullException>(() => new HueBridge((HueBridgeContract)null));

        [Test]
        public void Construction_Contract_NullId()
            => Assert.Throws<ArgumentNullException>(() => new HueBridge(new HueBridgeContract()
            {
                Id = null,
                InternalIpAddress = NetworkAddress.IPAddress(),
            }));

        [Test]
        public void Construction_Contract_NullIp()
            => Assert.Throws<ArgumentNullException>(() => new HueBridge(new HueBridgeContract()
            {
                Id = Guid.NewGuid().ToString(),
                InternalIpAddress = null,
            }));

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        public void Contruction_Contract_InvalidId(string id)
            => Assert.Throws<ArgumentException>(() => new HueBridge(new HueBridgeContract()
            {
                Id = id,
                InternalIpAddress = NetworkAddress.IPAddress(),
            }));

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        public void Contruction_Contract_InvalidIp(string ip)
            => Assert.Throws<ArgumentException>(() => new HueBridge(new HueBridgeContract()
            {
                Id = Guid.NewGuid().ToString(),
                InternalIpAddress = ip,
            }));

        [Test]
        public void Construction_Contract()
        {
            string id = Guid.NewGuid().ToString();
            string ip = NetworkAddress.IPAddress();

            IHueBridge bridge = new HueBridge(new HueBridgeContract()
            {
                Id = id,
                InternalIpAddress = ip,
            });

            Assert.NotNull(bridge);
            Assert.AreEqual(id, bridge.Id);
            Assert.AreEqual(ip, bridge.LocalIpAddress);
        }

        [Test]
        public void Construction_Ip()
        {
            string ip = NetworkAddress.IPAddress();

            IHueBridge bridge = new HueBridge(ip);
            
            Assert.NotNull(bridge);
            Assert.AreEqual(ip, bridge.LocalIpAddress);
        }

        [Test]
        public void Contruction_Ip_Null()
            => Assert.Throws<ArgumentNullException>(() => new HueBridge((string)null));

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        public void Contruction_Ip_InvalidIp(string ip)
            => Assert.Throws<ArgumentException>(() => new HueBridge(ip));

        [Test]
        public void WithUser_Null()
        {
            HueBridge bridge = CreateMockBridge();
            Assert.Throws<ArgumentNullException>(() => bridge.WithUser(null));
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        public void WithUser_Invalid(string user)
        {
            HueBridge bridge = CreateMockBridge();
            Assert.Throws<ArgumentException>(() => bridge.WithUser(user));
        }

        [Test]
        public void WithUser()
        {
            string user = Guid.NewGuid().ToString();
            HueBridge bridge = CreateMockBridge();
            bridge.WithUser(user);
            Assert.AreEqual(user, bridge.User);
        }

        [Test]
        public void GetAllLights_NoUserSet()
        {
            HueBridge bridge = CreateMockBridge();

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(ArgumentException), async () => await bridge.GetAllLightsAsync().ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => bridge.GetAllLights());
            });
        }

        [Test]
        public async Task GetAllLights()
        {
            var bridge = CreateMockBridge()
                .WithUser(Guid.NewGuid().ToString());

            var expected = new HueLight((HueBridge)bridge, Guid.NewGuid().ToString(), new HueLightContract()
            {
                Name = Guid.NewGuid().ToString(),
            });

            Mock.Get(Client)
                .Setup(c => c.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Method>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse>(
                    new RestResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseStatus = ResponseStatus.Completed,
                        Content = $"{{\"{expected.Id}\": {{\"name\": \"{expected.Name}\"}}}}",
                    }));

            var lights = await bridge.GetAllLightsAsync().ConfigureAwait(false);
            Assert.NotNull(lights);
            Assert.AreEqual(1, lights.Count());
            var light = lights.First();
            Assert.NotNull(light);
            Assert.AreEqual(expected.Name, light.Name);
            Assert.AreEqual(expected.Id, (light as HueLight).Id);
            Assert.AreSame(bridge, light.End());

            lights = bridge.GetAllLights();
            Assert.NotNull(lights);
            Assert.AreEqual(1, lights.Count());
            light = lights.First();
            Assert.NotNull(light);
            Assert.AreEqual(expected.Name, light.Name);
            Assert.AreEqual(expected.Id, (light as HueLight).Id);
            Assert.AreSame(bridge, light.End());
        }

        [Test]
        public void GetAllLights_NotSuccessfulThrows()
        {
            var bridge = CreateMockBridge()
                .WithUser(Guid.NewGuid().ToString());

            Mock.Get(Client)
                .Setup(c => c.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Method>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse>(
                    new RestResponse()
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        ResponseStatus = ResponseStatus.Completed
                    }));

            Assert.ThrowsAsync(typeof(Exception), async () => await bridge.GetAllLightsAsync().ConfigureAwait(false));
            Assert.Throws<AggregateException>(() => bridge.GetAllLights());
        }

        [Test]
        public async Task DiscoverNewLights_NoSerialNumbers()
        {
            var bridge = CreateMockBridge()
                .WithUser(Guid.NewGuid().ToString());

            Mock.Get(Client)
                .Setup(c => c.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Method>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse>(
                    new RestResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseStatus = ResponseStatus.Completed
                    }))
                .Callback<IRestRequest, Method, CancellationToken>((r, m, t) =>
                {
                    Assert.False(r.Parameters.Any());
                    Assert.AreEqual(Method.POST, m);
                });

            var result = await bridge.DiscoverNewLightsAsync().ConfigureAwait(false);
            Assert.AreSame(bridge, result);

            result = bridge.DiscoverNewLights();
            Assert.AreSame(bridge, result);
        }

        [Test]
        public async Task DiscoverNewLights_WithSerialNumbers()
        {
            var serialNumbers = new string[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
            };

            var bridge = CreateMockBridge()
                .WithUser(Guid.NewGuid().ToString());

            Mock.Get(Client)
                .Setup(c => c.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Method>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse>(
                    new RestResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseStatus = ResponseStatus.Completed
                    }))
                .Callback<IRestRequest, Method, CancellationToken>((r, m, t) =>
                {
                    Assert.True(r.Parameters.Any());
                    Assert.AreEqual(serialNumbers, ((DiscoverNewHueLightsContract)r.Parameters[0].Value).SerialNumbers);
                    Assert.AreEqual(Method.POST, m);
                });

            var result = await bridge.DiscoverNewLightsAsync(serialNumbers).ConfigureAwait(false);
            Assert.AreSame(bridge, result);

            result = bridge.DiscoverNewLights(serialNumbers);
            Assert.AreSame(bridge, result);
        }

        [Test]
        public void DiscoverNewLights_NotSuccessfulThrows()
        {
            var bridge = CreateMockBridge()
                .WithUser(Guid.NewGuid().ToString());

            Mock.Get(Client)
                .Setup(c => c.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Method>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse>(
                    new RestResponse()
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        ResponseStatus = ResponseStatus.Completed
                    }));

            Assert.ThrowsAsync(typeof(Exception), async () => await bridge.DiscoverNewLightsAsync().ConfigureAwait(false));
            Assert.Throws<AggregateException>(() => bridge.DiscoverNewLights());
        }

        [Test]
        public void DiscoverNewLights_NoUserSet()
        {
            HueBridge bridge = CreateMockBridge();

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(ArgumentException), async () => await bridge.DiscoverNewLightsAsync().ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => bridge.DiscoverNewLights());
            });
        }

        [Test]
        public void SelectLight_NoUserSet()
        {
            var light = Guid.NewGuid().ToString();

            HueBridge bridge = CreateMockBridge();

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(ArgumentException), async () => await bridge.SelectLightAsync(light).ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => bridge.SelectLight(light));
            });
        }

        [Test]
        public void SelectLight_NotSuccessfulThrows()
        {
            var light = Guid.NewGuid().ToString();

            var bridge = CreateMockBridge()
                .WithUser(Guid.NewGuid().ToString());

            Mock.Get(Client)
                .Setup(c => c.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Method>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse>(
                    new RestResponse()
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        ResponseStatus = ResponseStatus.Completed
                    }));

            Assert.ThrowsAsync(typeof(Exception), async () => await bridge.SelectLightAsync(light).ConfigureAwait(false));
            Assert.Throws<AggregateException>(() => bridge.SelectLight(light));
        }

        [Test]
        public async Task SelectLight()
        {
            var bridge = CreateMockBridge()
                .WithUser(Guid.NewGuid().ToString());

            var expected = new HueLight((HueBridge)bridge, Guid.NewGuid().ToString(), new HueLightContract()
            {
                Name = Guid.NewGuid().ToString(),
            });

            Mock.Get(Client)
                .Setup(c => c.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Method>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse>(
                    new RestResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseStatus = ResponseStatus.Completed,
                        Content = $"{{\"{expected.Id}\": {{\"name\": \"{expected.Name}\"}}}}",
                    }));

            var light = await bridge.SelectLightAsync(expected.Name).ConfigureAwait(false);
            Assert.NotNull(light);
            Assert.AreEqual(expected.Name, light.Name);
            Assert.AreEqual(expected.Id, (light as HueLight).Id);
            Assert.AreSame(bridge, light.End());

            light = bridge.SelectLight(expected.Name);
            Assert.NotNull(light);
            Assert.AreEqual(expected.Name, light.Name);
            Assert.AreEqual(expected.Id, (light as HueLight).Id);
            Assert.AreSame(bridge, light.End());
        }

        [Test]
        public void SelectLight_LightDoesntExist()
        {
            var bridge = CreateMockBridge()
                .WithUser(Guid.NewGuid().ToString());

            var expected = new HueLight((HueBridge)bridge, Guid.NewGuid().ToString(), new HueLightContract()
            {
                Name = Guid.NewGuid().ToString(),
            });

            Mock.Get(Client)
                .Setup(c => c.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Method>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse>(
                    new RestResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseStatus = ResponseStatus.Completed,
                        Content = $"{{\"{expected.Id}\": {{\"name\": \"{expected.Name}\"}}}}",
                    }));

            Assert.ThrowsAsync(typeof(InvalidOperationException), async () => await bridge.SelectLightAsync(Guid.NewGuid().ToString()).ConfigureAwait(false));
            Assert.Throws<AggregateException>(() => bridge.SelectLight(Guid.NewGuid().ToString()));
        }
    }
}