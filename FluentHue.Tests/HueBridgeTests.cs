namespace FluentHue.Tests
{
    using FluentHue.Contracts;
    using GenFu.ValueGenerators.Internet;
    using NUnit.Framework;
    using System;

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

        private HueBridge CreateMockBridge()
        {
            var bridge = new HueBridge(NetworkAddress.IPAddress());
            Assume.That(bridge != null);
            return bridge;
        }
    }
}