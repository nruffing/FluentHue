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
        public void Construction_NullMetadata()
            => Assert.Throws<ArgumentNullException>(() => new HueBridge((HueBridgeMetadata)null));

        [Test]
        public void Construction_Metadata_NullId()
            => Assert.Throws<ArgumentNullException>(() => new HueBridge(new HueBridgeMetadata()
            {
                Id = null,
                InternalIpAddress = NetworkAddress.IPAddress(),
            }));

        [Test]
        public void Construction_Metadata_NullIp()
            => Assert.Throws<ArgumentNullException>(() => new HueBridge(new HueBridgeMetadata()
            {
                Id = Guid.NewGuid().ToString(),
                InternalIpAddress = null,
            }));

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        public void Contruction_Metadata_InvalidId(string id)
            => Assert.Throws<ArgumentException>(() => new HueBridge(new HueBridgeMetadata()
            {
                Id = id,
                InternalIpAddress = NetworkAddress.IPAddress(),
            }));

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        public void Contruction_Metadata_InvalidIp(string ip)
            => Assert.Throws<ArgumentException>(() => new HueBridge(new HueBridgeMetadata()
            {
                Id = Guid.NewGuid().ToString(),
                InternalIpAddress = ip,
            }));

        [Test]
        public void Construction_Metadata()
        {
            string id = Guid.NewGuid().ToString();
            string ip = NetworkAddress.IPAddress();

            IHueBridge bridge = new HueBridge(new HueBridgeMetadata()
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

            var expected = new HueLight(bridge, Guid.NewGuid().ToString(), new HueLightMetadata()
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

        private HueBridge CreateMockBridge()
        {
            var bridge = new HueBridge(NetworkAddress.IPAddress());
            Assume.That(bridge != null);
            return bridge;
        }
    }
}